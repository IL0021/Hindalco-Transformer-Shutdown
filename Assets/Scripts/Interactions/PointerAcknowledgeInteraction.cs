
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PointerAcknowledgeInteraction : BaseInteraction
{
    Button acknowledgeButton;
    GameObject acknowledgeObject;
    public Transform acknowledgeObjectTransform;
    public GameObject confirmationObject;
    // public Transform pointerTransform;

    void Awake()
    {
        acknowledgeButton = FindObjectOfType<MainCanvas>().transform.GetChildWithName("AcknowledgeButton").GetComponent<Button>();
        acknowledgeObject = Resources.Load<GameObject>("Prefabs/AcknowledgeObject");
    }

    public override IEnumerator Process()
    {
        if (acknowledgeObjectTransform != null)
        {
            GameObject ackObj = Instantiate(acknowledgeObject, acknowledgeObjectTransform.position, acknowledgeObjectTransform.rotation);
            var ackObjScript = ackObj.GetComponent<XRGrabInteractable>();
            ackObjScript.hoverEntered.AddListener((selectEnterEvent) =>
            {
                "acknowledge object touched".Print();
                isCheckAcknowledged = true;
                confirmationObject.SetActive(true);
                ackObj.SetActive(false);
            });
            GuidingArrow.Instance.SetTarget(acknowledgeObjectTransform);
            yield return new WaitUntil(() => isCheckAcknowledged);
        }

        // pointerTransform.DOLocalRotate(new Vector3(-6.2f, 0, 0), 7f).SetEase(Ease.InOutSine);

        acknowledgeButton.onClick.AddListener(OnAcknowledgeClicked);
        acknowledgeButton.gameObject.SetActive(true);
        "added listener to acknowledge button".Print();
        yield return new WaitUntil(() => isUIAckBtnClicked);


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
