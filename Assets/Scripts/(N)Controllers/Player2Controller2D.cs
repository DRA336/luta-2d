using UnityEngine;

[RequireComponent(typeof(CharacterMotor2D))]
[RequireComponent(typeof(FighterActions))]
public class Player2Controller2D : MonoBehaviour
{
    public KeyCode left   = KeyCode.LeftArrow;
    public KeyCode right  = KeyCode.RightArrow;
    public KeyCode jump   = KeyCode.UpArrow;
    public KeyCode defend = KeyCode.DownArrow;
    public KeyCode jab    = KeyCode.RightControl;

    CharacterMotor2D motor;
    FighterActions actions;

    void Awake(){
        motor = GetComponent<CharacterMotor2D>();
        actions = GetComponent<FighterActions>();
    }

    void Update()
    {
        if (!actions.CanAct()) { motor.SetMove(0); actions.StopDefense(); return; }

        float x = 0f;
        if (Input.GetKey(left))  x -= 1f;
        if (Input.GetKey(right)) x += 1f;
        motor.SetMove(x);

        if (Input.GetKeyDown(jump)) motor.RequestJump();

        if (Input.GetKey(defend)) actions.StartDefense();
        else actions.StopDefense();

        if (Input.GetKeyDown(jab) && actions.CanAct())
            actions.TryJab();
    }
}
