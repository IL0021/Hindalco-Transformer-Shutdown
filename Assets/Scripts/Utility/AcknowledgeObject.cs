using UnityEngine;
using DG.Tweening;
public class AcknowledgeObject : MonoBehaviour
{
    void Start()
    {
        Animate();
    }

    void Animate()
    {
        var currentScale = transform.localScale;
        transform.DOScale(currentScale * 1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
}