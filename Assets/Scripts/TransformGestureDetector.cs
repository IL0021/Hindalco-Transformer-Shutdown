using UnityEngine;
using UnityEngine.Events;

public class TransformGestureDetector : MonoBehaviour
{
    [SerializeField] private Transform palmReference;
    [Range(0f, 1f)][SerializeField] private float threshold = 0.7f;

    public UnityEvent OnGestureDetected, OnGestureLost;

    private bool wasDetected = false;

    private void Update()
    {
        if (!palmReference) return;

        bool isDetected = Vector3.Dot(palmReference.up, Vector3.up) > threshold;

        if (GameManager.Instance && !GameManager.Instance.teleportationEnabled)
        {
            isDetected = true;
        }

        if (isDetected != wasDetected)
        {
            wasDetected = isDetected;
            (isDetected ? OnGestureDetected : OnGestureLost).Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        if (!palmReference) return;
        Gizmos.color = wasDetected ? Color.green : Color.red;
        Gizmos.DrawRay(palmReference.position, palmReference.up * 0.2f);
    }
}