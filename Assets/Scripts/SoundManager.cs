using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource sfxSource;

    private static SoundManager instance;

    void Start()
    {
        instance = this;
    }

    public static void PlayClip(AudioClip clip, float volumeScale = 1)
    {
        instance.sfxSource.PlayOneShot(clip, volumeScale);
    }
}
