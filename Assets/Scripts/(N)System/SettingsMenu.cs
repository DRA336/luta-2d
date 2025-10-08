using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public GameSession session;
    public Slider volumeSlider;
    public Toggle fullscreenToggle;

    void Awake()
    {
        if (!session) session = FindAnyObjectByType<GameSession>();
    }

    void OnEnable()
    {
        if (session)
        {
            if (volumeSlider)      volumeSlider.value = session.masterVolume;
            if (fullscreenToggle)  fullscreenToggle.isOn = session.fullscreen;
        }
    }

    public void OnVolumeChanged(float v)
    {
        if (!session) return;
        session.masterVolume = v;
        session.ApplyOptions();
    }

    public void OnFullscreenChanged(bool f)
    {
        if (!session) return;
        session.fullscreen = f;
        session.ApplyOptions();
    }

    public void OnBack() => session?.GoMainMenu();
}
