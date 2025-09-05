using UnityEngine;
using System.Collections;

public class FireIgnition : MonoBehaviour
{
    [Header("Fire Particle System")]
    public ParticleSystem fireParticleSystem;
    public AudioSource fireSound;
    
    [Header("Ignition Settings")]
    public bool isIgnited = false;
    public float ignitionDelay = 0.5f; // Delay before enabling collision detection
    public bool autoIgniteOnStart = false;
    
    [Header("Visual Effects")]
    public Light fireLight;
    public Color fireColor = Color.red;
    public float lightIntensity = 2f;
    
    [Header("Debug")]
    public bool showDebugMessages = true;
    
    private ParticleSystem.CollisionModule collisionModule;
    private bool collisionEnabled = false;
    
    void Start()
    {
        SetupFireSystem();
        
        if (autoIgniteOnStart)
        {
            StartCoroutine(AutoIgniteAfterDelay(1f));
        }
    }
    
    void SetupFireSystem()
    {
        // Get particle system if not assigned
        if (fireParticleSystem == null)
            fireParticleSystem = GetComponent<ParticleSystem>();
        
        if (fireParticleSystem == null)
        {
            Debug.LogError("FireIgnition: No ParticleSystem found!");
            return;
        }
        
        // Setup collision module but keep it disabled initially
        collisionModule = fireParticleSystem.collision;
        collisionModule.enabled = false;
        collisionEnabled = false;
        
        // Ensure Play On Awake is false as mentioned by user
        var main = fireParticleSystem.main;
        main.playOnAwake = false;
        
        // Setup fire light
        if (fireLight != null)
        {
            fireLight.color = fireColor;
            fireLight.intensity = 0f; // Start with no light
        }
        
        if (showDebugMessages)
            Debug.Log($"FireIgnition: Fire system setup complete for {gameObject.name}");
    }
    
    public void IgniteFire()
    {
        if (isIgnited) return;
        
        isIgnited = true;
        
        if (showDebugMessages)
            Debug.Log($"🔥 Fire ignited on {gameObject.name}!");
        
        // Start particle system
        if (fireParticleSystem != null)
        {
            fireParticleSystem.Play();
        }
        
        // Play fire sound
        if (fireSound != null)
        {
            fireSound.loop = true;
            fireSound.Play();
        }
        
        // Start light effect
        if (fireLight != null)
        {
            StartCoroutine(FadeInFireLight());
        }
        
        // Enable collision detection after delay
        StartCoroutine(EnableCollisionAfterDelay());
        
        // Notify other components that fire has started
        NotifyFireStarted();
    }
    
    IEnumerator EnableCollisionAfterDelay()
    {
        yield return new WaitForSeconds(ignitionDelay);
        
        if (isIgnited && fireParticleSystem != null)
        {
            collisionModule.enabled = true;
            collisionEnabled = true;
            
            if (showDebugMessages)
                Debug.Log($"Fire collision detection enabled for {gameObject.name}");
        }
    }
    
    IEnumerator FadeInFireLight()
    {
        if (fireLight == null) yield break;
        
        float fadeTime = 1f;
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeTime && isIgnited)
        {
            elapsedTime += Time.deltaTime;
            float intensity = Mathf.Lerp(0f, lightIntensity, elapsedTime / fadeTime);
            fireLight.intensity = intensity;
            yield return null;
        }
        
        if (isIgnited)
            fireLight.intensity = lightIntensity;
    }
    
    IEnumerator AutoIgniteAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        IgniteFire();
    }
    
    public void ExtinguishFire()
    {
        if (!isIgnited) return;
        
        isIgnited = false;
        
        if (showDebugMessages)
            Debug.Log($"Fire extinguished on {gameObject.name}");
        
        // Stop particle system
        if (fireParticleSystem != null)
        {
            fireParticleSystem.Stop();
        }
        
        // Stop fire sound
        if (fireSound != null)
        {
            fireSound.Stop();
        }
        
        // Disable collision detection
        if (collisionEnabled)
        {
            collisionModule.enabled = false;
            collisionEnabled = false;
        }
        
        // Turn off fire light
        if (fireLight != null)
        {
            StartCoroutine(FadeOutFireLight());
        }
        
        // Stop all coroutines
        StopAllCoroutines();
        
        // Notify other components that fire has stopped
        NotifyFireStopped();
    }
    
    IEnumerator FadeOutFireLight()
    {
        if (fireLight == null) yield break;
        
        float fadeTime = 0.5f;
        float elapsedTime = 0f;
        float startIntensity = fireLight.intensity;
        
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float intensity = Mathf.Lerp(startIntensity, 0f, elapsedTime / fadeTime);
            fireLight.intensity = intensity;
            yield return null;
        }
        
        fireLight.intensity = 0f;
    }
    
    void NotifyFireStarted()
    {
        // Notify FlammableObject component if present
        FlammableObject flammable = GetComponent<FlammableObject>();
        if (flammable != null)
        {
            flammable.CatchFire();
        }
        
        // Notify ChemicalContainer component if present
        ChemicalContainer container = GetComponent<ChemicalContainer>();
        if (container != null)
        {
            container.StartFire();
        }
    }
    
    void NotifyFireStopped()
    {
        // Notify FlammableObject component if present
        FlammableObject flammable = GetComponent<FlammableObject>();
        if (flammable != null)
        {
            flammable.ExtinguishFire();
        }
        
        // Notify ChemicalContainer component if present
        ChemicalContainer container = GetComponent<ChemicalContainer>();
        if (container != null && container.isOnFire)
        {
            // Use reflection or direct method if available
            container.ExtinguishFire();
        }
    }
    
    // Public methods for external control
    public bool IsFireIgnited()
    {
        return isIgnited;
    }
    
    public bool IsCollisionEnabled()
    {
        return collisionEnabled;
    }
    
    // Manual test methods
    [ContextMenu("Test Ignite Fire")]
    public void TestIgnite()
    {
        IgniteFire();
    }
    
    [ContextMenu("Test Extinguish Fire")]
    public void TestExtinguish()
    {
        ExtinguishFire();
    }
    
    // Visualize fire range in Scene view
    void OnDrawGizmosSelected()
    {
        if (isIgnited)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1f);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}