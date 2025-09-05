using UnityEngine;
using System.Collections;

public class DetectFire : MonoBehaviour
{
    [Header("Smoke Suppression Settings")]
    public float suppressionRate = 20f; // How much intensity reduced per particle hit
    public float suppressionCooldown = 0.08f; // Cooldown between particle applications
    public bool showDebugMessages = true;
    
    [Header("Continuous Suppression (OnTriggerStay)")]
    public bool enableContinuousSuppression = true;
    public float continuousSuppressionRate = 8f; // Per second when in trigger zone
    
    private float lastSuppressionTime;
    private bool isActivelySuppressing = false;
    
    void Start()
    {
        // Ensure this GameObject has a Collider set as trigger for backup detection
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            // Add a trigger collider if none exists
            SphereCollider triggerCol = gameObject.AddComponent<SphereCollider>();
            triggerCol.isTrigger = true;
            triggerCol.radius = 0.5f;
            if (showDebugMessages)
                Debug.Log($"DetectFire: Added trigger collider to {gameObject.name}");
        }
        else
        {
            col.isTrigger = true;
        }
    }
    
    // Primary method - Particle collision detection (most reliable)
    void OnParticleCollision(GameObject other)
    {
        // Check if particles are from smoke/fire extinguisher
        if (other.name.Contains("smoke") || other.name.Contains("Smoke") || 
            other.CompareTag("Smoke") || other.name.Contains("FireEX"))
        {
            ApplyParticleSuppression(other);
        }
    }
    
    void ApplyParticleSuppression(GameObject smokeSource)
    {
        // Apply cooldown to prevent over-suppression from rapid particle hits
        if (Time.time - lastSuppressionTime < suppressionCooldown)
            return;
            
        lastSuppressionTime = Time.time;
        
        if (showDebugMessages)
            Debug.Log($"煙霧粒子開始滅火！從 {smokeSource.name}");
        
        // Try to suppress fire on this object or parent objects
        SuppressFireOnTarget(transform);
    }
    
    // Backup method - Trigger detection for continuous suppression
    void OnTriggerEnter(Collider other)
    {
        if (IsFireExtinguisherSmoke(other))
        {
            isActivelySuppressing = true;
            if (showDebugMessages)
                Debug.Log($"煙霧進入滅火區域！從 {other.gameObject.name}");
        }
    }
    
    void OnTriggerStay(Collider other)
    {
        if (enableContinuousSuppression && IsFireExtinguisherSmoke(other) && isActivelySuppressing)
        {
            // Apply continuous suppression while smoke is in the area
            SuppressFireContinuously();
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (IsFireExtinguisherSmoke(other))
        {
            isActivelySuppressing = false;
            if (showDebugMessages)
                Debug.Log("煙霧離開滅火區域，停止滅火");
        }
    }
    
    bool IsFireExtinguisherSmoke(Collider other)
    {
        return other.name.Contains("smoke") || other.name.Contains("Smoke") || 
               other.CompareTag("Smoke") || other.name.Contains("FireEX") ||
               other.GetComponent<ParticleSystem>() != null;
    }
    
    void SuppressFireContinuously()
    {
        // Apply suppression over time while in trigger zone
        float suppressionAmount = continuousSuppressionRate * Time.deltaTime;
        SuppressFireOnTarget(transform, suppressionAmount);
    }
    
    void SuppressFireOnTarget(Transform target, float customSuppressionAmount = -1f)
    {
        float actualSuppressionAmount = customSuppressionAmount > 0 ? customSuppressionAmount : suppressionRate;
        
        // Try ChemicalContainer first
        ChemicalContainer container = target.GetComponent<ChemicalContainer>();
        if (container != null && container.isOnFire)
        {
            // Use the new suppression system in ChemicalContainer
            container.ApplyExternalSuppression(actualSuppressionAmount);
            if (showDebugMessages)
                Debug.Log($"ChemicalContainer {target.name} 被煙霧撲滅！強度降低 {actualSuppressionAmount}");
            return;
        }
        
        // Try FlammableObject
        FlammableObject flammable = target.GetComponent<FlammableObject>();
        if (flammable != null && flammable.isOnFire)
        {
            flammable.ExtinguishFire();
            if (showDebugMessages)
                Debug.Log($"FlammableObject {target.name} 被煙霧撲滅！");
            return;
        }
        
        // Try FireExtinguisher (legacy support)
        FireExtinguisher fireScript = target.GetComponent<FireExtinguisher>();
        if (fireScript != null)
        {
            fireScript.ReduceFireIntensity(actualSuppressionAmount);
            if (showDebugMessages)
                Debug.Log($"FireExtinguisher {target.name} 強度降低 {actualSuppressionAmount}");
            return;
        }
        
        // Try parent objects
        if (target.parent != null)
        {
            SuppressFireOnTarget(target.parent, customSuppressionAmount);
        }
    }
    
    // Public method to manually trigger suppression (for testing)
    public void TestSuppression()
    {
        if (showDebugMessages)
            Debug.Log("測試滅火功能");
        SuppressFireOnTarget(transform);
    }
    
    // Visualize suppression area in Scene view
    void OnDrawGizmosSelected()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = isActivelySuppressing ? Color.cyan : Color.blue;
            if (col is SphereCollider sphere)
            {
                Gizmos.DrawWireSphere(transform.position, sphere.radius * transform.localScale.x);
            }
            else if (col is BoxCollider box)
            {
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
                Gizmos.DrawWireCube(Vector3.zero, box.size);
            }
        }
    }
}
