using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterMotor2D))]
[RequireComponent(typeof(FighterActions))]
public class AnimatorDriver : MonoBehaviour
{
    public KeyCode kickKey = KeyCode.J;

    Animator anim;
    CharacterMotor2D motor;
    FighterActions actions;

    void Awake()
    {
        anim = GetComponent<Animator>();
        motor = GetComponent<CharacterMotor2D>();
        actions = GetComponent<FighterActions>();
    }

    void Update()
    {
        // Idle/Run
        bool running = motor.IsGrounded && Mathf.Abs(motor.InputX) > 0.05f;
        anim.SetBool("Running", running);

        // Chute
        if (Input.GetKeyDown(kickKey) && actions.CanAct())
        {
            anim.SetTrigger("Kick");
            // (o dano acontece via Animation Events AE_AttackOn/Off)
        }
    }
}
