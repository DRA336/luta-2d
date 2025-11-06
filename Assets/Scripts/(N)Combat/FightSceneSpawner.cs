using UnityEngine;

public class FightSceneSpawner : MonoBehaviour
{
    [Header("Sessão + Prefab Base + Spawns")]
    public GameSession session;
    public GameObject fighterBasePrefab;         // único prefab base
    public Transform spawnP1;
    public Transform spawnP2;

    [Header("UI")]
    public HealthBarUI leftHealthBar;
    public HealthBarUI rightHealthBar;

    [Header("Match")]
    public MatchController match;

    void Awake()
    {
        if (!session) session = FindAnyObjectByType<GameSession>();
        if (!match)   match   = FindAnyObjectByType<MatchController>();
    }

    void Start()
    {
        var specP1 = session ? session.GetP1Spec() : null;
        var specP2 = session ? session.GetP2Spec() : null;

        if (!fighterBasePrefab || !spawnP1 || !spawnP2 || !specP1 || !specP2)
        {
            Debug.LogError("[Spawner] Configure fighterBasePrefab, spawns e specs.");
            return;
        }

        var goP1 = Instantiate(fighterBasePrefab, spawnP1.position, Quaternion.identity);
        goP1.name = "Player1";
        var goP2 = Instantiate(fighterBasePrefab, spawnP2.position, Quaternion.identity);
        goP2.name = (session != null && session.p2IsHuman) ? "Player2" : "CPU";

        // orienta
        FaceRight(goP1, true);
        FaceRight(goP2, false);

        // instala visuals + stats
        goP1.GetComponent<VisualInstaller>()?.Install(specP1);
        goP2.GetComponent<VisualInstaller>()?.Install(specP2);

        // opponents
        var m1 = goP1.GetComponent<CharacterMotor2D>();
        var m2 = goP2.GetComponent<CharacterMotor2D>();
        if (m1 && m2) { m1.opponent = goP2.transform; m2.opponent = goP1.transform; }

        // ligar controladores:
        // no FighterBase, deixe Player1Controller, Player2Controller e BotController DESLIGADOS por padrão.
        // aqui você liga o necessário:
        // P1 -> Player1Controller on
        // P2 -> Player2Controller on (se session.p2IsHuman) ou Bot on (se não humano)
        EnableControllers(goP1, p1:true, p2Human:false);
        EnableControllers(goP2, p1:false, p2Human:(session!=null && session.p2IsHuman));

        // barras
        var h1 = goP1.GetComponent<PlayerHits>();
        var h2 = goP2.GetComponent<PlayerHits>();
        if (leftHealthBar)  leftHealthBar.SetTarget(h1);
        if (rightHealthBar) rightHealthBar.SetTarget(h2);

        // match
        if (match) { match.player1 = h1; match.player2 = h2; }
    }

    void FaceRight(GameObject go, bool right)
    {
        var s = go.transform.localScale;
        s.x = right ? Mathf.Abs(s.x) : -Mathf.Abs(s.x);
        go.transform.localScale = s;
    }

    void EnableControllers(GameObject go, bool p1, bool p2Human)
    {
        // troque os tipos abaixo pelos seus nomes reais
        var p1Ctrl = go.GetComponent<PlayerController2D>();     // seu script de P1
        var p2Ctrl = go.GetComponent<Player2Controller2D>();    // seu script de P2
        var bot    = go.GetComponent<BotController2D_Pro>();    // seu script de Bot

        if (p1)
        {
            if (p1Ctrl) p1Ctrl.enabled = true;
            if (p2Ctrl) p2Ctrl.enabled = false;
            if (bot)    bot.enabled    = false;
        }
        else
        {
            if (p1Ctrl) p1Ctrl.enabled = false;
            if (p2Ctrl) p2Ctrl.enabled = p2Human;
            if (bot)    bot.enabled    = !p2Human;
        }
    }
}
