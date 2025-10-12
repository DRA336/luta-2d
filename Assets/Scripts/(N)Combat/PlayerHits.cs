using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ColliderGroup
{
    public string name;
    public List<Collider2D> colliders = new();
}

public class PlayerHits : MonoBehaviour, IDamageable
{
    [Header("Vida / Dano")]
    [SerializeField] private float maxHP = 100f;
    public float MaxHP => maxHP;
    public float CurrentHP { get; private set; }
    public bool IsKO { get; private set; } = false;

    [Header("Ataque")]
    [Tooltip("Dano base retornado por GetDano() (use isto para balancear chutes/socos simples).")]
    public float baseAttackDamage = 6f;          // <<< ataque menos forte por padrão
    public float GetDano() => baseAttackDamage;

    public event Action<float, float> OnHealthChanged; // (current, max)
    public event Action<HitData> OnHitReceived;
    public event Action<PlayerHits> OnKO;

    [Header("Estados")]
    public bool IsDefending { get; private set; }

    [Header("HurtBoxes (corpo) – lista de listas")]
    public List<ColliderGroup> hurtSets = new();

    [Header("DefenseBoxes – lista de listas")]
    public List<ColliderGroup> defenseSets = new();

    [Header("DamageHitBoxes – lista de listas")]
    public List<ColliderGroup> damageSets = new();

    [Header("Defesa (balanço)")]
    [Range(0f, 1f)] public float blockMultiplier = 0.35f;   // toma 35% do dano ao defender
    [Range(0f, 1f)] public float chipMultiplier  = 0.00f;   // 0 = sem chip
    public bool useChipOnBlock = false;

    // ---- HURT / STUN ----
    [Header("Hurt (stun)")]
    public float hurtStun = 0.25f;
    public string hurtTrigger = "Hurt";

    int currentHurt = -1, currentDefense = -1, currentDamage = -1;

    // refs auxiliares
    Animator anim;
    FighterActions actions;

    void Awake()
    {
        anim = GetComponent<Animator>();
        actions = GetComponent<FighterActions>();
        ResetForRound();
    }

    // ===== API p/ animação/estado =====
    public void SetHurtSet(int index)    => ActivateOnly(hurtSets,    ref currentHurt,    index, true);
    public void SetDefenseSet(int index) => ActivateOnly(defenseSets, ref currentDefense, index, IsDefending);
    public void SetDamageSet(int index)  => ActivateOnly(damageSets,  ref currentDamage,  index, true);

    public void StartDefense(int defenseSetIndex)
    {
        IsDefending = true;
        SetDefenseSet(defenseSetIndex);
    }

    public void StopDefense()
    {
        IsDefending = false;
        ActivateOnly(defenseSets, ref currentDefense, -1, false);
    }

    // ===== Dano recebido =====
    public void ReceiveHit(HitData hit)
    {
        if (IsKO) return;

        float incoming = Mathf.Max(0f, hit.amount);
        float finalDamage = incoming;

        bool blocked = IsDefending;

        if (blocked)
        {
            if (useChipOnBlock && chipMultiplier > 0f)
                finalDamage = incoming * chipMultiplier;     // chip
            else
                finalDamage = incoming * blockMultiplier;    // redução

            var shield = GetComponentInChildren<ShieldVisual>();
            if (shield) shield.Flash();
        }
        else
        {
            // NÃO defendendo: aplica hurt (trava input) + trigger de animação
            if (actions) actions.LockInput(hurtStun);
            if (anim && !string.IsNullOrEmpty(hurtTrigger)) anim.SetTrigger(hurtTrigger);
        }

        // aplica dano
        CurrentHP = Mathf.Max(0f, CurrentHP - finalDamage);

        // eventos/UI
        OnHitReceived?.Invoke(hit);
        OnHealthChanged?.Invoke(CurrentHP, MaxHP);

        // KO
        if (CurrentHP <= 0f && !IsKO)
        {
            IsKO = true;
            SetDamageSet(-1);
            SetDefenseSet(-1);
            OnKO?.Invoke(this);
        }
    }

    // ===== Controle de round =====
    public void ResetForRound()
    {
        IsKO = false;
        CurrentHP = maxHP;
        IsDefending = false;

        SetHurtSet(Mathf.Clamp(currentHurt, 0, hurtSets.Count - 1));
        ActivateOnly(defenseSets, ref currentDefense, -1, false);
        ActivateOnly(damageSets,  ref currentDamage,  -1, false);

        OnHealthChanged?.Invoke(CurrentHP, MaxHP);
    }

    // ===== Util =====
    void ActivateOnly(List<ColliderGroup> groups, ref int current, int index, bool enableTarget)
    {
        for (int i = 0; i < groups.Count; i++)
        {
            bool enable = (i == index) && enableTarget;
            foreach (var c in groups[i].colliders)
                if (c) c.enabled = enable;
        }
        current = index;
    }
}
