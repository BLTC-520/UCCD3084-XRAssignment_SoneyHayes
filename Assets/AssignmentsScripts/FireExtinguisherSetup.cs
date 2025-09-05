using UnityEngine;

[System.Serializable]
public class FireExtinguisherSetup : MonoBehaviour
{
    [Header("Setup Instructions")]
    [TextArea(10, 15)]
    public string instructions = @"FIRE EXTINGUISHER SETUP INSTRUCTIONS:

1. PARTICLE SYSTEM COLLISION SETUP:
   - Select the FireEX_PS prefab in your scene
   - In the Particle System component, expand the 'Collision' section
   - Check 'Enabled' to turn on collision
   - Set Type to 'World'
   - Set Mode to 'Collision'  
   - Check 'Send Collision Messages' (IMPORTANT!)
   - Set Collision Quality to 'High' for better detection
   - Set Max Collision Shapes to 256
   - Check 'Enable Dynamic Colliders' if needed

2. FIRE OBJECT SETUP:
   - Add FireExtinguisher.cs script to ALL fire objects:
     * Flammable objects (with FlammableObject.cs)
     * Chemical containers (with ChemicalContainer.cs)  
     * Fire zones (with FireZone.cs)
   
3. SPRAY OBJECT TAG:
   - Set the spray particle system's GameObject tag to 'Smoke'
   - Or ensure the GameObject name contains 'FireEX_PS'

4. TESTING:
   - Play the scene
   - Use the VR controller trigger to spray
   - Spray should gradually reduce fire size
   - Fire should extinguish when intensity drops to 0";

    [Header("Auto Setup (Run in Play Mode)")]
    public bool autoConfigureSprayParticles = true;
    public bool autoAddFireExtinguisherScripts = true;
    
    [Header("References")]
    public ParticleSystem sprayParticleSystem;
    public GameObject[] fireObjects;
    
    void Start()
    {
        if (autoConfigureSprayParticles)
        {
            ConfigureSprayParticles();
        }
        
        if (autoAddFireExtinguisherScripts)
        {
            AddFireExtinguisherScripts();
        }
    }
    
    void ConfigureSprayParticles()
    {
        // Find spray particle system if not assigned
        if (!sprayParticleSystem)
        {
            sprayParticleSystem = GameObject.Find("FireEX_PS")?.GetComponent<ParticleSystem>();
            if (!sprayParticleSystem)
            {
                sprayParticleSystem = FindObjectOfType<SmokeVR>()?.smoke;
            }
        }
        
        if (sprayParticleSystem)
        {
            Debug.Log($"Configuring particle collision for {sprayParticleSystem.name}");
            
            // Configure collision settings
            var collision = sprayParticleSystem.collision;
            collision.enabled = true;
            collision.type = ParticleSystemCollisionType.World;
            collision.mode = ParticleSystemCollisionMode.Collision3D;
            collision.sendCollisionMessages = true; // This is crucial!
            collision.collidesWith = -1; // Collide with everything
            collision.maxCollisionShapes = 256;
            collision.quality = ParticleSystemCollisionQuality.High;
            collision.enableDynamicColliders = true;
            
            // Ensure the object has the right tag
            if (!sprayParticleSystem.gameObject.CompareTag("Smoke"))
            {
                if (sprayParticleSystem.gameObject.tag == "Untagged")
                {
                    sprayParticleSystem.gameObject.tag = "Smoke";
                    Debug.Log("Set spray particle system tag to 'Smoke'");
                }
            }
            
            Debug.Log("Spray particle collision configured successfully!");
        }
        else
        {
            Debug.LogWarning("Could not find spray particle system to configure!");
        }
    }
    
    void AddFireExtinguisherScripts()
    {
        int scriptsAdded = 0;
        
        // Auto-find fire objects if not manually assigned
        if (fireObjects == null || fireObjects.Length == 0)
        {
            // Find all flammable objects
            FlammableObject[] flammableObjects = FindObjectsOfType<FlammableObject>();
            ChemicalContainer[] chemicalContainers = FindObjectsOfType<ChemicalContainer>();
            FireZone[] fireZones = FindObjectsOfType<FireZone>();
            
            int totalObjects = flammableObjects.Length + chemicalContainers.Length + fireZones.Length;
            fireObjects = new GameObject[totalObjects];
            
            int index = 0;
            foreach (var obj in flammableObjects)
            {
                fireObjects[index++] = obj.gameObject;
            }
            foreach (var obj in chemicalContainers)
            {
                fireObjects[index++] = obj.gameObject;
            }
            foreach (var obj in fireZones)
            {
                fireObjects[index++] = obj.gameObject;
            }
        }
        
        // Add FireExtinguisher scripts to fire objects
        foreach (GameObject fireObj in fireObjects)
        {
            if (fireObj && !fireObj.GetComponent<FireExtinguisher>())
            {
                fireObj.AddComponent<FireExtinguisher>();
                scriptsAdded++;
                Debug.Log($"Added FireExtinguisher script to {fireObj.name}");
            }
        }
        
        if (scriptsAdded > 0)
        {
            Debug.Log($"Successfully added FireExtinguisher scripts to {scriptsAdded} fire objects!");
        }
        else
        {
            Debug.Log("All fire objects already have FireExtinguisher scripts.");
        }
    }
    
    [ContextMenu("Manual Setup")]
    void ManualSetup()
    {
        ConfigureSprayParticles();
        AddFireExtinguisherScripts();
    }
    
    void OnDrawGizmos()
    {
        // Draw setup status indicators
        if (sprayParticleSystem)
        {
            Gizmos.color = sprayParticleSystem.collision.enabled ? Color.green : Color.red;
            Gizmos.DrawWireCube(sprayParticleSystem.transform.position, Vector3.one * 0.5f);
        }
        
        // Draw fire objects
        if (fireObjects != null)
        {
            foreach (GameObject fireObj in fireObjects)
            {
                if (fireObj)
                {
                    bool hasExtinguisher = fireObj.GetComponent<FireExtinguisher>() != null;
                    Gizmos.color = hasExtinguisher ? Color.blue : Color.yellow;
                    Gizmos.DrawWireSphere(fireObj.transform.position, 0.3f);
                }
            }
        }
    }
}