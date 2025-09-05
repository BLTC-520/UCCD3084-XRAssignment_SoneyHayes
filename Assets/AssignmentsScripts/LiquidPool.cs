using UnityEngine;

public class LiquidPool : MonoBehaviour
{
    [SerializeField] private float maxSize = 5f;
    [SerializeField] private float growSpeed = 2f;
    [SerializeField] private Renderer poolRenderer; // assign in inspector
    [SerializeField] private GameObject fireSystem; // reference existing fire

    private float currentSize = 0f;
    private bool isOnFire = false;
    private Material poolMat;
    private float glowIntensity = 0f;

    void Start()
    {
        transform.localScale = Vector3.zero;
        poolMat = poolRenderer.material;
        poolMat.DisableKeyword("_EMISSION");

        if (fireSystem != null)
            fireSystem.gameObject.SetActive(false); // start hidden
    }

    void Update()
    {
        if (currentSize < maxSize)
        {
            currentSize += growSpeed * Time.deltaTime;
            transform.localScale = Vector3.one * currentSize;
        }

        if (isOnFire && glowIntensity < 5f)
        {
            glowIntensity += Time.deltaTime * 2f;
            poolMat.EnableKeyword("_EMISSION");
            poolMat.SetColor("_EmissionColor", Color.Lerp(Color.black, Color.red, glowIntensity / 5f));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isOnFire && other.CompareTag("Fire"))
        {
            Ignite();
        }
    }

    private void Ignite()
    {
        isOnFire = true;

        if (fireSystem != null)
            fireSystem.gameObject.SetActive(true); // turn on existing fire
    }
}