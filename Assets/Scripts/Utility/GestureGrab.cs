using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;

public class GestureGrab : MonoBehaviour
{
    public Transform thumbTip, indexTip;

    [SerializeField] private float pinchThreshold = 0.02f; // Distance to trigger pinch
    [SerializeField] private float releaseThreshold = 0.04f; // Distance to reset (Hysteresis)
    [SerializeField] private float pinchHoldTime = 1f;

    public UnityEvent OnPinchStarted;
    private bool isPinching = false;
    private Coroutine pinchCoroutine;
    public GameObject LocoSymbol;
    public float radialAmount = 0f;

    Image bg;
    void Awake()
    {
        bg = LocoSymbol.GetComponentInChildren<Image>();
    }

    void Update()
    {
        if (!thumbTip || !indexTip || !GameManager.Instance.teleportationEnabled) return;

        float currentDistance = Vector3.Distance(thumbTip.position, indexTip.position);

        if (!isPinching)
        {
            if (currentDistance <= pinchThreshold)
            {
                if (pinchCoroutine == null)
                    pinchCoroutine = StartCoroutine(PinchRoutine());
            }
            else
            {
                if (pinchCoroutine != null)
                {
                    StopCoroutine(pinchCoroutine);
                    pinchCoroutine = null;
                    LocoSymbol.SetActive(false);
                    bg.fillAmount = 0;
                    radialAmount = 0;
                }
            }
        }
        else if (currentDistance > releaseThreshold)
        {
            LocoSymbol.SetActive(false);
            bg.fillAmount = 0;
            radialAmount = 0;
            isPinching = false;
        }
    }

    private IEnumerator PinchRoutine()
    {
        LocoSymbol.SetActive(true);
        radialAmount = 0f;
        float timer = 0f;
        while (timer < pinchHoldTime)
        {
            timer += Time.deltaTime;
            radialAmount = timer / pinchHoldTime;
            bg.fillAmount = radialAmount;
            yield return null;
        }

        isPinching = true;
        OnPinchStarted.Invoke();
        pinchCoroutine = null;
        LocoSymbol.SetActive(false);
    }

    void OnDrawGizmos()
    {
        if (thumbTip && indexTip)
        {
            Gizmos.color = isPinching ? Color.green : Color.red;
            Gizmos.DrawLine(thumbTip.position, indexTip.position);
        }
    }
}