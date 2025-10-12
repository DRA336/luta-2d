using System.Collections;
using UnityEngine;

public enum MatchState { Idle, PreRound, Fighting, KO, TimeOver, RoundEnd, MatchEnd }

public class MatchController : MonoBehaviour
{
    [Header("Jogadores")]
    public PlayerHits player1;
    public PlayerHits player2;

    [Header("Regras")]
    public int roundTimeSeconds = 60;   // tempo por round
    public int winsToWinMatch = 2;      // melhor de 3 (2), melhor de 5 (3) etc.

    [Header("Nomes (opcional)")]
    public string player1Name = "Player 1";
    public string player2Name = "CPU";

    [Header("UI (opcional)")]
    public MatchUI ui;                  // mostra timer/placar/anúncios (se tiver)
    public MatchEndUI endUI;            // PAINEL de encerramento (arraste aqui)

    [Header("Pausa ao encerrar")]
    public bool pauseOnMatchEnd = true;
    public float pauseDelay = 1.0f;

    // Estado interno
    public MatchState State { get; private set; } = MatchState.Idle;
    private int p1Wins = 0, p2Wins = 0;
    private float timeLeft = 0f;
    private bool matchEndedOnce = false;

    void Awake()
    {
        if (!endUI) endUI = FindAnyObjectByType<MatchEndUI>();
        var gs = GameSession.I ? GameSession.I : FindAnyObjectByType<GameSession>();
        if (gs) player2Name = gs.p2IsHuman ? "Player 2" : "CPU";
    }

    void OnEnable()
    {
        if (player1) player1.OnKO += OnPlayerKO;
        if (player2) player2.OnKO += OnPlayerKO;
    }

    void OnDisable()
    {
        if (player1) player1.OnKO -= OnPlayerKO;
        if (player2) player2.OnKO -= OnPlayerKO;
        // segurança: sempre que sair, volta o timeScale
        Time.timeScale = 1f;
    }

    void Start()
    {
        StartCoroutine(RunMatch());
    }

    IEnumerator RunMatch()
    {
        State = MatchState.PreRound;

        while (p1Wins < winsToWinMatch && p2Wins < winsToWinMatch)
        {
            // ===== PRE-ROUND =====
            if (player1) player1.ResetForRound();
            if (player2) player2.ResetForRound();

            timeLeft = roundTimeSeconds;
            ui?.ShowTimer(timeLeft);
            ui?.ShowRoundIntro($"Round {p1Wins + p2Wins + 1}");

            yield return new WaitForSeconds(1.0f);
            ui?.ShowAnnounce("Fight!");
            yield return new WaitForSeconds(0.6f);
            ui?.HideAnnounce();

            State = MatchState.Fighting;

            // ===== LOOP DE LUTA =====
            while (State == MatchState.Fighting)
            {
                timeLeft = Mathf.Max(0f, timeLeft - Time.deltaTime);
                ui?.ShowTimer(timeLeft);

                if (timeLeft <= 0f)
                {
                    State = MatchState.TimeOver;
                    break;
                }
                yield return null;
            }

            // ===== RESOLUÇÃO DO ROUND =====
            if (State == MatchState.TimeOver)
            {
                ui?.ShowAnnounce("Time Over!");
                yield return new WaitForSeconds(0.8f);

                int winner = DecideByLife(); // 1, 2 ou 0 (empate)
                if (winner == 1)
                {
                    p1Wins++;
                    ui?.ShowAnnounce($"{player1Name} Wins");
                }
                else if (winner == 2)
                {
                    p2Wins++;
                    ui?.ShowAnnounce($"{player2Name} Wins");
                }
                else
                {
                    ui?.ShowAnnounce("Draw");
                }

                ui?.UpdateScore(p1Wins, p2Wins);
                yield return new WaitForSeconds(1.0f);
                ui?.HideAnnounce();
            }
            else if (State == MatchState.KO)
            {
                // winner setado em OnPlayerKO
                ui?.UpdateScore(p1Wins, p2Wins);
                yield return new WaitForSeconds(1.0f);
                ui?.HideAnnounce();
            }

            State = MatchState.RoundEnd;
            yield return new WaitForSeconds(0.5f);
            State = MatchState.PreRound;
        }

        // ===== MATCH END =====
        State = MatchState.MatchEnd;
        if (matchEndedOnce) yield break; // evita disparo duplo
        matchEndedOnce = true;

        string finalMsg = (p1Wins > p2Wins)
            ? $"{player1Name} WINS THE MATCH!"
            : $"{player2Name} WINS THE MATCH!";

        ui?.ShowAnnounce(finalMsg);

        // Mostra o PAINEL de encerramento (com botões)
        if (endUI)
        {
            // Decide o texto principal do painel
            endUI.ShowKO(p1Wins > p2Wins ? player1Name : player2Name);
        }

        if (pauseOnMatchEnd)
        {
            yield return new WaitForSeconds(pauseDelay);
            Time.timeScale = 0f; // pausa geral (o painel já lida com isso também)
        }
    }

    void OnPlayerKO(PlayerHits who)
    {
        if (State != MatchState.Fighting) return;

        State = MatchState.KO;

        if (who == player1)
        {
            p2Wins++;
            ui?.ShowAnnounce($"K.O.!  {player2Name} Wins");
        }
        else
        {
            p1Wins++;
            ui?.ShowAnnounce($"K.O.!  {player1Name} Wins");
        }
    }

    int DecideByLife()
    {
        float p1 = player1 ? (player1.CurrentHP / player1.MaxHP) : 0f;
        float p2 = player2 ? (player2.CurrentHP / player2.MaxHP) : 0f;
        if (Mathf.Approximately(p1, p2)) return 0;
        return (p1 > p2) ? 1 : 2;
    }

    // util pública caso queira pausar externamente
    public void Pause(bool pause)
    {
        Time.timeScale = pause ? 0f : 1f;
    }

    // Getter para UI de timer (se a sua MatchUI ler daqui)
    public float GetTimeLeft() => Mathf.Max(0f, timeLeft);
}
