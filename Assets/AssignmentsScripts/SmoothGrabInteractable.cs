using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SmoothGrabInteractable : XRGrabInteractable
{
    [Header("Smooth Grab Settings")]
    public float grabSmoothTime = 0.5f;  // How long to reach hand
    public float maxGrabDistance = 0.3f;  // Max distance from hand when grabbed
    public bool maintainGrabOffset = true;
    
    private Vector3 grabOffset;
    private Vector3 velocityRef;
    private bool isGrabbing = false;
    
    protected override void Awake()
    {
        base.Awake();
        // Use kinematic movement for smooth interpolation
        movementType = MovementType.Kinematic;
    }
    
    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        // Calculate offset when grab starts
        grabOffset = transform.position - args.interactorObject.transform.position;
        
        // Limit the grab distance
        if (grabOffset.magnitude > maxGrabDistance)
        {
            grabOffset = grabOffset.normalized * maxGrabDistance;
        }
        
        isGrabbing = true;
        base.OnSelectEntering(args);
    }
    
    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
        isGrabbing = false;
        base.OnSelectExiting(args);
    }
    
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);
        
        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic && isGrabbing && isSelected)
        {
            // Smooth movement to hand position with offset
            Vector3 targetPosition = firstInteractorSelecting.transform.position + grabOffset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocityRef, grabSmoothTime);
            
            // Optional: Smooth rotation too
            transform.rotation = Quaternion.Slerp(transform.rotation, firstInteractorSelecting.transform.rotation, Time.deltaTime / grabSmoothTime);
        }
    }
}