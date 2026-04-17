using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] public AudioClip sfxFire;
    [SerializeField] public AudioClip sfxIce;
    [SerializeField] public AudioClip sfxFail;
    [SerializeField] public AudioClip sfxWin;

    private void Awake()
    {
        Instance = this;
    }

    public void PlaySfx(AudioClip clip, float volume = 1f)
    {
        sfxSource.PlayOneShot(clip, volume);
    }

    // Play music stop
    public void StopMusic()
    {
        // Permanently stop music
        musicSource.Stop();
    }
}
