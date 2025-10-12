using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMotor2D : MonoBehaviour
{
    [Header("Movimento no chão")]
    public float moveSpeed = 6f;

    [Header("Pulo")]
    public float jumpForce = 12f;
    public int maxJumps = 1;                // <<< limite de pulos (pedido 2)
    public float coyoteTime = 0.08f;        // tolerância após sair do chão

    [Header("Controle no ar")]
    public bool canAirControl = false;      // mantém compat: se false, igual antes
    [Range(0f,1f)] public float airControl = 0.35f; // <<< quão rápido responde no ar
    public float maxAirSpeed = 3.2f;        // <<< clamp da velocidade horizontal no ar

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.1f;
    public LayerMask groundLayer = ~0;

    [Header("Facing")]
    public Transform opponent;
    public bool autoFaceOpponent = true;

    Rigidbody2D rb;
    bool isGrounded;
    float inputX;
    bool wantJump;
    int jumpsUsed = 0;
    float lastGroundedTime = -999f;

    public bool IsGrounded => isGrounded;
    public float InputX => inputX;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!groundCheck)
        {
            var gc = new GameObject("GroundCheck");
            gc.transform.SetParent(transform);
            gc.transform.localPosition = new Vector3(0, -0.9f, 0);
            groundCheck = gc.transform;
        }
    }

    void Update()
    {
        // virar para o oponente
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
        if (isGrounded) { jumpsUsed = 0; lastGroundedTime = Time.time; }

        // movimento
        float vxTarget = inputX * moveSpeed;

        if (!isGrounded && canAirControl)
        {
            // ar com controle reduzido
            float vx = Mathf.Lerp(rb.linearVelocity.x, vxTarget, airControl);
            vx = Mathf.Clamp(vx, -maxAirSpeed, maxAirSpeed);
            rb.linearVelocity = new Vector2(vx, rb.linearVelocity.y);
        }
        else
        {
            // chão (ou ar sem controle)
            float vx = (!isGrounded && !canAirControl) ? rb.linearVelocity.x : vxTarget;
            rb.linearVelocity = new Vector2(vx, rb.linearVelocity.y);
        }

        // pulo (coyote + contador)
        bool canCoyote = (Time.time - lastGroundedTime) <= coyoteTime;
        bool canJump = (isGrounded || canCoyote) && (jumpsUsed < maxJumps);

        if (wantJump && canJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpsUsed++;
        }
        wantJump = false;
    }

    // APIs chamadas pelos controladores (player/bot)
    public void SetMove(float x)  { inputX = Mathf.Clamp(x, -1f, 1f); }
    public void RequestJump()     { wantJump = true; }
}
