using UnityEngine;

/// Bot “Pro” — mantém espaçamento, ataca ao entrar na janela do preferredRange,
/// recua quando cola demais, e às vezes dá um passo atrás perto do maxRange.
/// Requer: CharacterMotor2D, FighterActions, Animator.
[RequireComponent(typeof(CharacterMotor2D))]
[RequireComponent(typeof(FighterActions))]
[RequireComponent(typeof(Animator))]
public class BotController2D_Pro : MonoBehaviour
{
    [Header("Alvo")]
    public Transform target;                     // Root do Player
    public MatchController match;                // opcional: trava fora do round

    [Header("Faixas de distância (m)")]
    public float minRange = 1.15f;               // < isso => recua
    public float preferredRange = 1.45f;         // “ponto doce” p/ atacar
    public float preferredWindow = 0.18f;        // ± janela do preferred
    public float maxRange = 1.95f;               // > isso => aproxima

    [Header("Movimento")]
    [Range(0f,1f)] public float approachAggression = 0.8f; // avança
    [Range(0f,1f)] public float retreatFactor = 0.6f;      // recua

    [Tooltip("Quando estiver perto de maxRange, chance por segundo de dar um pequeno passo pra trás para re-espacar.")]
    [Range(0f,1f)] public float tacticalBackstepChance = 0.25f;
    public float backstepTime = 0.18f;           // duração do recuo tático
    float backstepUntil = 0f;

    [Header("Ataque")]
    public float attackCooldown = 0.55f;         // ritmo de ataque
    [Range(0f,1f)] public float attackVariance = 0.10f; // aleatório no cooldown
    public float windupDelay = 0.06f;            // telegraph/hold antes do chute

    [Header("Defesa")]
    [Tooltip("Chance por segundo de defender quando muito perto (<= minRange).")]
    [Range(0f,1f)] public float defendChanceNear = 0.30f;
    public float defendTime = 0.25f;             // quanto tempo segura a defesa
    float defendUntil = 0f;

    // Interno
    CharacterMotor2D motor;
    FighterActions actions;
    Animator anim;
    Rigidbody2D rbTarget;

    float nextAttackTime = -999f;                // sentinela: pronto p/ atacar no início
    float holdUntil = 0f;                        // paciência (para de andar um pouco)

    void Awake()
    {
        motor   = GetComponent<CharacterMotor2D>();
        actions = GetComponent<FighterActions>();
        anim    = GetComponent<Animator>();

        if (target)
        {
            rbTarget = target.GetComponent<Rigidbody2D>();
            if (!motor.opponent) motor.opponent = target; // garante face ao player
        }
    }

    void Update()
    {
        // fora de combate? travar
        if (match && match.State != MatchState.Fighting)
        {
            motor.SetMove(0);
            actions.StopDefense();
            anim.SetBool("Running", false);
            return;
        }

        if (!target || !actions.CanAct())
        {
            motor.SetMove(0);
            actions.StopDefense();
            anim.SetBool("Running", false);
            return;
        }

        float dx   = target.position.x - transform.position.x;
        float dist = Mathf.Abs(dx);
        int   dir  = dx >= 0 ? 1 : -1;

        // ===================== DEFESA QUANDO MUITO PERTO =====================
        if (dist <= minRange + 0.05f)
        {
            // chance por segundo de levantar guarda
            if (Time.time >= defendUntil && Random.value < defendChanceNear * Time.deltaTime * 60f)
            {
                actions.StartDefense();
                defendUntil = Time.time + defendTime;
            }
        }
        // solta defesa quando o tempo acabar
        if (Time.time >= defendUntil) actions.StopDefense();

        // ===================== RECUO TÁTICO (MAX RANGE) ======================
        // Se está se “encostando” no maxRange, às vezes dá um passo pra trás p/ re-espacar
        bool nearMax = dist > (preferredRange + preferredWindow) && dist <= maxRange + 0.1f;
        if (nearMax && Time.time >= backstepUntil && Random.value < tacticalBackstepChance * Time.deltaTime * 60f)
        {
            backstepUntil = Time.time + backstepTime;
        }

        // ===================== MOVIMENTO/ESPAÇAMENTO =========================
        float moveX = 0f;

        if (Time.time < backstepUntil)
        {
            // recuo tático curto
            moveX = -dir * retreatFactor;
        }
        else if (dist < minRange)
        {
            // recua quando cola demais
            moveX = -dir * retreatFactor;
            // e “reseta” a paciência para não atacar colado
            holdUntil = Mathf.Max(holdUntil, Time.time + 0.15f);
        }
        else if (dist > maxRange)
        {
            // aproxima quando está longe
            moveX = dir * approachAggression;
        }
        else
        {
            // dentro da banda [minRange..maxRange]: segurar/ajuste fino
            // entra em “hold” curto (paciência) para não andar sem parar
            if (holdUntil <= Time.time) holdUntil = Time.time + Random.Range(0.10f, 0.25f);

            if (Time.time >= holdUntil)
            {
                // ajuste fino para tender ao preferred
                if (dist > preferredRange + 0.05f)      moveX = dir * (approachAggression * 0.35f);
                else if (dist < preferredRange - 0.05f) moveX = -dir * (retreatFactor * 0.35f);
                else                                     moveX = 0f;
            }
            else
            {
                moveX = 0f; // segura posição
            }
        }

        motor.SetMove(moveX);
        anim.SetBool("Running", Mathf.Abs(moveX) > 0.05f);

        // ===================== ATAQUE NA JANELA DO PREFERRED =================
        bool inPreferredWindow = Mathf.Abs(dist - preferredRange) <= preferredWindow;
        bool cooldownReady     = Time.time >= nextAttackTime;
        bool patiencePassed    = Time.time >= holdUntil || dist <= minRange + 0.05f; // se colou, pode atacar mesmo

        if (inPreferredWindow && cooldownReady && patiencePassed)
        {
            StartCoroutine(TelegraphAndKick()); // seta Trigger "Kick"
            nextAttackTime = Time.time + attackCooldown * Random.Range(1f - attackVariance, 1f + attackVariance);
        }
    }

    System.Collections.IEnumerator TelegraphAndKick()
    {
        // telegraph/hold curto antes do chute (parece humano)
        float t = windupDelay;
        while (t > 0f)
        {
            motor.SetMove(0);
            t -= Time.deltaTime;
            yield return null;
        }

        // animação de chute — ATENÇÃO: seus Animation Events AE_AttackOn/Off ligam a HitBox
        anim.SetTrigger("Kick");

        // Se ainda estiver ajustando os Animation Events, pode deixar este backup TEMPORÁRIO:
        // GetComponent<FighterActions>()?.TryJab();
    }

    // Gizmos para depurar as distâncias no editor
    void OnDrawGizmosSelected()
    {
        Vector3 o = transform.position + Vector3.up * 1.0f;
        float sgn = (transform.localScale.x >= 0) ? 1f : -1f;

        Gizmos.color = Color.red;     Gizmos.DrawLine(o, o + Vector3.right * minRange * sgn);
        Gizmos.color = Color.yellow;  Gizmos.DrawLine(o + Vector3.up*0.05f, o + Vector3.right * preferredRange * sgn);
        Gizmos.color = Color.green;   Gizmos.DrawLine(o + Vector3.up*0.10f, o + Vector3.right * maxRange * sgn);
    }
}
