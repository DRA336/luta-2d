using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHits : MonoBehaviour
{
    [Header("Movimentação do jogador")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    private bool isGrounded;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [Header("Dano do personagem")]
    public float danoBase = 10f;

    [Header("Vida do Jogador")]
    [SerializeField]
    private float maxHP = 100f;
    [SerializeField]
    private float currentHP;

    public float MaxHP { get { return maxHP; } }
    public float CurrentHP { get { return currentHP; } }

    [Header("Lista de hitboxes")]
    public List<GameObject> hitboxes;

    [Header("Referência à LifeBars")]
    public LifeBars lifeBars;

    [Header("Teclas de controle")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode kickKey = KeyCode.K;

    private bool isFlipped = false;
    private bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        currentHP = maxHP;

        if (lifeBars == null)
        {
            Debug.LogWarning("LifeBars reference não atribuída no PlayerHits.");
        }

        Debug.Log("Hitboxes registradas para " + gameObject.name + ":");
        foreach (GameObject hitbox in hitboxes)
        {
            Debug.Log("- " + hitbox.name);
            Collider2D collider = hitbox.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = true;
            }
        }
        Debug.Log("Dano base atual: " + danoBase);
    }

    void Update()
    {
        Move();
        Jump();
        Kick();
    }

    void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        if (moveInput > 0 && isFlipped)
        {
            isFlipped = false;
            spriteRenderer.flipX = false;
            FlipHitboxes(false);
            animator.SetBool("Flip", false);
        }
        else if (moveInput < 0 && !isFlipped)
        {
            isFlipped = true;
            spriteRenderer.flipX = true;
            FlipHitboxes(true);
            animator.SetBool("Flip", true);
        }
    }

    void Jump()
    {
        if (isGrounded && Input.GetKeyDown(jumpKey))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
        }
    }

    void Kick()
    {
        if (Input.GetKeyDown(kickKey))
        {
            animator.SetTrigger("Kick");
            StartCoroutine(AtivarAtaque(0.2f));
        }
    }

    private IEnumerator AtivarAtaque(float duration)
    {
        isAttacking = true;
        Debug.Log("Ataque iniciado.");
        yield return new WaitForSeconds(duration);
        isAttacking = false;
        Debug.Log("Ataque finalizado.");
    }

    void FlipHitboxes(bool flip)
    {
        foreach (GameObject hitbox in hitboxes)
        {
            Vector3 position = hitbox.transform.localPosition;
            position.x = flip ? -Mathf.Abs(position.x) : Mathf.Abs(position.x);
            hitbox.transform.localPosition = position;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public float GetDano()
    {
        return danoBase;
    }

    public void ReceberDano(float damage)
    {
        currentHP = Mathf.Max(currentHP - damage, 0);
        Debug.Log("Player recebeu dano: " + damage + ", HP atual: " + currentHP);

        if (lifeBars != null)
        {
            // lifeBars.TakeDamage(true, damage); // Removed as LifeBars now reads HP directly
        }

        if (currentHP <= 0)
        {
            Debug.Log("Player morreu.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAttacking && collision.CompareTag("Bot"))
        {
            Debug.Log("Acertou o inimigo com dano: " + danoBase);
            BotAttaque bot = collision.GetComponent<BotAttaque>();
            if (bot != null)
            {
                bot.ReceberDano(danoBase);
            }
        }
        else if (collision.CompareTag("BotAttack"))
        {
            BotAttaque bot = collision.GetComponent<BotAttaque>();
            if (bot != null)
            {
                ReceberDano(bot.DanoBase);
            }
        }
    }
}
