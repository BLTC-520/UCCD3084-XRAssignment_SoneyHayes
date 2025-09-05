using UnityEngine;
using System.Collections;

public class FireAlarm2 : MonoBehaviour
{
    public AudioSource audioSource;
    public float delay = 20f;
    
    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
            
        StartCoroutine(PlayAudioAfterDelay());
    }
    
    IEnumerator PlayAudioAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}