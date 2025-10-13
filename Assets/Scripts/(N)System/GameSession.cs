using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    public bool p2IsHuman = false; // false = CPU, true = Player 2

    public void ToggleP2Human() { p2IsHuman = !p2IsHuman; }
    public void SetP2Human(bool human) { p2IsHuman = human; }

    public static GameSession I { get; private set; }

    [Header("Cenas")]
    public string startScene = "StartScreen";
    public string mainMenuScene = "MainMenu";
    public string settingsScene = "Settings";
    public string charSelectScene = "CharacterSelect";
    public string stageSelectScene = "StageSelect";
    public string fightScene = "Fight";

    [Header("Dados de Seleção (simples)")]
    public Sprite[] characterPortraits;  // arraste imagens dos personagens
    public string[] characterNames;      // nomes correspondentes
    public Sprite[] stageThumbs;         // thumbs de mapas
    public string[] stageNames;

    [Header("Escolhas atuais")]
    public int p1CharIndex = 0;
    public int p2CharIndex = 0; // pode usar como “Bot”
    public int stageIndex = 0;

    [Header("Opções")]
    [Range(0f,1f)] public float masterVolume = 1f;
    public bool fullscreen = true;
    public AudioMixer mixer;            // (opcional) arraste aqui
    public string mixerParam = "MasterVolume"; // nome do parâmetro exposto no Mixer


    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        masterVolume = PlayerPrefs.GetFloat("SET_VOL", 0.8f);
        fullscreen   = PlayerPrefs.GetInt("SET_FS", 1) == 1;
        ApplyOptions();
    }

    public void SetP1(int index) => p1CharIndex = Mathf.Clamp(index, 0, Mathf.Max(0, characterNames.Length-1));
    public void SetP2(int index) => p2CharIndex = Mathf.Clamp(index, 0, Mathf.Max(0, characterNames.Length-1));
    public void SetStage(int index) => stageIndex = Mathf.Clamp(index, 0, Mathf.Max(0, stageNames.Length - 1));



    public void ApplyOptions()
    {
         // Volume
    if (mixer) {
        float dB = (masterVolume > 0.0001f) ? Mathf.Log10(masterVolume) * 20f : -80f;
        mixer.SetFloat(mixerParam, dB);
    } else {
        AudioListener.volume = Mathf.Clamp01(masterVolume);
    }

    // Tela cheia
    Screen.fullScreen = fullscreen;

    // Persistência
    PlayerPrefs.SetFloat("SET_VOL", Mathf.Clamp01(masterVolume));
    PlayerPrefs.SetInt("SET_FS", fullscreen ? 1 : 0);
        PlayerPrefs.Save();
     
     AudioListener.volume = masterVolume;
        Screen.fullScreen = fullscreen;
    }
       
    

    // Navegação utilitária
    public void Go(string sceneName) => SceneManager.LoadScene(sceneName);
    public void GoMainMenu() => Go(mainMenuScene);
    public void GoSettings() => Go(settingsScene);
    public void GoCharSelect() => Go(charSelectScene);
    public void GoStageSelect() => Go(stageSelectScene);
    public void GoFight() => Go(fightScene);
}
