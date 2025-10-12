using UnityEngine;

[RequireComponent(typeof(CharacterMotor2D))]
[RequireComponent(typeof(FighterActions))]
public class PlayerController2D : MonoBehaviour
{
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public KeyCode jump = KeyCode.W;          // ou Space
    public KeyCode defend = KeyCode.S;        // segurar para defender
    public KeyCode jab = KeyCode.J;

    [Header("Bloqueios de controle")]
    public bool disableMovement = true;       // pedido 1: sem mover
    public bool disableJump = true;           // pedido 1: sem pular

    CharacterMotor2D motor;
    FighterActions actions;

    void Awake()
    {
        motor = GetComponent<CharacterMotor2D>();
        actions = GetComponent<FighterActions>();
    }

    void Update()
    {
        // Hurt-stun / KO bloqueia input
        if (!actions.CanAct())
        {
            motor.SetMove(0);
            actions.StopDefense();
            return;
        }

        // Horizontal
        float x = 0f;
        if (!disableMovement)
        {
            if (Input.GetKey(left))  x -= 1f;
            if (Input.GetKey(right)) x += 1f;
        }
        motor.SetMove(x);

        // Pulo
        if (!disableJump && Input.GetKeyDown(jump))
            motor.RequestJump();

        // Defesa (segurar)
        if (Input.GetKey(defend)) actions.StartDefense();
        else actions.StopDefense();

        // Ataque (jab)
        if (Input.GetKeyDown(jab) && actions.CanAct())
            actions.TryJab();
    }
}
