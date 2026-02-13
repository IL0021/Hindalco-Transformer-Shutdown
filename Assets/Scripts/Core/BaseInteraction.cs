using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseInteraction : MonoBehaviour
{
    public event Action<BaseInteraction> OnComplete;
    public abstract IEnumerator Process();
    protected void FinishInteraction() => OnComplete?.Invoke(this);

    // protected void ShowInstruction(string title, string description, string buttonText, Action onClickAction = null)
    // {
    //     if (MainCanvas.Instance == null) return;

    //     MainCanvas.Instance.ShowSingleButtonPanel(title, description, buttonText, onClickAction);
    // }
    // protected void ShowChoices(string title, string description, List<string> options, Action<int> onOptionSelected = null)
    // {
    //     if (MainCanvas.Instance == null) return;

    //     MainCanvas.Instance.ShowFourOptionPanel(title, description, options, onOptionSelected);
    // }
}