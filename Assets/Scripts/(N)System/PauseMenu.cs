using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Atalho")]
    public KeyCode toggleKey = KeyCode.P;

    [Header("UI")]
    public GameObject panel;          // painel raiz do pause
    public Button defaultButton;      // botão focado ao abrir (ex.: Resume)

    [Header("Cenas (opcional)")]
    public string mainMenuScene = "MainMenu"; // para Quit
    public string fightSceneOverride = "";    // se quiser forçar o nome da cena de luta

    [Header("Refs (opcional)")]
    public MatchController match;     // se tiver, ele já tem um Pause(bool)

    bool isPaused = false;

    void Awake()
    {
        if (!match) match = FindAnyObjectByType<MatchController>();
        if (panel) panel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            TogglePause();
    }

    public void TogglePause()
    {
        SetPaused(!isPaused);
    }

    public void SetPaused(bool pause)
    {
        if (isPaused == pause) return;
        isPaused = pause;

        // pausa via MatchController se existir (mantém consistência)
        if (match)
            match.Pause(pause);
        else
            Time.timeScale = pause ? 0f : 1f;

        if (panel) panel.SetActive(pause);

        if (pause && defaultButton)
        {
            // foca um botão pra navegação por teclado/controle
            EventSystem.current?.SetSelectedGameObject(null);
            EventSystem.current?.SetSelectedGameObject(defaultButton.gameObject);
        }
        else
        {
            // limpa seleção
            if (EventSystem.current?.currentSelectedGameObject)
                EventSystem.current.SetSelectedGameObject(null);
        }
    }

    // ==== Botões ====

    public void OnResume()
    {
        SetPaused(false);
    }

    public void OnRestart()
    {
        // volta o tempo ANTES de trocar cena
        Time.timeScale = 1f;

        // decide qual cena recarregar
        string fightScene = !string.IsNullOrEmpty(fightSceneOverride)
            ? fightSceneOverride
            : SceneManager.GetActiveScene().name; // recarrega a atual

        SceneManager.LoadScene(fightScene);
    }

    public void OnQuitToMenu()
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(mainMenuScene))
            SceneManager.LoadScene(mainMenuScene);
        else
            Debug.LogWarning("[PauseMenu] mainMenuScene não definido.");
    }
}
