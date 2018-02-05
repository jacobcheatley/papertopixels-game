﻿using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Sources")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] private AudioClip[] hitSounds;
    [SerializeField] private AudioClip[] explodeSounds;
    [SerializeField] private AudioClip[] shootSounds;
    [SerializeField] private AudioClip[] lavaSounds;
    [SerializeField] private AudioClip[] dashSounds;
    // TODO: Lava hit, dash sounds

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
        PlayRandomClip(instance.explodeSounds);
    }

    public static void PlayShootSound()
    {
        PlayRandomClip(instance.shootSounds);
    }

    public static void PlayLavaSound()
    {
        PlayRandomClip(instance.lavaSounds, 0.05f);
    }

    public static void PlayDashSound()
    {
        PlayRandomClip(instance.dashSounds);
    }
}