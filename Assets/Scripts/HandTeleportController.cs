using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;


public class HandTeleportController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private XRRayInteractor rayInteractor;
    [SerializeField] private TeleportationProvider teleportationProvider;

    [Header("Settings")]
    // If true, the ray is hidden until you are ready to teleport (optional)
    [SerializeField] private bool toggleRayVisibility = false; 

    private void Start()
    {
        // Auto-find components if not assigned
        if (rayInteractor == null) rayInteractor = GetComponent<XRRayInteractor>();
        if (teleportationProvider == null) teleportationProvider = FindObjectOfType<TeleportationProvider>();
    }

    /// <summary>
    /// Call this function from your Palm Button's "OnPress" event.
    /// </summary>
    public void TryTeleport()
    {
        // 1. Check if the ray is actually hitting something valid
        if (!rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            Debug.Log("Teleport Failed: Ray is not hitting anything.");
            return;
        }

        // 2. check if the hit object is a valid Teleportation Area/Anchor
        // The XR Ray Interactor handles the layer mask validation, so if we hit something, 
        // it's likely on the Teleport layer. We just need to construct the request.

        TeleportRequest request = new TeleportRequest()
        {
            destinationPosition = hit.point,
            destinationRotation = transform.rotation, // Or target rotation if using Anchors
            matchOrientation = MatchOrientation.TargetUpAndForward
        };

        // 3. Send the request to the system
        teleportationProvider.QueueTeleportRequest(request);
    }
    
    // Optional: Call this to show/hide the ray based on hand state
    public void SetRayActive(bool isActive)
    {
        rayInteractor.gameObject.SetActive(isActive);
    }
}