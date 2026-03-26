using UnityEngine;

public class SoundEffectManager : MonoBehaviour
{
    public static SoundEffectManager Instance;

    [Header("Sound Effects")]
    public AudioClip gunShotSound;
    public AudioClip potionPickupSound;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float masterSFXVolume = 1f;
    [Range(0f, 1f)] public float gunShotVolume = 0.3f;
    [Range(0f, 1f)] public float potionPickupVolume = 0.6f;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlayGunShot()
    {
        if (gunShotSound != null)
        {
            audioSource.PlayOneShot(gunShotSound, gunShotVolume * masterSFXVolume);
        }
    }

    public void PlayPotionPickup()
    {
        if (potionPickupSound != null)
        {
            audioSource.PlayOneShot(potionPickupSound, potionPickupVolume * masterSFXVolume);
        }
    }
}