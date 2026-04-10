using UnityEngine;
using TMPro;
 
public class GraphicsSettingsController : MonoBehaviour
{
    [Header("FPS Dropdown")]
    [SerializeField] private TMP_Dropdown fpsDropdown;
 
    [Header("Resolution Dropdown")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
 
    // Matches dropdown order: 0 = 30, 1 = 60, 2 = 120
    private readonly int[] fpsOptions = { 30, 60, 120 };
 
    // Matches dropdown order: 0 = 720p, 1 = 1080p, 2 = 1440p
    private readonly (int width, int height)[] resolutionOptions =
    {
        (1280, 720),
        (1920, 1080),
        (2560, 1440)
    };
 
    private void Start()
    {
        SetupFPSDropdown();
        SetupResolutionDropdown();
    }
 
    // -------------------------
    //          FPS
    // -------------------------
 
    private void SetupFPSDropdown()
    {
        int savedFPS = PlayerPrefs.GetInt("TargetFPS", 60);
 
        int savedIndex = 1; // default to 60fps
        for (int i = 0; i < fpsOptions.Length; i++)
        {
            if (fpsOptions[i] == savedFPS)
            {
                savedIndex = i;
                break;
            }
        }
 
        fpsDropdown.value = savedIndex;
        ApplyFPS(savedIndex);
 
        fpsDropdown.onValueChanged.AddListener(OnFPSChanged);
    }
 
    private void OnFPSChanged(int index) => ApplyFPS(index);
 
    private void ApplyFPS(int index)
    {
        int fps = fpsOptions[index];
        Application.targetFrameRate = fps;
 
        PlayerPrefs.SetInt("TargetFPS", fps);
        PlayerPrefs.Save();
 
        Debug.Log("Target FPS set to: " + fps);
    }
 
    // -------------------------
    //       RESOLUTION
    // -------------------------
 
    private void SetupResolutionDropdown()
    {
        if (resolutionDropdown == null) return;
 
        resolutionDropdown.interactable = true;
 
        // Populate dropdown with resolution labels
        resolutionDropdown.ClearOptions();
        var options = new System.Collections.Generic.List<string>
        {
            "1280 x 720 (HD)",
            "1920 x 1080 (Full HD)",
            "2560 x 1440 (2K)"
        };
        resolutionDropdown.AddOptions(options);
 
        // Load saved resolution index (default to 1080p)
        int savedIndex = PlayerPrefs.GetInt("ResolutionIndex", 1);
        resolutionDropdown.value = savedIndex;
        ApplyResolution(savedIndex);
 
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
    }
 
    private void OnResolutionChanged(int index) => ApplyResolution(index);
 
    private void ApplyResolution(int index)
    {
        var res = resolutionOptions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);
 
        PlayerPrefs.SetInt("ResolutionIndex", index);
        PlayerPrefs.Save();
 
        Debug.Log($"Resolution set to: {res.width}x{res.height}");
    }
 
    private void OnDestroy()
    {
        fpsDropdown.onValueChanged.RemoveListener(OnFPSChanged);
 
        if (resolutionDropdown != null)
            resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);
    }
} 