using System.Collections;
using UnityEngine;

public enum MatchState { Idle, PreRound, Fighting, KO, TimeOver, RoundEnd, MatchEnd }

public class MatchController : MonoBehaviour
{
    [Header("Jogadores")]
    public PlayerHits player1;
    public PlayerHits player2;

    [Header("Regras")]
    public int roundTimeSeconds = 60;  // tempo por round
    public int winsToWinMatch = 2;     // melhor de 3 (2), melhor de 5 (3) etc.

    [Header("UI (opcional)")]
    public MatchUI ui;                 // script simples pra mostrar timer/placar/anúncios

    // Estado interno
    public MatchState State { get; private set; } = MatchState.Idle;
    private int p1Wins = 0, p2Wins = 0;
    private float timeLeft = 0f;
    [Header("Fim de Partida")]
    public bool pauseOnMatchEnd = true;
    public float pauseDelay = 1.0f;

    void Start()
    {
        // garante listeners
        player1.OnKO += OnPlayerKO;
        player2.OnKO += OnPlayerKO;

        StartCoroutine(RunMatch());
    }

    IEnumerator RunMatch()
    {
        State = MatchState.PreRound;

        while (p1Wins < winsToWinMatch && p2Wins < winsToWinMatch)
        {
            // PRE-ROUND
            player1.ResetForRound();
            player2.ResetForRound();

            timeLeft = roundTimeSeconds;
            ui?.ShowTimer(timeLeft);
            ui?.ShowRoundIntro($"Round {p1Wins + p2Wins + 1}");

            yield return new WaitForSeconds(1.0f); // “Round …”
            ui?.ShowAnnounce("Fight!");
            yield return new WaitForSeconds(0.6f);
            ui?.HideAnnounce();

            State = MatchState.Fighting;

            // FIGHTING LOOP
            while (State == MatchState.Fighting)
            {
                // Timer
                timeLeft = Mathf.Max(0f, timeLeft - Time.deltaTime);
                ui?.ShowTimer(timeLeft);

                if (timeLeft <= 0f)
                {
                    State = MatchState.TimeOver;
                    break;
                }
                yield return null;
            }

            // RESOLUÇÃO DO ROUND
            if (State == MatchState.TimeOver)
            {
                ui?.ShowAnnounce("Time Over!");
                yield return new WaitForSeconds(0.8f);

                int winner = DecideByLife(); // 1, 2 ou 0 (empate)
                if (winner == 1) { p1Wins++; ui?.ShowAnnounce("Player 1 Wins"); }
                else if (winner == 2) { p2Wins++; ui?.ShowAnnounce("Player 2 Wins"); }
                else { ui?.ShowAnnounce("Draw"); } // empate: nenhum ganha ponto

                ui?.UpdateScore(p1Wins, p2Wins);
                yield return new WaitForSeconds(1.0f);
                ui?.HideAnnounce();
            }
            else if (State == MatchState.KO)
            {
                // winner setado no OnPlayerKO
                ui?.UpdateScore(p1Wins, p2Wins);
                yield return new WaitForSeconds(1.0f);
                ui?.HideAnnounce();
            }

            State = MatchState.RoundEnd;
            yield return new WaitForSeconds(0.5f);
            State = MatchState.PreRound;
        }

       // MATCH END
    State = MatchState.MatchEnd;
    string finalMsg = (p1Wins > p2Wins) ? "Player 1 WINS THE MATCH!" : "Player 2 WINS THE MATCH!";
    ui?.ShowAnnounce(finalMsg);

    // pausa após um tempinho
    if (pauseOnMatchEnd)
    {
        yield return new WaitForSeconds(pauseDelay);
        Time.timeScale = 0f;    // pausa geral
    }

    }

    void OnPlayerKO(PlayerHits who)
    {
        if (State != MatchState.Fighting) return;

        State = MatchState.KO;

        if (who == player1) { p2Wins++; ui?.ShowAnnounce("KO!  Player 2 Wins"); }
        else { p1Wins++; ui?.ShowAnnounce("KO!  Player 1 Wins"); }
    }

    int DecideByLife()
    {
        float p1 = player1.CurrentHP / player1.MaxHP;
        float p2 = player2.CurrentHP / player2.MaxHP;
        if (Mathf.Approximately(p1, p2)) return 0;
        return (p1 > p2) ? 1 : 2;
    }

    // util pública caso queira pausar
    public void Pause(bool pause)
    {
        if (pause) Time.timeScale = 0f;
        else Time.timeScale = 1f;
    }
}
