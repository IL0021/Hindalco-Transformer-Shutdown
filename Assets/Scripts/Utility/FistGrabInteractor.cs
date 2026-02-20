// Should work now. Nahi chala to Gemini ko tang karna

using UnityEngine;
using UnityEngine.XR.Hands;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Attachment;

public class FistGrabInteractor : MonoBehaviour
{
    [Header("Settings")]
    public XRBaseInteractor interactor;
    [SerializeField] private InteractionAttachController attachController;
    public bool isLeftHand = false;
    public float pinchDistanceThreshold = 0.02f;
    public float grabHoldTime = 0.15f;
    public LayerMask grabbableLayer;
    public Transform overlapOffset;
    public float overlapRadius = 0.05f;

    private XRHandSubsystem handSubsystem;
    private float fistTimer = 0f;

    private List<IXRSelectInteractable> nearbyGrabbables = new();

    [Header("Debug (ReadOnly)")]
    [SerializeField] public bool isPinchDetected = false;
    public MonoBehaviour closestInteractable = null;
    [SerializeField] private bool isGrabRequested = false;
    [SerializeField] private bool isCurrentlyGrabbing = false;

    private NearFarInteractor nearFarInteractor;

    private void Start()
    {
        var subsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(subsystems);

        if (subsystems.Count > 0)
        {
            handSubsystem = subsystems[0];
        }
        else
        {
            Debug.LogWarning("XRHandSubsystem not found.");
        }

        nearFarInteractor = interactor.GetComponent<NearFarInteractor>();

    }

    private void Update()
    {
        if (handSubsystem == null || interactor == null) return;

        XRHand hand = isLeftHand ? handSubsystem.leftHand : handSubsystem.rightHand;
        if (!hand.isTracked)
        {
            ResetStates();
            return;
        }

        isPinchDetected = IsPinching(hand);

        nearFarInteractor.enableFarCasting = false;

        if (isPinchDetected)
            fistTimer += Time.deltaTime;
        else
            fistTimer = 0f;

        if (fistTimer >= grabHoldTime && !isCurrentlyGrabbing)
        {
            EnableNearbyGrabbables();
            closestInteractable = FindClosestInteractable();

            if (closestInteractable is IXRSelectInteractable ixrInteractable && (ixrInteractable.interactionLayers & InteractionLayerMask.GetMask("Interaction")) != 0 && !interactor.hasSelection)
            {

                interactor.StartManualInteraction(ixrInteractable);
                isCurrentlyGrabbing = true;
                isGrabRequested = true;
            }
        }
        else if (!isPinchDetected || !interactor.hasSelection)
        {
            if (interactor.hasSelection)
            {
                interactor.EndManualInteraction();
            }
            isCurrentlyGrabbing = false;
            isGrabRequested = false;
        }
    }

    private void ResetStates()
    {
        isPinchDetected = false;
        fistTimer = 0f;
        isGrabRequested = false;
        closestInteractable = null;

        if (interactor.hasSelection)
        {
            interactor.EndManualInteraction();
        }
        isCurrentlyGrabbing = false;
    }

    private void EnableNearbyGrabbables()
    {
        nearbyGrabbables.Clear();
        Vector3 center = overlapOffset != null ? overlapOffset.position : transform.position;
        Collider[] hits = Physics.OverlapSphere(center, overlapRadius, grabbableLayer);

        foreach (var hit in hits)
        {
            Transform current = hit.transform;
            while (current != null)
            {
                var interactables = current.GetComponents<MonoBehaviour>();
                foreach (var mb in interactables)
                {
                    if (mb is IXRSelectInteractable ixr && mb.enabled && !nearbyGrabbables.Contains(ixr))
                    {
                        nearbyGrabbables.Add(ixr);
                    }
                }
                current = current.parent;
            }
        }
    }

    private MonoBehaviour FindClosestInteractable()
    {
        Vector3 center = overlapOffset != null ? overlapOffset.position : transform.position;
        float minDist = float.MaxValue;
        MonoBehaviour closest = null;

        foreach (var interactable in nearbyGrabbables)
        {
            if (interactable is MonoBehaviour mb && mb.enabled)
            {
                Collider col = mb.GetComponentInChildren<Collider>(); // updated: handle child colliders
                if (col)
                {
                    float dist = Vector3.Distance(center, col.ClosestPoint(center));
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closest = mb;
                    }
                }
            }
        }
        return closest;
    }

    public MonoBehaviour GetGrabbedItem() => closestInteractable;

    public bool IsFistActive() => isPinchDetected;


    private bool IsPinching(XRHand hand)
    {
        if (hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out Pose indexPose) &&
            hand.GetJoint(XRHandJointID.ThumbTip).TryGetPose(out Pose thumbPose))
        {
            float distance = Vector3.Distance(indexPose.position, thumbPose.position);
            return distance < pinchDistanceThreshold;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Vector3 center = overlapOffset != null ? overlapOffset.position : transform.position;
        Gizmos.DrawWireSphere(center, overlapRadius);

        if (closestInteractable != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(closestInteractable.transform.position, 0.05f);
        }
    }
}