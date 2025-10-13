using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public GameSession session;
    public Slider volumeSlider;
    public Toggle fullscreenToggle;
    public TMP_Text volumeLabel; // opcional

    void Awake() {
        if (!session) session = FindAnyObjectByType<GameSession>();
    }

    void OnEnable() {
        float vol = session ? session.masterVolume : PlayerPrefs.GetFloat("SET_VOL", 0.8f);
        bool fs   = session ? session.fullscreen   : PlayerPrefs.GetInt("SET_FS", 1) == 1;

        if (volumeSlider)    volumeSlider.SetValueWithoutNotify(vol);
        if (fullscreenToggle) fullscreenToggle.SetIsOnWithoutNotify(fs);
        if (volumeLabel)     volumeLabel.text = Mathf.RoundToInt(vol * 100f) + "%";
    }

    public void OnVolumeChanged(float v) {
        v = Mathf.Clamp01(v);
        if (session) { session.masterVolume = v; session.ApplyOptions(); }
        else {
            AudioListener.volume = v;
            PlayerPrefs.SetFloat("SET_VOL", v); PlayerPrefs.Save();
        }
        if (volumeLabel) volumeLabel.text = Mathf.RoundToInt(v * 100f) + "%";
    }

    public void OnFullscreenChanged(bool f) {
        if (session) { session.fullscreen = f; session.ApplyOptions(); }
        else {
            Screen.fullScreen = f;
            PlayerPrefs.SetInt("SET_FS", f ? 1 : 0); PlayerPrefs.Save();
        }
    }

    public void OnBack() => session?.GoMainMenu();
}
