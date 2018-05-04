using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Clips - Gameplay")]
    [SerializeField] private AudioClip[] hitSounds;
    [SerializeField] private AudioClip[] explodeSounds;
    [SerializeField] private AudioClip[] shootSounds;
    [SerializeField] private AudioClip[] lavaSounds;
    [SerializeField] private AudioClip[] dashSounds;
    [SerializeField] private AudioClip[] reloadSounds;
    [SerializeField] private AudioClip[] deathSounds;

    [Header("Clips - Menus")]
    [SerializeField] private AudioClip[] clickSounds;
    [SerializeField] private AudioClip[] positiveSounds;
    [SerializeField] private AudioClip[] negativeSounds;
    [SerializeField] private AudioClip[] joinSounds;
    [SerializeField] private AudioClip[] levelSelectSounds;
    [SerializeField] private AudioClip[] startSounds;

    private static SoundManager instance;

    void Start()
    {
        instance = this;
    }

    public static void PlayClip(AudioClip clip, float volumeScale = 1)
    {
        instance.sfxSource.PlayOneShot(clip, volumeScale);
    }

    public static void PlayRandomClip(AudioClip[] clips, float volumeScale = 1)
    {
        PlayClip(clips[Random.Range(0, clips.Length)], volumeScale);
    }

    public static void PlayHitSound()
    {
        PlayRandomClip(instance.hitSounds);
    }

    public static void PlayExplodeSound()
    {
        PlayRandomClip(instance.explodeSounds, 0.1f);
    }

    public static void PlayShootSound()
    {
        PlayRandomClip(instance.shootSounds, 0.1f);
    }

    public static void PlayLavaSound()
    {
        PlayRandomClip(instance.lavaSounds, 0.05f);
    }

    public static void PlayDashSound()
    {
        PlayRandomClip(instance.dashSounds);
    }

    public static void PlayReloadSound()
    {
        PlayRandomClip(instance.reloadSounds, 0.5f);
    }

    public static void PlayDeathSound()
    {
        PlayRandomClip(instance.deathSounds, 0.5f);
    }

    public static void PlayClickSound()
    {
        PlayRandomClip(instance.clickSounds, 0.5f);
    }

    public static void PlayPositiveSound()
    {
        PlayRandomClip(instance.positiveSounds, 0.5f);
    }

    public static void PlayNegativeSound()
    {
        PlayRandomClip(instance.negativeSounds, 0.5f);
    }

    public static void PlayJoinSound()
    {
        PlayRandomClip(instance.joinSounds, 0.5f);
    }

    public static void PlayLevelSelectSound()
    {
        PlayRandomClip(instance.levelSelectSounds, 0.5f);
    }

    public static void PlayStartSound()
    {
        PlayRandomClip(instance.startSounds, 0.5f);
    }
}
