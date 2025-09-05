using UnityEngine;
using System.Collections;

public class FireAlarm : MonoBehaviour
{
    [Header("Alarm Settings")]
    public bool isActive = false;
    public float alarmDuration = 30f; // How long alarm stays active (-1 for infinite)
    
    [Header("Audio Settings")]
    public AudioSource alarmSound;
    public AudioClip[] alarmClips; // Different alarm sounds to choose from
    public float volume = 1f;
    public bool loopAlarm = true;
    
    [Header("Visual Effects")]
    public Light alarmLight;
    public Color alarmColor = Color.red;
    public Color normalColor = Color.white;
    public float flashSpeed = 3f;
    public bool flashLight = true;
    
    [Header("Particle Effects")]
    public ParticleSystem alarmParticles; // Optional warning particles
    
    [Header("Animation")]
    public Animator alarmAnimator; // Optional animator for alarm movement
    public string alarmTrigger = "AlarmTriggered";
    
    private bool isFlashing = false;
    private float flashTimer = 0f;
    private Color originalLightColor;
    private float originalLightIntensity;
    private Coroutine alarmCoroutine;

    void Awake()
    {
        // Setup audio source
        if (alarmSound == null)
        {
            alarmSound = gameObject.AddComponent<AudioSource>();
        }
        
        alarmSound.loop = loopAlarm;
        alarmSound.volume = volume;
        alarmSound.playOnAwake = false;
        
        // Setup random alarm sound if multiple clips provided
        if (alarmClips.Length > 0)
        {
            alarmSound.clip = alarmClips[Random.Range(0, alarmClips.Length)];
        }
        
        // Store original light settings
        if (alarmLight != null)
        {
            originalLightColor = alarmLight.color;
            originalLightIntensity = alarmLight.intensity;
            alarmLight.color = normalColor;
        }
        
        // Setup particles
        if (alarmParticles != null)
        {
            alarmParticles.Stop();
        }
    }

    void Update()
    {
        // Handle light flashing when alarm is active
        if (isActive && flashLight && alarmLight != null)
        {
            flashTimer += Time.deltaTime * flashSpeed;
            float alpha = Mathf.PingPong(flashTimer, 1f);
            
            // Flash between alarm color and normal color
            alarmLight.color = Color.Lerp(normalColor, alarmColor, alpha);
            
            // Also flash intensity for more dramatic effect
            alarmLight.intensity = Mathf.Lerp(originalLightIntensity * 0.5f, originalLightIntensity * 1.5f, alpha);
        }
    }

    public void ActivateAlarm()
    {
        if (isActive) return; // Already active
        
        isActive = true;
        isFlashing = true;
        flashTimer = 0f;
        
        Debug.Log($"🚨 Fire Alarm {gameObject.name} ACTIVATED! 🚨");
        
        // Start alarm sound
        if (alarmSound != null && alarmSound.clip != null)
        {
            alarmSound.Play();
        }
        
        // Start light effects
        if (alarmLight != null)
        {
            alarmLight.enabled = true;
            if (!flashLight)
            {
                alarmLight.color = alarmColor;
                alarmLight.intensity = originalLightIntensity * 1.5f;
            }
        }
        
        // Start particle effects
        if (alarmParticles != null)
        {
            alarmParticles.Play();
        }
        
        // Trigger animation
        if (alarmAnimator != null)
        {
            alarmAnimator.SetTrigger(alarmTrigger);
        }
        
        // Auto-deactivate after duration (if set)
        if (alarmDuration > 0)
        {
            alarmCoroutine = StartCoroutine(AutoDeactivateAfterDelay());
        }
    }

    public void DeactivateAlarm()
    {
        if (!isActive) return; // Already inactive
        
        isActive = false;
        isFlashing = false;
        
        Debug.Log($"Fire Alarm {gameObject.name} deactivated");
        
        // Stop alarm sound
        if (alarmSound != null)
        {
            alarmSound.Stop();
        }
        
        // Reset light
        if (alarmLight != null)
        {
            alarmLight.color = originalLightColor;
            alarmLight.intensity = originalLightIntensity;
        }
        
        // Stop particle effects
        if (alarmParticles != null)
        {
            alarmParticles.Stop();
        }
        
        // Stop auto-deactivate coroutine
        if (alarmCoroutine != null)
        {
            StopCoroutine(alarmCoroutine);
            alarmCoroutine = null;
        }
    }

    IEnumerator AutoDeactivateAfterDelay()
    {
        yield return new WaitForSeconds(alarmDuration);
        
        Debug.Log($"Fire Alarm {gameObject.name} auto-deactivating after {alarmDuration} seconds");
        DeactivateAlarm();
    }

    // Manual toggle for testing
    public void ToggleAlarm()
    {
        if (isActive)
        {
            DeactivateAlarm();
        }
        else
        {
            ActivateAlarm();
        }
    }

    // Test activation method
    [ContextMenu("Test Activate Alarm")]
    public void TestActivateAlarm()
    {
        ActivateAlarm();
    }

    [ContextMenu("Test Deactivate Alarm")]
    public void TestDeactivateAlarm()
    {
        DeactivateAlarm();
    }

    // Ensure alarm stops when object is destroyed
    void OnDestroy()
    {
        if (alarmCoroutine != null)
        {
            StopCoroutine(alarmCoroutine);
        }
    }

}