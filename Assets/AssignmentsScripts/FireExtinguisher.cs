using UnityEngine;
using System.Collections;

public class FireExtinguisher : MonoBehaviour
{
    [Header("Fire Suppression Settings")]
    public float suppressionRate = 10f; // How fast fire gets suppressed per particle hit
    public float maxFireIntensity = 100f; // Maximum fire intensity
    public float minFireIntensity = 0f; // Minimum before extinguishing
    public float suppressionCooldown = 0.1f; // Cooldown between suppression applications
    
    [Header("Visual Feedback")]
    public bool showSuppressionEffects = true;
    public Color suppressionColor = Color.white;
    
    private float currentFireIntensity;
    private FlammableObject flammableObject;
    private ChemicalContainer chemicalContainer;
    private FireZone fireZone;
    private ParticleSystem fireParticles;
    private bool isSuppressing = false;
    private float lastSuppressionTime;
    
    void Start()
    {
        // Initialize fire intensity to maximum
        currentFireIntensity = maxFireIntensity;
        
        // Get references to fire components
        flammableObject = GetComponent<FlammableObject>();
        chemicalContainer = GetComponent<ChemicalContainer>();
        fireZone = GetComponent<FireZone>();
        
        // Find fire particle system
        fireParticles = GetComponent<ParticleSystem>();
        if (!fireParticles)
        {
            // Look for fire particles in children
            fireParticles = GetComponentInChildren<ParticleSystem>();
        }
        
        // Set up particle system collision if not already configured
        if (fireParticles && !fireParticles.collision.enabled)
        {
            Debug.LogWarning($"Fire particles on {gameObject.name} don't have collision enabled. Please enable collision in Unity Editor for spray interaction to work.");
        }
    }
    
    void OnParticleCollision(GameObject other)
    {
        // Check if the colliding particles are from the spray system
        if (other.name.Contains("FireEX_PS") || other.CompareTag("Smoke"))
        {
            ApplySuppression();
        }
    }
    
    void ApplySuppression()
    {
        // Apply cooldown to prevent over-suppression
        if (Time.time - lastSuppressionTime < suppressionCooldown)
            return;
            
        lastSuppressionTime = Time.time;
        
        // Reduce fire intensity
        currentFireIntensity -= suppressionRate;
        currentFireIntensity = Mathf.Clamp(currentFireIntensity, minFireIntensity, maxFireIntensity);
        
        Debug.Log($"Fire suppression applied to {gameObject.name}. Intensity: {currentFireIntensity:F1}");
        
        // Update visual effects
        UpdateFireVisuals();
        
        // Check if fire should be extinguished
        if (currentFireIntensity <= minFireIntensity)
        {
            ExtinguishCompletely();
        }
        else
        {
            // Start suppression effect
            if (!isSuppressing)
            {
                StartCoroutine(SuppressionEffect());
            }
        }
    }
    
    void UpdateFireVisuals()
    {
        if (!fireParticles) return;
        
        // Scale fire particle system based on intensity
        float intensityRatio = currentFireIntensity / maxFireIntensity;
        
        var main = fireParticles.main;
        var emission = fireParticles.emission;
        var shape = fireParticles.shape;
        
        // Scale emission rate
        emission.rateOverTime = emission.rateOverTime.constant * intensityRatio;
        
        // Scale particle size
        main.startSize = main.startSize.constant * intensityRatio;
        
        // Scale shape radius for spread fires
        if (shape.enabled)
        {
            shape.radius *= intensityRatio;
        }
        
        // Scale object if it's a FireZone
        if (fireZone)
        {
            transform.localScale = Vector3.one * intensityRatio;
        }
    }
    
    IEnumerator SuppressionEffect()
    {
        isSuppressing = true;
        
        // Visual feedback for suppression
        if (showSuppressionEffects && fireParticles)
        {
            var colorOverLifetime = fireParticles.colorOverLifetime;
            if (colorOverLifetime.enabled)
            {
                // Temporarily change fire color to indicate suppression
                Gradient originalGradient = colorOverLifetime.color.gradient;
                Gradient suppressionGradient = new Gradient();
                
                // Create white-tinted gradient
                GradientColorKey[] colorKeys = new GradientColorKey[2];
                colorKeys[0] = new GradientColorKey(suppressionColor, 0.0f);
                colorKeys[1] = new GradientColorKey(Color.white, 1.0f);
                
                GradientAlphaKey[] alphaKeys = originalGradient.alphaKeys;
                suppressionGradient.SetKeys(colorKeys, alphaKeys);
                
                colorOverLifetime.color = suppressionGradient;
                
                // Restore original gradient after effect
                yield return new WaitForSeconds(0.5f);
                colorOverLifetime.color = originalGradient;
            }
        }
        
        isSuppressing = false;
    }
    
    void ExtinguishCompletely()
    {
        Debug.Log($"Fire completely extinguished on {gameObject.name}!");
        
        // Call appropriate extinguish method based on component type
        if (flammableObject && flammableObject.isOnFire)
        {
            flammableObject.ExtinguishFire();
        }
        else if (chemicalContainer && chemicalContainer.isOnFire)
        {
            chemicalContainer.ExtinguishFire();
        }
        else if (fireZone)
        {
            // For fire zones, disable or destroy them
            Destroy(gameObject, 1f); // Slight delay for effect
        }
        
        // Reset intensity for potential re-ignition
        currentFireIntensity = maxFireIntensity;
    }
    
    // Public method to check if fire is being suppressed
    public bool IsBeingSuppressed()
    {
        return currentFireIntensity < maxFireIntensity;
    }
    
    // Public method to get current fire intensity ratio (0-1)
    public float GetFireIntensityRatio()
    {
        return currentFireIntensity / maxFireIntensity;
    }
    
    // Method to manually reduce fire intensity (for other systems)
    public void ReduceFireIntensity(float amount)
    {
        currentFireIntensity -= amount;
        currentFireIntensity = Mathf.Clamp(currentFireIntensity, minFireIntensity, maxFireIntensity);
        UpdateFireVisuals();
        
        if (currentFireIntensity <= minFireIntensity)
        {
            ExtinguishCompletely();
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Visualize current fire intensity
        Gizmos.color = Color.Lerp(Color.red, Color.blue, 1f - (currentFireIntensity / maxFireIntensity));
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}