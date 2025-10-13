using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsMixerSnapshots : MonoBehaviour
{
    [Header("Mixer + Snapshots")]
    public AudioMixer mixer;
    public AudioMixerSnapshot volMute;   // snapshot com volumes em -80 dB (ou o que você definiu)
    public AudioMixerSnapshot volFull;   // snapshot com volumes em 0 dB (ou seu “padrão”)

    [Header("UI")]
    public Slider volumeSlider;          // 0..1
    public TMP_Text volumeLabel;         // opcional (exibe 78%)

    [Header("Transição")]
    public float fadeSeconds = 0.05f;    // suavização da troca

    const string VOL_KEY = "SET_VOL_SNAP";

    void Awake()
    {
        if (!mixer || !volMute || !volFull)
            Debug.LogWarning("[SettingsMixerSnapshots] Configure o Mixer e as Snapshots no Inspector.");
    }

    void OnEnable()
    {
        // restaura preferências
        float v = PlayerPrefs.GetFloat(VOL_KEY, 0.8f);
        if (volumeSlider) volumeSlider.SetValueWithoutNotify(v);
        if (volumeLabel)  volumeLabel.text = Mathf.RoundToInt(v * 100f) + "%";
        ApplyVolume(v, 0f); // aplica sem fade ao abrir
    }

    // ligar no OnValueChanged do Slider
    public void OnVolumeChanged(float v01)
    {
        v01 = Mathf.Clamp01(v01);
        if (volumeLabel) volumeLabel.text = Mathf.RoundToInt(v01 * 100f) + "%";
        ApplyVolume(v01, fadeSeconds);
        PlayerPrefs.SetFloat(VOL_KEY, v01);
        PlayerPrefs.Save();
    }

    void ApplyVolume(float v01, float seconds)
    {
        if (!mixer || !volMute || !volFull) return;

        // dois pesos que somam 1: (mute ↔ full)
        var snapshots = new[] { volMute, volFull };
        var weights   = new[] { 1f - v01, v01 };

        mixer.TransitionToSnapshots(snapshots, weights, seconds);
    }
}
