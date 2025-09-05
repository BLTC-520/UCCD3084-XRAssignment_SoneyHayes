using UnityEngine;

public class FireZone : MonoBehaviour
{
    [Header("Fire Zone Settings")]
    public float damagePerSecond = 15f;
    public float ignitionDelay = 1f; // Delay before igniting objects that enter zone
    
    [HideInInspector]
    public ChemicalContainer parentContainer; // Reference to the container that created this zone
    
    private void Start()
    {
        Debug.Log($"Fire zone created at {transform.position}");
    }
    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Object entered fire zone: {other.name}");
        
        // Ignore the parent container that created this fire zone
        if (parentContainer && other.gameObject == parentContainer.gameObject)
            return;
        
        // Try to ignite flammable objects that enter the fire zone
        FlammableObject flammable = other.GetComponent<FlammableObject>();
        if (flammable && !flammable.isOnFire)
        {
            Debug.Log($"Flammable object {other.name} entered fire zone - igniting!");
            Invoke(nameof(IgniteFlammableObject), ignitionDelay);
            
            void IgniteFlammableObject()
            {
                if (flammable && !flammable.isOnFire)
                {
                    flammable.CatchFire();
                }
            }
        }
        
        // Try to ignite chemical containers that enter the fire zone
        ChemicalContainer container = other.GetComponent<ChemicalContainer>();
        if (container && !container.isOnFire && container != parentContainer)
        {
            Debug.Log($"Chemical container {other.name} entered fire zone - igniting!");
            Invoke(nameof(IgniteContainer), ignitionDelay);
            
            void IgniteContainer()
            {
                if (container && !container.isOnFire)
                {
                    container.StartFire();
                }
            }
        }
        
        // Handle player entering fire zone
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered fire zone - taking damage!");
            // You can add actual player damage system here
            // PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            // if (playerHealth) playerHealth.TakeDamage(damagePerSecond);
        }
    }
    
    void OnTriggerStay(Collider other)
    {
        // Continuous effects while objects stay in fire zone
        if (other.CompareTag("Player"))
        {
            // Apply continuous fire damage to player
            Debug.Log($"Player taking fire damage: {damagePerSecond * Time.deltaTime}");
            
            // Example damage application (uncomment if you have player health system):
            // PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            // if (playerHealth)
            // {
            //     playerHealth.TakeDamage(damagePerSecond * Time.deltaTime);
            // }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited fire zone - safe from fire damage");
        }
    }
    
    // Visualize the fire zone in Scene view
    void OnDrawGizmos()
    {
        SphereCollider sphereCol = GetComponent<SphereCollider>();
        if (sphereCol)
        {
            Gizmos.color = new Color(1f, 0.3f, 0f, 0.3f); // Semi-transparent red-orange
            Gizmos.DrawSphere(transform.position, sphereCol.radius);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, sphereCol.radius);
        }
    }
}