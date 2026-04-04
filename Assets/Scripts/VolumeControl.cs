using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VolumeController : MonoBehaviour
{
    [Header("Music UI")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private TMP_Text musicText;

    [Header("SFX UI")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_Text sfxText;

    [Header("Music AudioSource")]
    [SerializeField] private AudioSource musicSource;

    private void Start()
    {
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float savedSFX   = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // Music slider
        musicSlider.minValue = 0f;
        musicSlider.maxValue = 1f;
        musicSlider.value = savedMusic;
        ApplyMusicVolume(savedMusic);
        musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);

        // SFX slider — set UI value but apply volume after all Awakes are done
        sfxSlider.minValue = 0f;
        sfxSlider.maxValue = 1f;
        sfxSlider.value = savedSFX;
        UpdateSFXLabel(savedSFX);
        sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
    }

    // Runs after all Start() methods — SoundEffectManager.Instance is guaranteed to exist
    private void LateUpdate()
    {
        // Only apply once on the first frame
        if (!_sfxInitialized)
        {
            _sfxInitialized = true;
            ApplySFXVolume(sfxSlider.value);
        }
    }
    private bool _sfxInitialized = false;

    private void OnMusicSliderChanged(float value) => ApplyMusicVolume(value);
    private void OnSFXSliderChanged(float value)   => ApplySFXVolume(value);

    private void ApplyMusicVolume(float value)
    {
        if (musicSource != null)
            musicSource.volume = value;

        if (musicText != null)
            musicText.text = "Music: " + Mathf.RoundToInt(value * 100) + "%";

        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    private void ApplySFXVolume(float value)
    {
        if (SoundEffectManager.Instance != null)
            SoundEffectManager.Instance.masterSFXVolume = value;

        UpdateSFXLabel(value);

        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }

    private void UpdateSFXLabel(float value)
    {
        if (sfxText != null)
            sfxText.text = "SFX: " + Mathf.RoundToInt(value * 100) + "%";
    }

    private void OnDestroy()
    {
        musicSlider.onValueChanged.RemoveListener(OnMusicSliderChanged);
        sfxSlider.onValueChanged.RemoveListener(OnSFXSliderChanged);
    }
}