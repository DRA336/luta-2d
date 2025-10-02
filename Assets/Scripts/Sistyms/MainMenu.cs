using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameSession session;

    void Awake()
    {
        if (!session) session = FindAnyObjectByType<GameSession>();
    }

    public void OnPlay()      => session?.GoCharSelect();
    public void OnSettings()  => session?.GoSettings();
    public void OnQuit()      { 
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
