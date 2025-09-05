using UnityEngine;

public class LiquidSpawner : MonoBehaviour
{
    public GameObject poolPrefab;   // assign prefab in inspector
    public Transform ground;        // assign ground plane
    private bool poolSpawned = false;

    void OnParticleCollision(GameObject other)
    {
        // Only spawn once
        if (!poolSpawned && other.CompareTag("Ground"))
        {
            // Spawn pool at collision point
            Vector3 spawnPos = other.transform.position;
            GameObject puddle = Instantiate(poolPrefab, spawnPos, Quaternion.identity);

            // Optional: parent to ground so it stays in place
            puddle.transform.SetParent(other.transform);

            poolSpawned = true;
        }
    }
}