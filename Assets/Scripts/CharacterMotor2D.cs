using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMotor2D : MonoBehaviour
{
    [Header("Movimento")]
    public float moveSpeed = 6f;
    public float jumpForce = 12f;
    public bool canAirControl = false;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.1f;
    public LayerMask groundLayer = ~0;

    [Header("Facing")]
    public Transform opponent;        // arraste o outro lutador (Player ou Bot)
    public bool autoFaceOpponent = true;

    Rigidbody2D rb;
    bool isGrounded;
    float inputX;
    bool wantJump;

    public bool IsGrounded => isGrounded;
    public float InputX => inputX;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!groundCheck)
        {
            // cria um groundCheck simples abaixo do pé se não tiver
            GameObject gc = new GameObject("GroundCheck");
            gc.transform.SetParent(transform);
            gc.transform.localPosition = new Vector3(0, -0.9f, 0);
            groundCheck = gc.transform;
        }
    }

    void Update()
    {
        // virar para o oponente (sem travar quando cruza)
        if (autoFaceOpponent && opponent)
        {
            float dx = opponent.position.x - transform.position.x;
            if (Mathf.Abs(dx) > 0.01f)
            {
                Vector3 s = transform.localScale;
                s.x = Mathf.Sign(dx) * Mathf.Abs(s.x);
                transform.localScale = s;
            }
        }
    }

    void FixedUpdate()
    {
        // ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        // mover
        float vx = inputX * moveSpeed;
        float vy = rb.linearVelocity.y;

        if (!isGrounded && !canAirControl) vx = rb.linearVelocity.x; // sem controle no ar
        rb.linearVelocity = new Vector2(vx, vy);

        // pular
        if (wantJump && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        wantJump = false;
    }

    // APIs chamadas pelos controladores (player/bot)
    public void SetMove(float x)
    {
        inputX = Mathf.Clamp(x, -1f, 1f);
    }

    public void RequestJump()
    {
        wantJump = true;
    }
}
