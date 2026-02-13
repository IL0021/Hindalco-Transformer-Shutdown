
using UnityEngine;
using DG.Tweening;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;
using System;

public class Check : MonoBehaviour
{
    void Awake()
    {
        Animate();
        GetComponent<XRGrabInteractable>().hoverEntered.AddListener(OnHoverEntered);
    }

    private void OnHoverEntered(HoverEnterEventArgs arg0)
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    void Animate()
    {
        var currentRotation = transform.rotation;
        transform.DORotateQuaternion(currentRotation * Quaternion.Euler(0, 60f, 0), 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
}
