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

    CharacterMotor2D motor;
    FighterActions actions;

    void Awake()
    {
        motor = GetComponent<CharacterMotor2D>();
        actions = GetComponent<FighterActions>();
    }

    void Update()
    {
        // input horizontal
        float x = 0f;
        if (Input.GetKey(left)) x -= 1f;
        if (Input.GetKey(right)) x += 1f;
        motor.SetMove(x);

        // pulo
        if (Input.GetKeyDown(jump))
            motor.RequestJump();

        // defesa (segurar)
        if (Input.GetKey(defend)) actions.StartDefense();
        else actions.StopDefense();

        // jab
        if (Input.GetKeyDown(jab) && actions.CanAct())
            actions.TryJab();
    }
}
