using UnityEngine;

[RequireComponent(typeof(CharacterMotor2D))]
[RequireComponent(typeof(FighterActions))]
[RequireComponent(typeof(Animator))]
public class BotController2D : MonoBehaviour
{
    public Transform target;              // Player
    public float desiredRange = 1.4f;
    public float approachSpeed = 1f;
    public float attackCooldown = 0.8f;
    public float defendChance = 0.2f;

    // Opcional: para travar o bot fora do round
    public MatchController match;

    CharacterMotor2D motor;
    FighterActions actions;
    Animator anim;
    float lastAttack = -999f;

    void Awake()
    {
        motor = GetComponent<CharacterMotor2D>();
        actions = GetComponent<FighterActions>();
        anim   = GetComponent<Animator>();
    }

    void Update()
    {
        // Se existe MatchController, só age quando estiver lutando
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

        float dx = target.position.x - transform.position.x;
        float dist = Mathf.Abs(dx);

        // Aproxima/afasta até a faixa desejada
        float moveX = 0f;
        if (dist > desiredRange * 1.05f)      moveX = Mathf.Sign(dx) * approachSpeed;
        else if (dist < desiredRange * 0.75f) moveX = -Mathf.Sign(dx) * approachSpeed * 0.6f;
        motor.SetMove(moveX);

        // Atualiza parâmetro de corrida no Animator
        bool running = motor.IsGrounded && Mathf.Abs(moveX) > 0.05f;
        anim.SetBool("Running", running);

        // Defesa casual quando perto
        if (dist <= desiredRange && Random.value < defendChance * Time.deltaTime * 60f)
            actions.StartDefense();
        else
            actions.StopDefense();

        // Ataque: dispare animação e a lógica de hitbox
        if (dist <= desiredRange && Time.time - lastAttack >= attackCooldown)
        {
            lastAttack = Time.time;

            // 1) animação Kick (o dano liga pelos Animation Events AE_AttackOn/Off)
            anim.SetTrigger("Kick");

            // 2) Se você NÃO estiver usando Animation Events,
            //    deixe também a linha abaixo para ligar a hitbox via FighterActions:
            // actions.TryJab();
        }
    }
}
