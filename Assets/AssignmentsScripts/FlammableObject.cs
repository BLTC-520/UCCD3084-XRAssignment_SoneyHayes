using UnityEngine;
using System.Collections;

public class FlammableObject : MonoBehaviour
{
    [Header("Fire Settings")]
    public ParticleSystem fireParticles;
    public AudioSource fireSound;
    public float burnDuration = 12f; // How long object burns
    public bool destroyWhenBurned = false; // Whether to destroy object after burning
    
    [Header("Fire Spread Settings")]
    public float fireSpreadRadius = 2.5f;
    public float fireSpreadDelay = 2f; // Delay before spreading fire to other objects
    public LayerMask flammableLayerMask = -1;
    
    [Header("Visual Effects")]
    public bool changeColorWhenBurning = true;
    public Color burnedColor = new Color(0.3f, 0.2f, 0.1f, 1f); // Dark brownish color
    
    public bool isOnFire = false;
    private Material originalMaterial;
    private Renderer objRenderer;
    private Color originalColor;
    
    void Start()
    {
        // Get renderer and store original material
        objRenderer = GetComponent<Renderer>();
        if (objRenderer && objRenderer.material)
        {
            originalMaterial = objRenderer.material;
            originalColor = originalMaterial.color;
        }
        
        // Setup fire particles (initially stopped)
        if (fireParticles) fireParticles.Stop();
        
        // Setup fire sound
        if (fireSound) 
        {
            fireSound.loop = true;
            fireSound.Stop();
        }
    }
    
    public void CatchFire()
    {
        if (isOnFire) return;
        
        isOnFire = true;
        
        Debug.Log($"Flammable object {gameObject.name} caught fire!");
        
        // Start fire particle effect
        if (fireParticles)
        {
            fireParticles.Play();
        }
        
        // Play fire burning sound
        if (fireSound)
        {
            fireSound.Play();
        }
        
        // Start visual burning effect
        if (changeColorWhenBurning)
        {
            StartCoroutine(BurnVisualEffect());
        }
        
        // Start spreading fire to nearby objects after delay
        StartCoroutine(SpreadFireAfterDelay());
        
        // Handle object destruction if enabled
        if (destroyWhenBurned)
        {
            StartCoroutine(DestroyAfterBurning());
        }
    }
    
    IEnumerator BurnVisualEffect()
    {
        if (!objRenderer || !originalMaterial) yield break;
        
        // Create new material instance to avoid affecting other objects
        Material burnMaterial = new Material(originalMaterial);
        objRenderer.material = burnMaterial;
        
        float burnTime = 0f;
        
        while (burnTime < burnDuration && isOnFire)
        {
            float burnProgress = burnTime / burnDuration;
            
            // Gradually change color from original to burned
            Color currentColor = Color.Lerp(originalColor, burnedColor, burnProgress);
            burnMaterial.color = currentColor;
            
            burnTime += Time.deltaTime;
            yield return null;
        }
        
        // Final burned color
        if (isOnFire)
        {
            burnMaterial.color = burnedColor;
        }
    }
    
    IEnumerator SpreadFireAfterDelay()
    {
        yield return new WaitForSeconds(fireSpreadDelay);
        
        // Find nearby flammable objects
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, fireSpreadRadius, flammableLayerMask);
        
        foreach (Collider obj in nearbyObjects)
        {
            if (obj.gameObject == gameObject) continue; // Skip self
            
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            
            // Spread fire to other flammable objects
            FlammableObject otherFlammable = obj.GetComponent<FlammableObject>();
            if (otherFlammable && !otherFlammable.isOnFire)
            {
                Debug.Log($"Fire spreading from {gameObject.name} to flammable object {obj.name}!");
                yield return new WaitForSeconds(0.5f); // Dramatic delay
                otherFlammable.CatchFire();
            }
            
            // Spread fire to chemical containers
            ChemicalContainer container = obj.GetComponent<ChemicalContainer>();
            if (container && !container.isOnFire)
            {
                Debug.Log($"Fire spreading from {gameObject.name} to chemical container {obj.name}!");
                yield return new WaitForSeconds(0.3f);
                container.StartFire();
            }
        }
    }
    
    IEnumerator DestroyAfterBurning()
    {
        yield return new WaitForSeconds(burnDuration);
        
        Debug.Log($"{gameObject.name} completely burned - destroying object");
        
        // Stop fire effects before destroying
        if (fireParticles) fireParticles.Stop();
        if (fireSound) fireSound.Stop();
        
        // Optional: Fade out effect
        yield return StartCoroutine(FadeOutAndDestroy());
    }
    
    IEnumerator FadeOutAndDestroy()
    {
        if (objRenderer)
        {
            Material fadeMaterial = objRenderer.material;
            Color fadeColor = fadeMaterial.color;
            
            float fadeTime = 2f;
            float elapsedTime = 0f;
            
            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
                fadeColor.a = alpha;
                fadeMaterial.color = fadeColor;
                yield return null;
            }
        }
        
        Destroy(gameObject);
    }
    
    // Manually extinguish fire (optional - for fire extinguisher mechanics)
    public void ExtinguishFire()
    {
        if (!isOnFire) return;
        
        isOnFire = false;
        
        Debug.Log($"{gameObject.name} fire extinguished!");
        
        // Stop fire effects
        if (fireParticles) fireParticles.Stop();
        if (fireSound) fireSound.Stop();
        
        // Stop all burning coroutines
        StopAllCoroutines();
    }
    
    // Visualize fire spread radius in Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fireSpreadRadius);
    }
}