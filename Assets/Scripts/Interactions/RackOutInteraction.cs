using System.Collections;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using DG.Tweening;

public class RackOutInteraction : BaseInteraction
{
    GameObject key, handle;
    [SerializeField] GameObject keyInstance, handleInstance;

    [SerializeField] Transform keyTransform, handleTransform;
    [SerializeField] Transform keyAnimTransform, handleAnimTransform;
    void Awake()
    {
        key = Resources.Load<GameObject>("Prefabs/Key");
        handle = Resources.Load<GameObject>("Prefabs/Handle");
    }

    public int turns = 0;
    public Tween keyInsertAnim, keyRotateAnim, handleInsertAnim, handleRotateAnim;

    public override IEnumerator Process()
    {

        keyInstance = Instantiate(key, keyTransform.position, keyTransform.rotation);

        var lids = transform.GetChildrenWithNameContaining("LeverLid");

        foreach (var lid in lids) lid.gameObject.SetActive(false);

        var keySocket = transform.GetChildWithName("KeySocket");
        keySocket.gameObject.SetActive(true);

        var keySocketInteractor = keySocket.GetComponent<XRSocketInteractor>();


        if (keyAnimTransform != null && handleAnimTransform != null)
        {
            keyAnimTransform.gameObject.SetActive(true);
            keyInsertAnim = keyAnimTransform.DOLocalMoveX(-0.04f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
            GuidingArrow.Instance.SetTarget(keyInstance.transform);
            Annotation.Instance.Annotate(keyInstance.transform, Vector3.up * 0.1f + Vector3.left * 0.1f, keyInstance.transform, Color.white, "Pick Key");
        }

        yield return new WaitUntil(() => keySocketInteractor.hasSelection);


        if (keyAnimTransform != null && handleAnimTransform != null)
        {
            keyInsertAnim.Kill();

            keyAnimTransform.localPosition = new Vector3(-0.0127f, keyAnimTransform.localPosition.y, keyAnimTransform.localPosition.z);
            keyRotateAnim = keyAnimTransform.DOLocalRotate(new Vector3(0, 0, 0f), 1f).SetLoops(-1, LoopType.Restart).SetEase(Ease.InOutSine);
            GuidingArrow.Instance.SetTarget(keyInstance.transform);
            Annotation.Instance.Annotate(keyInstance.transform, Vector3.up * 0.1f + Vector3.left * 0.1f, keyInstance.transform, Color.white, "Rotate Key");

        }



        var rotateKey = transform.GetChildWithName("RotateKey");
        var keyKnob = rotateKey.GetComponent<XRKnob>();

        rotateKey.gameObject.SetActive(true);
        keyInstance.gameObject.SetActive(false);
        keySocket.gameObject.SetActive(false);

        yield return new WaitUntil(() => keyKnob.value == 0);


        if (keyAnimTransform != null && handleAnimTransform != null)
        {
            keyRotateAnim.Kill();
            keyAnimTransform.gameObject.SetActive(false);
        }
        keyKnob.enabled = false;

        handleInstance = Instantiate(handle, handleTransform.position, handleTransform.rotation);

        if (keyAnimTransform != null && handleAnimTransform != null)
        {
            handleAnimTransform.gameObject.SetActive(true);
            handleInsertAnim = handleAnimTransform.DOLocalMoveX(-0.04f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
            GuidingArrow.Instance.SetTarget(handleInstance.transform);
            Annotation.Instance.Annotate(handleInstance.transform, Vector3.up * 0.1f + Vector3.left * 0.1f, handleInstance.transform, Color.white, "Pick Handle");
        }


        var handleSocket = transform.GetChildWithName("HandleSocket");
        handleSocket.gameObject.SetActive(true);

        var handleSocketInteractor = handleSocket.GetComponent<XRSocketInteractor>();

        yield return new WaitUntil(() => handleSocketInteractor.hasSelection);


        if (keyAnimTransform != null && handleAnimTransform != null)
        {
            handleInsertAnim.Kill();
            handleRotateAnim = handleAnimTransform.DOLocalRotate(new Vector3(0, 90, 0f), 1f).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
            GuidingArrow.Instance.SetTarget(handleInstance.transform);
            Annotation.Instance.Annotate(handleInstance.transform, Vector3.up * 0.1f + Vector3.left * 0.1f, handleInstance.transform, Color.white, "Rotate Handle");
        }


        var rotateHandle = transform.GetChildWithName("RotateHandle");
        var handleKnob = rotateHandle.GetComponent<XRKnob>();
        turns = 0;

        handleKnob.onValueChange.AddListener((value) =>
        {
            turns = Mathf.FloorToInt(value);
        });
        rotateHandle.gameObject.SetActive(true);
        handleInstance.gameObject.SetActive(false);
        handleSocket.gameObject.SetActive(false);

        yield return new WaitUntil(() => turns <= -2);


        if (keyAnimTransform != null && handleAnimTransform != null)
        {
            handleRotateAnim.Kill();
            handleAnimTransform.gameObject.SetActive(false);
            GuidingArrow.Instance.ClearTarget();
            Annotation.Instance.Annotate();
        }


        yield return new WaitUntil(() => turns <= -10);

        rotateHandle.gameObject.SetActive(false);
        rotateKey.gameObject.SetActive(false);

        foreach (var lid in lids) lid.gameObject.SetActive(true);


        FinishInteraction();
    }
}
