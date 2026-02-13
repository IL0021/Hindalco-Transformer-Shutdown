
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using DG.Tweening;
using System;

public class AcknowledgeInteraction : BaseInteraction
{
    Button acknowledgeButton;
    GameObject acknowledgeObject;
    public Transform acknowledgeObjectTransform;
    public bool skipButton = false;
    public GameObject pokeAnim;

    public ModuleMode moduleMode;
    // public GameObject confirmationObject;
    void Awake()
    {
        acknowledgeButton = FindObjectOfType<MainCanvas>().transform.GetChildWithName("AcknowledgeButton").GetComponent<Button>();
        acknowledgeObject = Resources.Load<GameObject>("Prefabs/Check");
        pokeAnim = Resources.Load<GameObject>("Prefabs/PokeAnim");
    }
    private GameObject pokeHand;
    public override IEnumerator Process()
    {
        if (acknowledgeObjectTransform != null)
        {
            GameObject ackObj = Instantiate(acknowledgeObject, acknowledgeObjectTransform.position, acknowledgeObjectTransform.rotation, acknowledgeObjectTransform);
            var ackGrabInteractable = ackObj.GetComponent<XRGrabInteractable>();
            ackGrabInteractable.hoverEntered.AddListener((selectEnterEvent) =>
            {
                "acknowledge object touched".Print();
                isCheckAcknowledged = true;
                ackGrabInteractable.enabled = false;
            });
            if (moduleMode == ModuleMode.Training)
            {
                pokeHand = Instantiate(pokeAnim, acknowledgeObjectTransform);
                pokeHand.transform.DOLocalMoveZ(0.2f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                GuidingArrow.Instance.SetTarget(acknowledgeObjectTransform);
            }
            yield return new WaitUntil(() => isCheckAcknowledged);
            
            if (moduleMode == ModuleMode.Training)
            {
                GuidingArrow.Instance.ClearTarget();
                Destroy(pokeHand);
            }
        }

        if (!skipButton)
        {
            acknowledgeButton.onClick.AddListener(OnAcknowledgeClicked);
            acknowledgeButton.gameObject.SetActive(true);
            "added listener to acknowledge button".Print();
            yield return new WaitUntil(() => isUIAckBtnClicked);
        }

        FinishInteraction();

    }
    bool isCheckAcknowledged = false;
    bool isUIAckBtnClicked = false;

    private void OnAcknowledgeClicked()
    {
        isUIAckBtnClicked = true;
        acknowledgeButton.onClick.RemoveListener(OnAcknowledgeClicked);
        acknowledgeButton.gameObject.SetActive(false);
    }
}
