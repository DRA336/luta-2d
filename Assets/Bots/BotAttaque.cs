using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BotAttaque : MonoBehaviour
{
    [Header("Vida do Bot")]
    [SerializeField]
    private float maxHP = 100f;
    [SerializeField]
    private float currentHP;

    [Header("Dano do Bot")]
    [SerializeField]
    private float danoBase = 10f;

    public float MaxHP { get { return maxHP; } }
    public float CurrentHP { get { return currentHP; } }
    public float DanoBase { get { return danoBase; } }

    private Animator animator;
    private bool isAttacking = false;

    [Header("Referência à LifeBars")]
    public LifeBars lifeBars;

    void Start()
    {
        currentHP = maxHP;
        animator = GetComponent<Animator>();

        if (lifeBars == null)
        {
            Debug.LogWarning("LifeBars reference não atribuída no BotAttaque.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftBracket)) // "[" key
        {
            if (!isAttacking)
            {
                animator.SetTrigger("Kick");
                StartCoroutine(AtivarAtaque(0.2f));
            }
        }
    }

    private IEnumerator AtivarAtaque(float duration)
    {
        isAttacking = true;
        Debug.Log("Bot ataque iniciado.");
        yield return new WaitForSeconds(duration);
        isAttacking = false;
        Debug.Log("Bot ataque finalizado.");
    }

    public void ReceberDano(float damage)
    {
        currentHP = Mathf.Max(currentHP - damage, 0);
        Debug.Log("Bot recebeu dano: " + damage + ", HP atual: " + currentHP);

        if (lifeBars != null)
        {
            // lifeBars.TakeDamage(false, damage); // Removed as LifeBars now reads HP directly
        }

        if (currentHP <= 0)
        {
            Debug.Log("Bot morreu.");
            // Aqui você pode adicionar lógica para morte do bot
        }
    }
}
