using UnityEngine;

public class FightSceneSpawner : MonoBehaviour
{
    [Header("Sessão + Prefab")]
    public GameSession session;                  // acha sozinho se vazio
    public GameObject fighterPrefab;             // seu prefab base do lutador

    [Header("Spawns")]
    public Transform spawnP1;
    public Transform spawnP2;

    [Header("UI (opcional)")]
    public HealthBarUI leftHealthBar;            // aponta pro P1
    public HealthBarUI rightHealthBar;           // aponta pro P2
    public MatchController match;                // conecta os PlayerHits

    void Awake()
    {
        if (!session) session = FindAnyObjectByType<GameSession>();
        if (!match)   match   = FindAnyObjectByType<MatchController>();
    }

    void Start()
    {
        if (!fighterPrefab || session == null || spawnP1 == null || spawnP2 == null)
        {
            Debug.LogError("[FightSceneSpawner] Configure fighterPrefab/session/spawns.");
            return;
        }

        // 1) Instanciar P1 e P2
        var goP1 = Instantiate(fighterPrefab, spawnP1.position, Quaternion.identity);
        goP1.name = "Player1";

        var goP2 = Instantiate(fighterPrefab, spawnP2.position, Quaternion.identity);
        goP2.name = session.p2IsHuman ? "Player2" : "CPU";

        // 2) Aplicar CharacterSpec
        var specP1 = session.GetP1Spec();
        var specP2 = session.GetP2Spec();

        var applierP1 = goP1.GetComponent<CharacterApplier>();
        var applierP2 = goP2.GetComponent<CharacterApplier>();
        if (applierP1) applierP1.Apply(specP1);
        if (applierP2) applierP2.Apply(specP2);

        // 3) Ajustes de orientação e “opponent”
        var motorP1 = goP1.GetComponent<CharacterMotor2D>();
        var motorP2 = goP2.GetComponent<CharacterMotor2D>();
        if (motorP1 && motorP2)
        {
            motorP1.opponent = motorP2.transform;
            motorP2.opponent = motorP1.transform;

            // garanta que comecem se encarando
            var s1 = goP1.transform.localScale; s1.x =  1 * Mathf.Abs(s1.x); goP1.transform.localScale = s1;
            var s2 = goP2.transform.localScale; s2.x = -1 * Mathf.Abs(s2.x); goP2.transform.localScale = s2;
        }

        // 4) Conectar barras de vida
        var hitsP1 = goP1.GetComponent<PlayerHits>();
        var hitsP2 = goP2.GetComponent<PlayerHits>();
        if (leftHealthBar)  leftHealthBar.SetTarget(hitsP1);
        if (rightHealthBar) rightHealthBar.SetTarget(hitsP2);

        // 5) Entregar para o MatchController
        if (match)
        {
            match.player1 = hitsP1;
            match.player2 = hitsP2;
        }

        // 6) Ativar CPU ou P2 humano no lado 2 (P2ModeSwitcher no prefab do lado 2)
        var switcherP2 = goP2.GetComponent<P2ModeSwitcher>();
        if (switcherP2)
        {
            switcherP2.Apply(session.p2IsHuman); // true = P2 humano / false = CPU
        }
        else
        {
            // fallback: se não tiver switcher, liga/desliga controladores manualmente (opcional)
            var bot = goP2.GetComponent<MonoBehaviour>();          // troque pelo seu BotController2D_Pro
            var p2c = goP2.GetComponent<Player2Controller2D>();
            if (bot) bot.enabled = !session.p2IsHuman;
            if (p2c) p2c.enabled = session.p2IsHuman;
        }

        // 7) Se o prefab tiver P2ModeSwitcher, DESATIVE num Player 1 (para evitar troca acidental no P1)
        var swP1 = goP1.GetComponent<P2ModeSwitcher>();
        if (swP1) swP1.enabled = false;
    }
}
