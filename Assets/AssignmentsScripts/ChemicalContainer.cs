using UnityEngine;
using System.Collections;

public class ChemicalContainer : MonoBehaviour
{
    [Header("Liquid Settings")]
    public ParticleSystem liquidParticles;
    public ParticleSystem fireParticles;
    // sound is sequential 
    // container falls -> spill -> fire burns
    public AudioSource containerFallSound;
    public AudioSource spillSound;
    public AudioSource fireSound;
    public float spillThreshold = 3f; // Velocity needed to spill when hitting ground
    public float fireDelay = 1.5f; // Time before chemical ignites
    
    [Header("Fire Spread Settings")]
    public float fireSpreadRadius = 4f;
    public float fireSpreadDelay = 1f;
    public LayerMask flammableLayerMask = -1; // What objects can catch fire
    
    [Header("Fire Suppression Settings")]
    public float maxFireIntensity = 100f;
    public float suppressionRate = 15f; // How much intensity is reduced per smoke particle hit
    public float suppressionCooldown = 0.1f; // Cooldown between particle hits
    public bool showSuppressionEffects = true;
    
    private Rigidbody rb;
    public bool hasSpilled = false;
    public bool isOnFire = false;
    private float currentFireIntensity;
    private float lastSuppressionTime;
    private bool isSuppressing = false;

    private bool hasPlayedFallSound = false; 
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Initialize fire intensity
        currentFireIntensity = maxFireIntensity;
        
        // Setup particles (initially stopped)
        if (liquidParticles) liquidParticles.Stop();
        if (fireParticles) fireParticles.Stop();

        // Setup audio sources
        if (containerFallSound)
        {
            containerFallSound.loop = false;
            containerFallSound.Stop();
        }
        if (spillSound) spillSound.loop = false;
        if (fireSound) 
        {
            fireSound.loop = true; // Fire sound should loop
            fireSound.Stop();
        }
        
        // Setup fire particles collision detection if available
        if (fireParticles && !fireParticles.collision.enabled)
        {
            Debug.LogWarning($"Fire particles on {gameObject.name} don't have collision enabled. Enable collision for smoke detection to work.");
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"{gameObject.name}: Collision with {collision.gameObject.name}! hasSpilled: {hasSpilled}, hasPlayedFallSound: {hasPlayedFallSound}, velocity: {collision.relativeVelocity.magnitude}");
        
        if (!hasSpilled && collision.relativeVelocity.magnitude > spillThreshold)
        {
            if (collision.gameObject.CompareTag("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Default"))
            {
                Debug.Log($"{gameObject.name}: Valid ground collision - playing fall sound");
                
                if (containerFallSound && !hasPlayedFallSound)
                {
                    containerFallSound.PlayOneShot(containerFallSound.clip);
                    hasPlayedFallSound = true;
                }
                
                SpillLiquid();
            }
        }
    }
        
    public void SpillLiquid()
    {
        if (hasSpilled) return;
        
        hasSpilled = true;
        
        Debug.Log($"{gameObject.name} chemical spilled! Igniting in {fireDelay} seconds...");

        // Play spill sound after 1 second (++ real)
        StartCoroutine(StartSpillEffectsAfterDelay(1f)); // 1 sec delay
        
        // Chemical reacts with oxygen - start fire after delay
        StartCoroutine(IgniteAfterDelay());
    }
    
    IEnumerator StartSpillEffectsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Play spill sound
        if (spillSound)
        {
            spillSound.Play();
        }
        
        // Start liquid particles at the same time as sound
        if (liquidParticles)
        {
            liquidParticles.Play();
            
            // Stop liquid particles after a few seconds
            StartCoroutine(StopLiquidAfterDelay(5f));
        }
    }
    IEnumerator StopLiquidAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (liquidParticles) liquidParticles.Stop();
    }
    
    IEnumerator IgniteAfterDelay()
    {
        yield return new WaitForSeconds(fireDelay);
        StartFire();
    }
    
    public void StartFire()
    {
        if (isOnFire) return;
        
        isOnFire = true;
        
        Debug.Log($"{gameObject.name} caught fire! Spreading to nearby objects...");
        
        // Start fire particle effect
        if (fireParticles)
        {
            fireParticles.Play();
        }
        
        // Play fire sound (looping)
        if (fireSound)
        {
            fireSound.Play();
        }
        
        // Create fire damage zone around this object
        CreateFireZone();
        
        // Start spreading fire to nearby flammable objects
        StartCoroutine(SpreadFireToNearbyObjects());
    }
    
    // Particle collision detection for smoke particles from fire extinguisher
    void OnParticleCollision(GameObject other)
    {
        // Check if the colliding particles are from smoke/fire extinguisher
        if (other.name.Contains("smoke") || other.name.Contains("Smoke") || 
            other.CompareTag("Smoke") || other.name.Contains("FireEX"))
        {
            if (isOnFire)
            {
                ApplySuppression();
            }
        }
    }
    
    void ApplySuppression()
    {
        // Apply cooldown to prevent over-suppression from rapid particle hits
        if (Time.time - lastSuppressionTime < suppressionCooldown)
            return;
            
        lastSuppressionTime = Time.time;
        
        // Reduce fire intensity
        currentFireIntensity -= suppressionRate;
        currentFireIntensity = Mathf.Clamp(currentFireIntensity, 0f, maxFireIntensity);
        
        Debug.Log($"Smoke suppression applied to {gameObject.name}! Fire intensity: {currentFireIntensity:F1}/{maxFireIntensity}");
        
        // Update fire visual effects based on intensity
        UpdateFireVisuals();
        
        // Check if fire should be completely extinguished
        if (currentFireIntensity <= 0f)
        {
            ExtinguishFire();
        }
        else
        {
            // Show suppression effect
            if (!isSuppressing && showSuppressionEffects)
            {
                StartCoroutine(ShowSuppressionEffect());
            }
        }
    }
    
    void UpdateFireVisuals()
    {
        if (!fireParticles || !isOnFire) return;
        
        // Calculate intensity ratio (0 to 1)
        float intensityRatio = currentFireIntensity / maxFireIntensity;
        
        // Scale fire particle effects based on intensity
        var main = fireParticles.main;
        var emission = fireParticles.emission;
        
        // Reduce emission rate
        emission.rateOverTime = emission.rateOverTime.constant * intensityRatio;
        
        // Reduce particle size
        main.startSize = main.startSize.constant * intensityRatio;
        
        // Adjust fire sound volume
        if (fireSound && fireSound.isPlaying)
        {
            fireSound.volume = intensityRatio * 0.8f; // Max volume 0.8
        }
    }
    
    IEnumerator ShowSuppressionEffect()
    {
        isSuppressing = true;
        
        // Visual feedback - briefly change fire color to show suppression
        if (fireParticles && showSuppressionEffects)
        {
            var colorModule = fireParticles.colorOverLifetime;
            if (colorModule.enabled)
            {
                // Store original gradient
                Gradient originalGradient = colorModule.color.gradient;
                
                // Create suppression gradient (more white/blue tint)
                Gradient suppressionGradient = new Gradient();
                GradientColorKey[] colorKeys = new GradientColorKey[2];
                colorKeys[0] = new GradientColorKey(Color.white, 0.0f);
                colorKeys[1] = new GradientColorKey(Color.cyan, 1.0f);
                
                GradientAlphaKey[] alphaKeys = originalGradient.alphaKeys;
                suppressionGradient.SetKeys(colorKeys, alphaKeys);
                
                colorModule.color = suppressionGradient;
                
                // Return to original after brief effect
                yield return new WaitForSeconds(0.3f);
                colorModule.color = originalGradient;
            }
        }
        
        yield return new WaitForSeconds(0.2f);
        isSuppressing = false;
    }

    // Manually extinguish fire (for fire extinguisher mechanics)
    public void ExtinguishFire()
    {
        if (!isOnFire) return;
        
        isOnFire = false;
        
        Debug.Log($"{gameObject.name} chemical fire completely extinguished!");
        
        // Stop fire effects
        if (fireParticles) fireParticles.Stop();
        if (fireSound) fireSound.Stop();
        
        // Stop all burning coroutines
        StopAllCoroutines();
        
        // Reset fire intensity for potential re-ignition
        currentFireIntensity = maxFireIntensity;
    }
    
    // Public methods for external access
    public float GetFireIntensityRatio()
    {
        return currentFireIntensity / maxFireIntensity;
    }
    
    public bool IsBeingSuppressed()
    {
        return currentFireIntensity < maxFireIntensity;
    }
    
    // Public method to apply external suppression (for DetectFire.cs and other systems)
    public void ApplyExternalSuppression(float amount = -1f)
    {
        if (!isOnFire) return;
        
        float suppressionAmount = amount > 0 ? amount : suppressionRate;
        
        // Apply suppression with cooldown
        if (Time.time - lastSuppressionTime < suppressionCooldown)
            return;
            
        lastSuppressionTime = Time.time;
        
        currentFireIntensity -= suppressionAmount;
        currentFireIntensity = Mathf.Clamp(currentFireIntensity, 0f, maxFireIntensity);
        
        Debug.Log($"External suppression applied to {gameObject.name}! Fire intensity: {currentFireIntensity:F1}/{maxFireIntensity}");
        
        UpdateFireVisuals();
        
        if (currentFireIntensity <= 0f)
        {
            ExtinguishFire();
        }
        else if (!isSuppressing && showSuppressionEffects)
        {
            StartCoroutine(ShowSuppressionEffect());
        }
    }
    
    IEnumerator SpreadFireToNearbyObjects()
    {
        yield return new WaitForSeconds(fireSpreadDelay);
        
        // Find all nearby objects that can catch fire
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, fireSpreadRadius, flammableLayerMask);
        
        foreach (Collider obj in nearbyObjects)
        {
            if (obj.gameObject == gameObject) continue; // Skip self
            
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            Debug.Log($"Found nearby object: {obj.name} at distance {distance}");
            
            // Try to ignite other chemical containers
            ChemicalContainer otherContainer = obj.GetComponent<ChemicalContainer>();
            if (otherContainer && !otherContainer.isOnFire)
            {
                Debug.Log($"Fire spreading to chemical container: {obj.name}!");
                yield return new WaitForSeconds(0.5f); // Small delay for dramatic effect
                otherContainer.StartFire();
            }
            
            // Try to ignite flammable objects (boxes, etc.)
            FlammableObject flammableObj = obj.GetComponent<FlammableObject>();
            if (flammableObj && !flammableObj.isOnFire)
            {
                Debug.Log($"Fire spreading to flammable object: {obj.name}!");
                yield return new WaitForSeconds(0.3f); // Small delay between ignitions
                flammableObj.CatchFire();
            }
        }
    }
    
    void CreateFireZone()
    {
        // Create invisible fire damage zone
        GameObject fireZone = new GameObject($"FireZone_{gameObject.name}");
        fireZone.transform.position = transform.position;
        fireZone.transform.SetParent(transform); // Move with the container
        
        // Add trigger collider for fire zone
        SphereCollider fireCollider = fireZone.AddComponent<SphereCollider>();
        fireCollider.isTrigger = true;
        fireCollider.radius = fireSpreadRadius;
        
        // Add fire zone script
        FireZone fireScript = fireZone.AddComponent<FireZone>();
        fireScript.parentContainer = this;
        
        // Fire zone lasts for extended time
        Destroy(fireZone, 20f);
    }
    
    // Visualize fire spread radius in Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fireSpreadRadius);
    }
}