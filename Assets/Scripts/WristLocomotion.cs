using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.UI; // For XROrigin

public class WristLocomotion : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 2.0f;

    [Header("References")]
    [SerializeField] private XROrigin xrOrigin;
    [SerializeField] private CharacterController characterController;

    // Internal state
    private bool isMovingForward = false;
    private bool isMovingBackward = false;

    // public Button forwardButton;
    // public Button backwardButton;

    private void Start()
    {
        // Auto-find components if missing
        if (xrOrigin == null) xrOrigin = FindObjectOfType<XROrigin>();
        if (characterController == null && xrOrigin != null)
            characterController = xrOrigin.GetComponent<CharacterController>();
        // forwardButton.onClick.AddListener(OnForwardPress);
        // backwardButton.onClick.AddListener(OnBackwardPress);
    }

    private void Update()
    {
        // If no buttons are held, do nothing
        if (!isMovingForward && !isMovingBackward) return;

        // Determine direction based on Headset (Camera) look direction
        // We flatten the Y so you don't fly into the sky when looking up
        Vector3 forwardDir = xrOrigin.Camera.transform.forward;
        forwardDir.y = 0; 
        forwardDir.Normalize();

        Vector3 moveDirection = Vector3.zero;

        if (isMovingForward) moveDirection += forwardDir;
        if (isMovingBackward) moveDirection -= forwardDir;

        // Apply Movement
        if (characterController != null)
        {
            // Use CharacterController for collision support
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
        }
        else
        {
            // Fallback: Raw transform movement (goes through walls)
            xrOrigin.transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
    }

    // --- PUBLIC METHODS FOR BUTTON EVENTS ---

    public void OnForwardPress() => isMovingForward = true;
    public void OnForwardRelease() => isMovingForward = false;

    public void OnBackwardPress() => isMovingBackward = true;
    public void OnBackwardRelease() => isMovingBackward = false;
}