using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;


public class HandTeleportController : MonoBehaviour
{
    [SerializeField] private XRRayInteractor rayInteractor;
    [SerializeField] private TeleportationProvider teleportationProvider;

    private void Start()
    {
        if (!rayInteractor) rayInteractor = GetComponent<XRRayInteractor>();
        if (!teleportationProvider) teleportationProvider = FindObjectOfType<TeleportationProvider>();
    }

    public void TryTeleport()
    {
        if (GameManager.Instance && !GameManager.Instance.teleportationEnabled) return;

        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            teleportationProvider.QueueTeleportRequest(new TeleportRequest
            {
                destinationPosition = hit.point,
                destinationRotation = transform.rotation,
                matchOrientation = MatchOrientation.None
            });
        }
    }
    
    public void SetRayActive(bool isActive)
    {
        if (isActive && GameManager.Instance && !GameManager.Instance.teleportationEnabled)
        {
            isActive = false;
        }
        rayInteractor.gameObject.SetActive(isActive);
    }
}