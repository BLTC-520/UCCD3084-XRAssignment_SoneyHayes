using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class PinLock : MonoBehaviour
{
    [Header("Pin Settings")]
    public bool isPulledOut = false;
    public float pullDistance = 0.1f; // Minimum distance to pull before pin releases
    public AudioSource pinPullSound;
    
    [Header("Haptic Feedback")]
    public float hapticIntensity = 0.5f;
    public float hapticDuration = 0.2f;

    private ConfigurableJoint joint;
    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    private Vector3 initialPosition;
    private Transform initialParent;
    private bool isBeingGrabbed = false;

    void Awake()
    {
        joint = GetComponent<ConfigurableJoint>();
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        
        // Store initial position and parent for distance calculation
        initialPosition = transform.localPosition;
        initialParent = transform.parent;

        // Set up grab events for pin itself
        grabInteractable.selectEntered.AddListener(OnPinGrabbed);
        grabInteractable.selectExited.AddListener(OnPinReleased);
    }

    void Update()
    {
        // Check pull distance only when pin is being grabbed but not yet pulled out
        if (isBeingGrabbed && !isPulledOut && initialParent != null)
        {
            float currentDistance = Vector3.Distance(transform.position, initialParent.TransformPoint(initialPosition));
            
            if (currentDistance >= pullDistance)
            {
                PullOutPin();
            }
        }
    }

    void OnPinGrabbed(SelectEnterEventArgs args)
    {
        isBeingGrabbed = true;
        Debug.Log("Pin grabbed - pull to remove!");
    }

    void OnPinReleased(SelectExitEventArgs args)
    {
        isBeingGrabbed = false;
        
        // If pin wasn't pulled out far enough, it snaps back (realistic behavior)
        if (!isPulledOut && joint != null)
        {
            Debug.Log("Pin released before pulling out - snapping back to position");
            // Optional: Add snap-back animation here
        }
    }

    void PullOutPin()
    {
        if (isPulledOut) return;
        
        Debug.Log("Pin successfully pulled out!");
        
        // Remove joint constraint
        if (joint != null)
        {
            Destroy(joint);
        }

        // Enable physics
        rb.isKinematic = false;
        rb.useGravity = true;

        // Detach from parent
        transform.parent = null;

        // Mark as pulled out
        isPulledOut = true;

        // Play sound feedback
        if (pinPullSound != null)
        {
            pinPullSound.Play();
        }

        // Haptic feedback
        if (grabInteractable.isSelected)
        {
            var interactor = grabInteractable.firstInteractorSelecting;
            if (interactor is XRBaseControllerInteractor controllerInteractor)
            {
                controllerInteractor.SendHapticImpulse(hapticIntensity, hapticDuration);
            }
        }
    }
}
