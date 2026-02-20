using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using DG.Tweening;
using System.Collections.Generic;
public class ButtonPressInteraction : BaseInteraction
{
    [SerializeField] XRSimpleInteractable button;
    // [SerializeField] XRLever lever;
    // [SerializeField] XRKnob knob;

    [SerializeField] Transform acknowledgeObjectTransform;

    GameObject acknowledgeObject;

    public bool buttonPressed = false;
    [SerializeField] bool isCheckAcknowledged = false;
    public Transform door, handle, handAnim;

    public List<Transform> leftCableHands = new(), rightCableHands = new();

    void Awake()
    {
        acknowledgeObject = Resources.Load<GameObject>("Prefabs/AcknowledgeObject");
    }
    void OnEnable()
    {
        button.selectEntered.AddListener(OnButtonPressed);
        button.selectExited.AddListener(OnButtonReleased);
    }

    private void OnButtonPressed(SelectEnterEventArgs arg0)
    {
        buttonPressed = true;
    }

    private void OnButtonReleased(SelectExitEventArgs arg0)
    {
        buttonPressed = false;
    }

    void OnDisable()
    {
        button.selectEntered.RemoveListener(OnButtonPressed);
        button.selectExited.RemoveListener(OnButtonReleased);
    }
    Tween pokeHandAnim;
    public override IEnumerator Process()
    {

        if (GameManager.Instance.moduleMode == ModuleMode.Training)
        {
            GuidingArrow.Instance.SetTarget(acknowledgeObjectTransform);
            Annotation.Instance.Annotate(acknowledgeObjectTransform, Vector3.up * 0.1f + Vector3.right * 0.1f + Vector3.forward * 0.1f, acknowledgeObjectTransform, Color.white, "Open Panel");
        }

        if (acknowledgeObjectTransform != null)
        {
            GameObject ackObj = Instantiate(acknowledgeObject, acknowledgeObjectTransform.position, acknowledgeObjectTransform.rotation);
            var ackObjScript = ackObj.GetComponent<XRGrabInteractable>();
            ackObjScript.hoverEntered.AddListener((selectEnterEvent) =>
            {
                "acknowledge object touched".Print();
                isCheckAcknowledged = true;
                ackObj.SetActive(false);
            });
            yield return new WaitUntil(() => isCheckAcknowledged);
            if (GameManager.Instance.moduleMode == ModuleMode.Training)
            {
                GuidingArrow.Instance.ClearTarget();
                Annotation.Instance.Annotate();
            }
        }

        handle.DOLocalRotate(new Vector3(0, 180, 0), 3f).SetEase(Ease.InOutSine);
        yield return new WaitForSeconds(3f);

        door.DORotate(door.rotation.eulerAngles + new Vector3(0, 90f, 0), 3f).SetEase(Ease.InOutSine);
        yield return new WaitForSeconds(3f);

        if (handAnim != null)
        {
            handAnim.gameObject.SetActive(true);
            pokeHandAnim = handAnim.DOLocalMoveX(-29.64f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }

        if (GameManager.Instance.moduleMode == ModuleMode.Training)
        {
            GuidingArrow.Instance.SetTarget(button.transform);
            Annotation.Instance.Annotate(button.transform, Vector3.up * 0.1f + Vector3.right * 0.1f + Vector3.forward * 0.1f, button.transform, Color.white, "Press Button");
        }

        yield return new WaitUntil(() => buttonPressed);

        if (GameManager.Instance.moduleMode == ModuleMode.Training)
        {
            GuidingArrow.Instance.ClearTarget();
            Annotation.Instance.Annotate();
        }

        foreach (var hand in leftCableHands)
        {
            hand.DORotate(new Vector3(0, 90, 0), 1f).SetEase(Ease.InOutSine);
        }
        foreach (var hand in rightCableHands)
        {
            hand.DORotate(new Vector3(0, -180, 180), 1f).SetEase(Ease.InOutSine);
        }

        if (handAnim != null)
        {
            pokeHandAnim.Kill();
            handAnim.gameObject.SetActive(false);
        }

        door.DORotate(door.rotation.eulerAngles + new Vector3(0, -90f, 0), 3f).SetEase(Ease.InOutSine);
        yield return new WaitForSeconds(3f);
        handle.DOLocalRotate(new Vector3(0, 90, 0), 3f).SetEase(Ease.InOutSine);


        FinishInteraction();
    }
}
