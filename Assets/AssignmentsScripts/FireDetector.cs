using UnityEngine;
using System.Collections.Generic;

public class FireDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRadius = 5f;
    public bool isTriggered = false;
    public bool isFire = false;
    
    [Header("Fire Alarms")]
    public List<FireAlarm> fireAlarms = new List<FireAlarm>();
    
    [Header("Visual Feedback")]
    public Light detectorLight;
    public Color normalColor = Color.green;
    public Color alertColor = Color.red;
    public float blinkSpeed = 2f;
    
    [Header("Audio")]
    public AudioSource detectorBeep;
    
    [Header("Debug (VR Testing)")]
    public UnityEngine.UI.Text debugText; // Assign a UI Text element for VR debugging
    public bool enableDetection = false; // Disable detection until manually enabled
    
    private bool isBlinking = false;
    private float blinkTimer = 0f;
    private string lastDebugMessage = "";

    void Start()
    {
        // Ensure clean state at startup
        isTriggered = false;
        isBlinking = false;
        blinkTimer = 0f;
        
        // Setup detector light
        if (detectorLight != null)
        {
            detectorLight.color = normalColor;
        }
        
        // Find all FireAlarm components if not manually assigned
        if (fireAlarms.Count == 0)
        {
            FireAlarm[] alarms = FindObjectsOfType<FireAlarm>();
            fireAlarms.AddRange(alarms);
            Debug.Log($"Fire Detector found {fireAlarms.Count} fire alarms automatically");
        }
        
        // Ensure all alarms start in deactivated state
        foreach (FireAlarm alarm in fireAlarms)
        {
            if (alarm != null && alarm.isActive)
            {
                Debug.Log($"Deactivating pre-activated alarm: {alarm.gameObject.name}");
                alarm.DeactivateAlarm();
            }
        }
        
        // Simple detection setup - no collision needed
        
        Debug.Log("Fire Detector initialized with clean state");
    }

    void Update()
    {
        // Simple timer-based fire detection
        CheckForFireIgnition();
        
        // Handle light blinking when triggered
        if (isTriggered && isBlinking)
        {
            blinkTimer += Time.deltaTime * blinkSpeed;
            float alpha = Mathf.PingPong(blinkTimer, 1f);
            
            if (detectorLight != null)
            {
                detectorLight.color = Color.Lerp(normalColor, alertColor, alpha);
            }
        }
    }

    void CheckForFireIgnition()
    {
        if (isTriggered) return; // Already triggered, no need to check
        
        // Find all FireIgnition components in detection radius
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, detectionRadius);
        
        foreach (Collider obj in nearbyObjects)
        {
            FireIgnition fireIgnition = obj.GetComponent<FireIgnition>();
            if (fireIgnition != null && fireIgnition.IsFireIgnited())
            {
                isFire = true;
                Debug.Log($"Fire Detector: Fire detected from {obj.name}!");
                TriggerFireAlarm();
                return;
            }
        }
    }

    public void TriggerFireAlarm()
    {
        if (isTriggered) return; // Prevent multiple triggers
        
        isTriggered = true;
        isBlinking = true;
        isFire = true;
        
        Debug.Log("🔥 FIRE DETECTED! Triggering all fire alarms! 🔥");
        
        // Play detector beep (will continue playing even after fire is extinguished)
        if (detectorBeep != null)
        {
            detectorBeep.loop = true; // Ensure alarm keeps playing
            detectorBeep.Play();
        }
        
        // Activate all connected fire alarms
        foreach (FireAlarm alarm in fireAlarms)
        {
            if (alarm != null)
            {
                alarm.ActivateAlarm();
            }
        }
        
        // Change light to alert color permanently
        if (detectorLight != null)
        {
            detectorLight.color = alertColor;
        }
    }

    // Add method to allow multiple fire detections (for testing or extinguishing/re-ignition scenarios)
    public void AllowRetrigger()
    {
        isTriggered = false;
        isBlinking = false;
        blinkTimer = 0f;
        
        if (detectorLight != null)
        {
            detectorLight.color = normalColor;
        }
        
        if (detectorBeep != null)
        {
            detectorBeep.Stop();
        }
        
        Debug.Log("Fire Detector reset - ready for new fire detection");
    }

    // Manual reset method (for testing or when fire is extinguished)
    public void ResetDetector()
    {
        isTriggered = false;
        isBlinking = false;
        blinkTimer = 0f;
        
        Debug.Log("Fire Detector reset");
        
        // Reset detector light
        if (detectorLight != null)
        {
            detectorLight.color = normalColor;
        }
        
        // Stop detector beep
        if (detectorBeep != null)
        {
            detectorBeep.Stop();
        }
        
        // Reset all fire alarms
        foreach (FireAlarm alarm in fireAlarms)
        {
            if (alarm != null)
            {
                alarm.DeactivateAlarm();
            }
        }
    }

    // Update debug display for VR testing
    void UpdateDebugDisplay(string message)
    {
        lastDebugMessage = message;
        if (debugText != null)
        {
            debugText.text = $"Fire Detector Debug:\n{message}";
        }
        Debug.Log($"FireDetector: {message}"); // Still log for editor testing
    }

    // Visualize detection radius in Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = isTriggered ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
        // Draw lines to connected fire alarms
        Gizmos.color = Color.blue;
        foreach (FireAlarm alarm in fireAlarms)
        {
            if (alarm != null)
            {
                Gizmos.DrawLine(transform.position, alarm.transform.position);
            }
        }
    }
}