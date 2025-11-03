using UnityEngine;

public class FightSceneSpawner : MonoBehaviour
{
    [Header("Sessão + Spawns")]
    public GameSession session;     
    public Transform spawnP1;
    public Transform spawnP2;

    [Header("UI (opcional)")]
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

        if (!specP1 || !specP1.fighterPrefab || !specP2 || !specP2.fighterPrefab)
        {
            Debug.LogError("[FightSceneSpawner] CharacterSpec ou fighterPrefab não configurados.");
            return;
        }
        if (!spawnP1 || !spawnP2)
        {
            Debug.LogError("[FightSceneSpawner] Defina spawnP1 e spawnP2.");
            return;
        }

        // 1) Instanciar prefabs
        var goP1 = Instantiate(specP1.fighterPrefab, spawnP1.position, Quaternion.identity);
        goP1.name = "Player1";

        var goP2 = Instantiate(specP2.fighterPrefab, spawnP2.position, Quaternion.identity);
        goP2.name = session != null && session.p2IsHuman ? "Player2" : "CPU";

        // 2) Encarar
        FaceRight(goP1, true);
        FaceRight(goP2, false);

        // 3) Opponents nos motores
        var motorP1 = goP1.GetComponent<CharacterMotor2D>();
        var motorP2 = goP2.GetComponent<CharacterMotor2D>();
        if (motorP1 && motorP2) { motorP1.opponent = goP2.transform; motorP2.opponent = goP1.transform; }

        // 4) Barras de vida
        var hitsP1 = goP1.GetComponent<PlayerHits>();
        var hitsP2 = goP2.GetComponent<PlayerHits>();
        if (leftHealthBar)  leftHealthBar.SetTarget(hitsP1);
        if (rightHealthBar) rightHealthBar.SetTarget(hitsP2);

        // 5) Overrides (opcional)
        ApplyOverrides(specP1, hitsP1);
        ApplyOverrides(specP2, hitsP2);

        // 6) MatchController
        if (match) { match.player1 = hitsP1; match.player2 = hitsP2; }

        // 7) CPU ↔ P2 humano
        var switcherP2 = goP2.GetComponent<P2ModeSwitcher>();
        if (switcherP2) switcherP2.Apply(session != null && session.p2IsHuman);
        var swP1 = goP1.GetComponent<P2ModeSwitcher>();
        if (swP1) swP1.enabled = false; // garante que P1 não alterna por engano
    }

    void FaceRight(GameObject go, bool right)
    {
        var s = go.transform.localScale;
        s.x = right ? Mathf.Abs(s.x) : -Mathf.Abs(s.x);
        go.transform.localScale = s;
    }

    void ApplyOverrides(CharacterSpec spec, PlayerHits hits)
    {
        if (!spec || !hits) return;

        if (spec.overrideMaxHP > 0f)
        {
            // Se você tem um método para alterar MaxHP/curar, chame aqui:
            // hits.OverrideMaxHP(spec.overrideMaxHP, heal:true);
        }
        if (spec.overrideBaseDamage >= 0)
        {
            // hits.baseAttackDamage = spec.overrideBaseDamage;
        }
    }
}
