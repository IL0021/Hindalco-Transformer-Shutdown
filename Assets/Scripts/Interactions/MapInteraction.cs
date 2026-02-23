using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MapInteraction : BaseInteraction
{
    public string locationTarget;
    public List<Button> buttons = new();

    public SerializedDictionary<string, Button> buttonDict = new();
    public SerializedDictionary<string, TeleportInteraction> teleportDict = new();

    public GameObject map;
    [ContextMenu("Get Buttons")]
    public void GetButtons()
    {
        buttons = GetComponentsInChildren<Button>().ToList();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    Tween animateButton;
    public override IEnumerator Process()
    {
        transform.position = MainCanvas.Instance.transform.position;
        transform.rotation = MainCanvas.Instance.transform.rotation;
        MainCanvas.Instance.gameObject.SetActive(false);
        map.SetActive(true);
        GuidingArrow.Instance.SetTarget(transform);


        Button concernedButton = buttonDict[locationTarget];
        TeleportInteraction teleportInteraction = teleportDict[locationTarget];

        if (concernedButton != null && GameManager.Instance.moduleMode == ModuleMode.Training)
        {
            animateButton = concernedButton.GetComponent<Image>().DOColor(Color.green, 0.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }

        if (concernedButton == null)
        {
            StartCoroutine(teleportInteraction.Process());
            map.SetActive(false);
            MainCanvas.Instance.gameObject.SetActive(true);
            GuidingArrow.Instance.ClearTarget();
            yield return new WaitForSeconds(teleportInteraction.delayTime);
            FinishInteraction();
            yield break;
        }
        bool isButtonPressed = false;
        concernedButton.onClick.AddListener(
            () =>
            {
                isButtonPressed = true;
                animateButton.Kill();
                concernedButton.GetComponent<Image>().color = new Color32(94, 166, 204, 255);
            }
        );

        yield return new WaitUntil(() => isButtonPressed);
        
        yield return StartCoroutine(teleportInteraction.Process());
        FinishInteraction();

        map.SetActive(false);
        MainCanvas.Instance.gameObject.SetActive(true);
    }
}
