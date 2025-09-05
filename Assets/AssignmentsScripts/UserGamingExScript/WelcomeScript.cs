using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FactoryWelcomeMessage : MonoBehaviour
{
    [Header("UI Elements")]
    public Canvas welcomeUI;
    public Text welcomeText; // or TextMeshPro if you're using that
    public AudioSource welcomeAudio;
    
    [Header("Settings")]
    public float displayDuration = 5f;
    public bool fadeInOut = true;
    
    void Start()
    {
        // Small delay to ensure VR is fully loaded
        StartCoroutine(ShowWelcomeAfterDelay(1f));
    }
    
    IEnumerator ShowWelcomeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowWelcomeMessage();
    }
    
    void ShowWelcomeMessage()
    {
        welcomeUI.gameObject.SetActive(true);
        
        if (welcomeAudio != null)
            welcomeAudio.Play();
        
        if (fadeInOut)
        {
            StartCoroutine(FadeInOutMessage());
        }
        else
        {
            StartCoroutine(HideMessageAfterDelay(displayDuration));
        }
    }
    
    IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        welcomeUI.gameObject.SetActive(false);
    }
    
    IEnumerator FadeInOutMessage()
    {
        CanvasGroup canvasGroup = welcomeUI.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = welcomeUI.gameObject.AddComponent<CanvasGroup>();
        
        // Fade in
        float fadeTime = 1f;
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            canvasGroup.alpha = t / fadeTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
        
        // Stay visible
        yield return new WaitForSeconds(displayDuration - 2f);
        
        // Fade out
        for (float t = fadeTime; t > 0; t -= Time.deltaTime)
        {
            canvasGroup.alpha = t / fadeTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
        welcomeUI.gameObject.SetActive(false);
    }
}