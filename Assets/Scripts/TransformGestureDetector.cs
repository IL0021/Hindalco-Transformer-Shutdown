using UnityEngine;
using UnityEngine.Events;

public class TransformGestureDetector : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Drag the Transform you attached to your palm here.")]
    [SerializeField] private Transform palmReference;

    [Header("Settings")]
    [Tooltip("How strict is the alignment? (1.0 = Perfect, 0.7 = ~45 degrees off)")]
    [Range(0f, 1f)]
    [SerializeField] private float threshold = 0.7f;

    [Header("Events")]
    public UnityEvent OnGestureDetected; // Ray ON
    public UnityEvent OnGestureLost;     // Ray OFF

    private bool wasDetected = false;

    private void Update()
    {
        if (palmReference == null) return;

        // 1. Check Alignment
        // We compare the Reference's Green Arrow (up) with the World's Ceiling (Vector3.up)
        // You can rotate the reference object in the editor to change what "Up" means relative to your hand.
        float dot = Vector3.Dot(palmReference.up, Vector3.up);
        
        bool isDetected = dot > threshold;

        // 2. Fire Events on State Change
        if (isDetected && !wasDetected)
        {
            OnGestureDetected.Invoke();
            wasDetected = true;
        }
        else if (!isDetected && wasDetected)
        {
            OnGestureLost.Invoke();
            wasDetected = false;
        }
    }
    
    // Optional: Draw Gizmo to visualize the check
    private void OnDrawGizmos()
    {
        if (palmReference == null) return;

        // Draw the Reference's Up vector
        Gizmos.color = wasDetected ? Color.green : Color.red;
        Gizmos.DrawRay(palmReference.position, palmReference.up * 0.2f);
    }
}