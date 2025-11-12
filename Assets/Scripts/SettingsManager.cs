using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("UI References")]
    public GameObject settingsPanel; // 👈 add this!
    public Slider audioSlider;
    public Slider sensitivitySlider;

    [Header("Audio")]
    public AudioMixer mainMixer; // optional

    public float MouseSensitivity { get; private set; } = 1f;

    private bool isOpen = false; // 👈 track whether it's open or closed

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        LoadSettings();

        if (audioSlider) audioSlider.onValueChanged.AddListener(SetVolume);
        if (sensitivitySlider) sensitivitySlider.onValueChanged.AddListener(SetSensitivity);

        // Start with the panel hidden
        if (settingsPanel) settingsPanel.SetActive(false);
    }

    // 🔊 Audio control
    public void SetVolume(float value)
    {
        if (mainMixer)
            mainMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
        else
            AudioListener.volume = value;

        PlayerPrefs.SetFloat("Volume", value);
    }

    // 🖱️ Sensitivity control
    public void SetSensitivity(float value)
    {
        MouseSensitivity = value;
        PlayerPrefs.SetFloat("Sensitivity", value);
    }

    void LoadSettings()
    {
        float vol = PlayerPrefs.GetFloat("Volume", 1f);
        float sens = PlayerPrefs.GetFloat("Sensitivity", 1f);

        if (audioSlider) audioSlider.value = vol;
        if (sensitivitySlider) sensitivitySlider.value = sens;

        SetVolume(vol);
        SetSensitivity(sens);
    }

    // 🧭 Panel Toggle Functions
    public void ToggleSettingsPanel()
    {
        if (settingsPanel == null) return;

        isOpen = !isOpen;
        settingsPanel.SetActive(isOpen);
    }

    public void CloseSettingsPanel()
    {
        if (settingsPanel == null) return;

        isOpen = false;
        settingsPanel.SetActive(false);
    }

    public void OpenSettingsPanel()
    {
        if (settingsPanel == null) return;

        isOpen = true;
        settingsPanel.SetActive(true);
    }
}
