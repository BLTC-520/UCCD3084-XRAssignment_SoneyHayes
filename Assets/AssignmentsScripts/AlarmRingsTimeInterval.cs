using UnityEngine;
using System.Collections;

public class AlarmRingsTimeInterval : MonoBehaviour
{
    [Header("Mock Fire Alarm Settings")]
    public float delayBeforeFire = 20f; // Wait 20 seconds after scene starts
    public FireDetector fireDetector;
    public bool autoStartAlarm = true;
    
    [Header("Debug")]
    public bool showDebugMessages = true;
    
    void Start()
    {
        // Find fire detector if not assigned
        if (fireDetector == null)
        {
            fireDetector = FindObjectOfType<FireDetector>();
        }
        
        if (autoStartAlarm)
        {
            StartCoroutine(TriggerAlarmAfterDelay());
        }
        
        if (showDebugMessages)
            Debug.Log($"AlarmRingsTimeInterval: Will trigger alarm in {delayBeforeFire} seconds");
    }
    
    IEnumerator TriggerAlarmAfterDelay()
    {
        if (showDebugMessages)
            Debug.Log("AlarmRingsTimeInterval: Waiting to trigger mock fire alarm...");
            
        yield return new WaitForSeconds(delayBeforeFire);
        
        if (fireDetector != null)
        {
            if (showDebugMessages)
                Debug.Log("🔥 AlarmRingsTimeInterval: MOCK FIRE! Triggering alarm!");
                
            fireDetector.TriggerFireAlarm();
        }
        else
        {
            Debug.LogError("AlarmRingsTimeInterval: No FireDetector found!");
        }
    }
    
    // Manual trigger for testing
    [ContextMenu("Test Trigger Alarm Now")]
    public void TriggerAlarmNow()
    {
        if (fireDetector != null)
        {
            if (showDebugMessages)
                Debug.Log("AlarmRingsTimeInterval: Manual alarm trigger!");
                
            fireDetector.TriggerFireAlarm();
        }
    }
}