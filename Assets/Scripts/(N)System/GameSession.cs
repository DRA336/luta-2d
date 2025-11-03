using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    // Singleton
    public static GameSession I { get; private set; }

    [Header("Modo P2")]
    public bool p2IsHuman = false; // false = CPU, true = Player 2
    public void ToggleP2Human() => p2IsHuman = !p2IsHuman;
    public void SetP2Human(bool human) => p2IsHuman = human;

    [Header("Cenas")]
    public string startScene = "StartScreen";
    public string mainMenuScene = "MainMenu";
    public string settingsScene = "Settings";
    public string charSelectScene = "CharacterSelect";
    public string stageSelectScene = "StageSelect";
    public string fightScene = "Fight";

    [Header("Dados de Seleção (simples)")]
    public Sprite[] characterPortraits;
    public string[] characterNames;
    public Sprite[] stageThumbs;
    public string[] stageNames;

    [Header("Escolhas atuais")]
    public int p1CharIndex = 0;
    public int p2CharIndex = 0; // pode usar como “Bot”
    public int stageIndex = 0;

    [Header("Opções")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    public bool fullscreen = true;
    public AudioMixer mixer;                 // (opcional)
    public string mixerParam = "MasterVolume";

    [Header("Lista de personagens (Specs)")]
    public CharacterSpec[] characterSpecs;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        // Restaurar prefs
        masterVolume = PlayerPrefs.GetFloat("SET_VOL", 0.8f);
        fullscreen   = PlayerPrefs.GetInt("SET_FS", 1) == 1;
        ApplyOptions();
    }

    // ---- Getters de Spec (UMA ÚNICA VEZ) ----
    public CharacterSpec GetP1Spec()
    {
        if (characterSpecs == null || characterSpecs.Length == 0) return null;
        int i = Mathf.Clamp(p1CharIndex, 0, characterSpecs.Length - 1);
        return characterSpecs[i];
    }

    public CharacterSpec GetP2Spec()
    {
        if (characterSpecs == null || characterSpecs.Length == 0) return null;
        int i = Mathf.Clamp(p2CharIndex, 0, characterSpecs.Length - 1);
        return characterSpecs[i];
    }

    // ---- Setters de índice (usando o que existir) ----
    public void SetP1(int index)
    {
        int max = characterSpecs != null && characterSpecs.Length > 0
                  ? characterSpecs.Length - 1
                  : (characterNames != null ? characterNames.Length - 1 : 0);
        p1CharIndex = Mathf.Clamp(index, 0, Mathf.Max(0, max));
    }

    public void SetP2(int index)
    {
        int max = characterSpecs != null && characterSpecs.Length > 0
                  ? characterSpecs.Length - 1
                  : (characterNames != null ? characterNames.Length - 1 : 0);
        p2CharIndex = Mathf.Clamp(index, 0, Mathf.Max(0, max));
    }

    public void SetStage(int index)
    {
        int max = stageNames != null ? stageNames.Length - 1 : 0;
        stageIndex = Mathf.Clamp(index, 0, Mathf.Max(0, max));
    }

    // ---- Opções ----
    public void ApplyOptions()
    {
        // Volume
        if (mixer)
        {
            float dB = (masterVolume > 0.0001f) ? Mathf.Log10(masterVolume) * 20f : -80f;
            mixer.SetFloat(mixerParam, dB);
        }
        else
        {
            AudioListener.volume = Mathf.Clamp01(masterVolume);
        }

        // Tela cheia
        Screen.fullScreen = fullscreen;

        // Persistência
        PlayerPrefs.SetFloat("SET_VOL", Mathf.Clamp01(masterVolume));
        PlayerPrefs.SetInt("SET_FS", fullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    // ---- Navegação ----
    public void Go(string sceneName) => SceneManager.LoadScene(sceneName);
    public void GoMainMenu()   => Go(mainMenuScene);
    public void GoSettings()   => Go(settingsScene);
    public void GoCharSelect() => Go(charSelectScene);
    public void GoStageSelect()=> Go(stageSelectScene);
    public void GoFight()      => Go(fightScene);
}
