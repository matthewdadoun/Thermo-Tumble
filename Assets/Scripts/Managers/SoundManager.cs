using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] public AudioClip sfxFire;
    [SerializeField] public AudioClip sfxIce;

    private void Awake()
    {
        Instance = this;
    }

    public void PlaySfx(AudioClip clip, float volume = 1f)
    {
        sfxSource.PlayOneShot(clip, volume);
    }
}
