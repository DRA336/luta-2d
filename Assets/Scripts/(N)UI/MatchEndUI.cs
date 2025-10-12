using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MatchEndUI : MonoBehaviour
{
    [Header("Refs do Painel")]
    public GameObject root;       // painel/popup (GameObject) — deixe DESLIGADO na cena
    public TMP_Text titleText;    // "K.O." ou "Tempo esgotado"
    public TMP_Text subtitleText; // "Fulano vence!" (opcional)

    [Header("Sessão (opcional)")]
    public GameSession session;   // se não arrastar, ele encontra sozinho

    void Awake()
    {
        if (!session) session = FindAnyObjectByType<GameSession>();
        HideImmediate();
    }

    void OnDisable()
    {
        // segurança: se alguém destruir a UI, voltamos o timeScale
        Time.timeScale = 1f;
    }

    // -------- API pública: chame isso quando a luta acabar --------
    public void ShowKO(string winnerName)
    {
        Show("K.O.", string.IsNullOrEmpty(winnerName) ? "" : $"{winnerName} vence!");
    }

    public void ShowTimeUp(string winnerName = "")
    {
        Show("TEMPO ESGOTADO", string.IsNullOrEmpty(winnerName) ? "" : $"{winnerName} vence!");
    }

    // ---------------------------------------------------------------
    void Show(string title, string subtitle)
    {
        if (titleText)    titleText.text    = title;
        if (subtitleText) subtitleText.text = subtitle;

        if (root) root.SetActive(true);
        Time.timeScale = 0f; // PAUSA o jogo
    }

    public void HideImmediate()
    {
        if (root) root.SetActive(false);
        Time.timeScale = 1f;
    }

    // --------- Botões ----------
    public void OnRematch()
    {
        Time.timeScale = 1f;
        if (session) session.GoFight();
        else SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnChangeCharacter()
    {
        Time.timeScale = 1f;
        session?.GoCharSelect();
    }

    public void OnChangeStage()
    {
        Time.timeScale = 1f;
        session?.GoStageSelect();
    }

    public void OnMainMenu()
    {
        Time.timeScale = 1f;
        if (session) session.GoMainMenu();
        else SceneManager.LoadScene("MainMenu");
    }
}
