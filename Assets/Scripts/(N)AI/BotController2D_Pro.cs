using UnityEngine;

[RequireComponent(typeof(CharacterMotor2D))]
[RequireComponent(typeof(FighterActions))]
[RequireComponent(typeof(Animator))]
public class BotController2D_Pro : MonoBehaviour
{
    [Header("Alvos")]
    public Transform target;                       // Player raiz
    CharacterMotor2D targetMotor;                  // Para saber se o player está no chão

    [Header("Faixa de distância (m)")]
    public float minRange = 1.10f;                 // muito perto => recua
    public float preferredRange = 1.45f;           // “ponto doce” para atacar
    public float maxRange = 1.85f;                 // muito longe => aproxima

    [Header("Movimento")]
    [Range(0f,1f)] public float approachAggression = 0.9f;   // 0..1 (o quanto anda por tick ao aproximar)
    [Range(0f,1f)] public float retreatFactor = 0.7f;        // velocidade ao recuar
    public float holdMin = 0.15f;                  // segurar posição por X..Y antes do ataque
    public float holdMax = 0.35f;

    [Header("Ataque")]
    public float attackCooldown = 0.85f;           // tempo mínimo entre ataques
    public float windupDelay = 0.10f;              // pausa curta antes do kick (telegrapho)
    [Range(0f,1f)] public float attackVariance = 0.15f; // aleatoriedade no cooldown

    [Header("Defesa / Anti-air")]
    [Range(0f,1f)] public float defendChance = 0.25f; // chance por segundo quando perto
    public float antiAirVyThreshold = 4.0f;           // se player subindo rápido
    public float antiAirHold = 0.20f;                 // segura posição por este tempo
    public float microRetreat = 0.35f;                // pequeno recuo quando AA ativa

    [Header("Pulos (evitar pular junto)")]
    public float jumpCooldown = 1.8f;                 // raramente pula
    [Range(0f,1f)] public float jumpChanceNear = 0.05f;

    [Header("Match (opcional)")]
    public MatchController match;

    // Interno
    CharacterMotor2D motor;
    FighterActions actions;
    Animator anim;

    float nextAttackTime = -999f;
    float holdUntil = 0f;
    float nextJumpTime = 0f;

    void Awake()
    {
        motor   = GetComponent<CharacterMotor2D>();
        actions = GetComponent<FighterActions>();
        anim    = GetComponent<Animator>();
        if (target) targetMotor = target.GetComponent<CharacterMotor2D>();
    }

    void Update()
    {
        // trava o bot fora do combate
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

        // Distâncias básicas
        float dx = target.position.x - transform.position.x;
        float dist = Mathf.Abs(dx);
        int dir = dx > 0 ? 1 : -1;

        // ------- ANTI-AIR / NÃO IMITAR PULO -------
        bool playerAir = (targetMotor ? !targetMotor.IsGrounded : false);
        float playerVy = 0f;
        if (targetMotor) playerVy = targetMotor.GetComponent<Rigidbody2D>() ? targetMotor.GetComponent<Rigidbody2D>().linearVelocity.y : 0f;

        bool antiAirWindow = playerAir && playerVy > antiAirVyThreshold; // player subindo rápido
        if (antiAirWindow)
        {
            // em vez de pular junto: segurar posição + micro recuo e pode defender
            motor.SetMove(-dir * retreatFactor);
            anim.SetBool("Running", Mathf.Abs(retreatFactor) > 0.05f);

            if (Random.value < defendChance * Time.deltaTime * 60f)
                actions.StartDefense();
            else
                actions.StopDefense();

            holdUntil = Time.time + antiAirHold; // segura um pouco antes de qualquer ação
            return;
        }

        // solta a defesa se não há pressão
        actions.StopDefense();

        // ------- CONTROLE DE ESPAÇAMENTO -------
        float moveX = 0f;

        // Se estiver muito perto, recua
        if (dist < minRange)
        {
            moveX = -dir * retreatFactor;
            holdUntil = Mathf.Max(holdUntil, Time.time + Random.Range(holdMin * 0.5f, holdMin)); // “reseta” a paciência
        }
        // Se perto do ideal, segura (paciência) por um curtinho período
        else if (dist >= minRange && dist <= maxRange)
        {
            // entra em “hold” por um tempo curto (evita andar sem parar)
            if (holdUntil <= Time.time)
                holdUntil = Time.time + Random.Range(holdMin, holdMax);

            // enquanto estiver no hold, quase não mexe
            if (Time.time < holdUntil)
                moveX = 0f;
            else
            {
                // ajusta fino: se está mais longe que o ideal, aproxima um pouco
                if (dist > preferredRange) moveX = dir * (approachAggression * 0.35f);
                else if (dist < preferredRange) moveX = -dir * (retreatFactor * 0.35f);
            }
        }
        // Longe demais: aproxima
        else if (dist > maxRange)
        {
            moveX = dir * approachAggression;
        }

        motor.SetMove(moveX);
        anim.SetBool("Running", Mathf.Abs(moveX) > 0.05f);

        // ------- ATAQUE (sem depender do pulo do player) -------
        // Ataca quando estiver na faixa ideal e não estiver em hold/paciência
        bool inIdeal = dist >= (preferredRange - 0.08f) && dist <= (preferredRange + 0.10f);
        bool cooldownReady = Time.time >= nextAttackTime;
        bool notHolding = Time.time >= holdUntil;

        if (inIdeal && cooldownReady && notHolding)
        {
            // pequeno "telegraph": para um instante antes de chutar
            StartCoroutine(TelegraphAndKick());
            // agenda próximo ataque com variação
            float variance = attackCooldown * Random.Range(1f - attackVariance, 1f + attackVariance);
            nextAttackTime = Time.time + variance;
        }

        // ------- DEFESA CASUAL quando colado/pressionado -------
        if (dist <= minRange + 0.05f && Random.value < defendChance * Time.deltaTime * 60f)
            actions.StartDefense();

        // ------- PULO (raramente e nunca em sincronia) -------
        if (Time.time >= nextJumpTime && Random.value < jumpChanceNear * Time.deltaTime * 60f)
        {
            // só pula se um pouco longe do ideal, para variar eixo
            if (dist > preferredRange * 1.1f)
                motor.RequestJump();

            nextJumpTime = Time.time + jumpCooldown * Random.Range(0.8f, 1.2f);
        }
    }

    System.Collections.IEnumerator TelegraphAndKick()
    {
        // segura posição por um instante (windup)
        float t = windupDelay;
        while (t > 0f)
        {
            motor.SetMove(0);
            t -= Time.deltaTime;
            yield return null;
        }

        // dispara animação de chute
        var animator = GetComponent<Animator>();
        animator.SetTrigger("Kick");

        // Se você NÃO estiver usando Animation Events, descomente a linha abaixo:
        // actions.TryJab();
    }

    // Gizmos pra depurar distâncias no editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;   Gizmos.DrawLine(transform.position + Vector3.up*1.2f, transform.position + Vector3.right *  minRange *  (transform.localScale.x>=0?1:-1));
        Gizmos.color = Color.yellow;Gizmos.DrawLine(transform.position + Vector3.up*1.1f, transform.position + Vector3.right *  preferredRange*(transform.localScale.x>=0?1:-1));
        Gizmos.color = Color.green; Gizmos.DrawLine(transform.position + Vector3.up*1.0f, transform.position + Vector3.right *  maxRange   * (transform.localScale.x>=0?1:-1));
    }
}
