using System;
using System.Collections;
using UnityEngine;

public class BreakerInteraction : BaseInteraction
{
    public XRLeverNeutral lever;
    public bool targetValue;

    public Action OnTrip, OnClose;

    void Awake()
    {
        lever = GetComponentInChildren<XRLeverNeutral>();
    }

    void OnEnable()
    {
        if (lever != null)
        {
            lever.onLeverActivate.AddListener(OnLeverActivate);
            lever.onLeverDeactivate.AddListener(OnLeverDeactivate);
        }
    }

    void OnDisable()
    {
        if (lever != null)
        {
            lever.onLeverActivate.RemoveListener(OnLeverActivate);
            lever.onLeverDeactivate.RemoveListener(OnLeverDeactivate);
        }
    }

    private void OnLeverDeactivate() => OnClose?.Invoke();
    private void OnLeverActivate() => OnTrip?.Invoke();

    public override IEnumerator Process()
    {
        GameManager.Instance.ToggleCanvasAnimation();
        if (GameManager.Instance.moduleMode == ModuleMode.Training)
        {
            GuidingArrow.Instance.SetTarget(lever.handle.transform);
            Annotation.Instance.Annotate(lever.handle.transform, new Vector3(-0.3f, 0f, 0f), lever.handle.transform, Color.white, "Hold");
        }

        lever.value = targetValue;
        if (targetValue)
        {
            OnTrip?.Invoke();
        }
        else
        {
            OnClose?.Invoke();
        }
        lever.hoverEntered.AddListener((args) =>
        {
            if (GameManager.Instance.moduleMode == ModuleMode.Training)
            {
                GuidingArrow.Instance.SetTarget(lever.handle.transform);
                Annotation.Instance.Annotate(lever.handle.transform, new Vector3(-0.3f, 0f, 0f), lever.handle.transform, Color.white, "Rotate");
            }
        });

        Debug.Log("Breaker initialized.");
        yield return new WaitUntil(() => lever.value != targetValue);

        lever.hoverEntered.RemoveAllListeners();

        if (GameManager.Instance.moduleMode == ModuleMode.Training)
        {
            GuidingArrow.Instance.ClearTarget();
            Annotation.Instance.Annotate();
        }


        ClearHighlights();
        FinishInteraction();
    }


    public void UpdateHighlights()
    {
        if (GameManager.Instance.moduleMode == ModuleMode.Assessment) return;
        "Updating Highlights".Print();
        Highlighter highlighter = GetComponentInChildren<Highlighter>();
        if (highlighter != null)
        {
            highlighter.ClearHighlights();
            if (targetValue)
            {
                var controller = Resources.Load<RuntimeAnimatorController>("AnimControllers/BreakerClose");
                highlighter.animatorController = controller;
            }
            else
            {
                var controller = Resources.Load<RuntimeAnimatorController>("AnimControllers/BreakerTrip");
                highlighter.animatorController = controller;
            }
            highlighter.CreateHighlights();
            "highlights recreated".Print();
        }
    }

    public void ClearHighlights()
    {
        Highlighter highlighter = GetComponentInChildren<Highlighter>();
        if (highlighter != null)
        {
            highlighter.ClearHighlights();
        }
    }

    // public void RecreateHighlights()
    // {
    //     Highlighter highlighter = GetComponentInChildren<Highlighter>();
    //     if (highlighter != null)
    //     {
    //         highlighter.CreateHighlights();
    //     }
    // }

    // public void ResetHighlight()
    // {
    //     Highlighter highlighter = GetComponentInChildren<Highlighter>();
    //     if (highlighter != null)
    //     {
    //         highlighter.ClearHighlights();
    //     }
    // }
}
