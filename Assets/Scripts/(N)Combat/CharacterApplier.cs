// Assets/Scripts/Characters/CharacterApplier.cs
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class CharacterApplier : MonoBehaviour
{
    [Header("Componentes alvo")]
    public SpriteRenderer spriteRenderer;  // se vazio, pega no Awake
    public Animator animator;              // se vazio, pega no Awake
    public PlayerHits hits;                // vida/dano já usados no seu projeto
    public CharacterMotor2D motor;         // movimento/jump já usado
    public FighterActions actions;         // onde seta baseDamage (seu script)

    void Awake()
    {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        if (!animator)       animator       = GetComponent<Animator>();
        if (!hits)           hits           = GetComponent<PlayerHits>();
        if (!motor)          motor          = GetComponent<CharacterMotor2D>();
        if (!actions)        actions        = GetComponent<FighterActions>();
    }

    public void Apply(CharacterSpec spec)
{
    if (!spec) return;

    // Visual
    if (spriteRenderer && spec.defaultSprite)
        spriteRenderer.sprite = spec.defaultSprite;

    if (animator && spec.animator)
        animator.runtimeAnimatorController = spec.animator;

    // Atributos
    if (hits)
    {
        // MaxHP + cura
        hits.OverrideMaxHP(spec.maxHP, heal: true);

        // Dano base está em PlayerHits (não em FighterActions)
        // Garanta que seu PlayerHits tenha o campo abaixo:
        // public float baseAttackDamage = 6f;
        hits.baseAttackDamage = spec.baseDamage;
    }

    if (motor)
    {
        motor.moveSpeed = spec.moveSpeed;
        motor.jumpForce = spec.jumpForce;
    }

    // FighterActions não guarda 'baseDamage' — nada a fazer aqui.
}

}
