using UnityEngine;
using System.Collections;

public class FireSpreadOnMesh : MonoBehaviour
{
    [SerializeField] private ParticleSystem firePrefab;
    [SerializeField] private int fireCount = 5; // how many spread points
    [SerializeField] private float spreadDelay = 0.5f;

    private bool isBurning = false;
    private Vector3[] debugPositions; // store positions for gizmos

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Collider detected: {other.name}, Tag: {other.tag}");
        if (!isBurning && other.CompareTag("Fire"))
        {
            Debug.Log("Fire Start Spreading");
            StartCoroutine(SpreadFire());
        }
    }

    private IEnumerator SpreadFire()
    {
        isBurning = true;
        Bounds meshBounds = GetComponent<MeshRenderer>().bounds;
        debugPositions = new Vector3[fireCount];

        for (int i = 0; i < fireCount; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(meshBounds.min.x, meshBounds.max.x),
                Random.Range(meshBounds.min.y, meshBounds.max.y), // full surface, not just bottom
                Random.Range(meshBounds.min.z, meshBounds.max.z)
            );

            debugPositions[i] = randomPos;
            Debug.Log("Fire spawn at" + randomPos);

            Instantiate(firePrefab, randomPos, Quaternion.identity, transform);
            yield return new WaitForSeconds(spreadDelay);
        }
    }

    // Draw debug spheres in the scene view
    private void OnDrawGizmos()
    {
        if (debugPositions == null) return;

        Gizmos.color = Color.red;
        foreach (var pos in debugPositions)
        {
            Gizmos.DrawSphere(pos, 0.2f);
        }
    }
}