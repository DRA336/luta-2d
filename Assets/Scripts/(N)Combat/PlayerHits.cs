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

    public event Action<float, float> OnHealthChanged; // (current, max)
    public event Action<HitData> OnHitReceived;
    public event Action<PlayerHits> OnKO;              // <- NOVO: dispara quando zera HP

    [Header("Estados")]
    public bool IsDefending { get; private set; }

    [Header("HurtBoxes (corpo) – lista de listas")]
    public List<ColliderGroup> hurtSets = new();

    [Header("DefenseBoxes – lista de listas")]
    public List<ColliderGroup> defenseSets = new();

    [Header("DamageHitBoxes – lista de listas")]
    public List<ColliderGroup> damageSets = new();

    int currentHurt = -1, currentDefense = -1, currentDamage = -1;

    void Awake()
    {
        ResetForRound();                       // <- garante HP cheio no start
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

    public float GetDano() => 10f;

    // ===== Dano recebido =====
    public void ReceiveHit(HitData hit)
    {
        if (IsKO) return;

        float realDamage = Mathf.Max(0f, hit.amount);
        CurrentHP = Mathf.Max(0f, CurrentHP - realDamage);

        OnHitReceived?.Invoke(hit);
        OnHealthChanged?.Invoke(CurrentHP, MaxHP);

        if (CurrentHP <= 0f && !IsKO)
        {
            IsKO = true;
            // desliga colliders ofensivos/defensivos
            ActivateOnly(damageSets,  ref currentDamage,  -1, false);
            ActivateOnly(defenseSets, ref currentDefense, -1, false);
            OnKO?.Invoke(this);               // <- avisa o MatchController
        }
    }

    // ===== Controle de round =====
    public void ResetForRound()
    {
        IsKO = false;
        CurrentHP = maxHP;
        IsDefending = false;

        SetHurtSet(Mathf.Clamp(currentHurt, 0, hurtSets.Count - 1)); // liga set inicial
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
