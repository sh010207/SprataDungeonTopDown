using System.Collections;
using UnityEngine;
public class SoundSource : MonoBehaviour
{
    private AudioSource audioSource;

    public void Play(AudioClip clip, float soundEffectVolume, float soundEffectPitch)
    {
        if(audioSource == null)
            audioSource = GetComponent<AudioSource>();  
        
        CancelInvoke();
        audioSource.clip = clip;
        audioSource.volume = soundEffectVolume;
        audioSource.Play();
        audioSource.pitch = 1f + Random.Range(-soundEffectPitch, soundEffectPitch);
        
        Invoke("Disable",clip.length + 2);
    }
    private void Disable()
    {
        audioSource.Stop();
        gameObject.SetActive(false);
    }
}
