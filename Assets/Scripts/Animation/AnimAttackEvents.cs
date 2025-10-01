using UnityEngine;

public class AnimAttackEvents : MonoBehaviour
{
    public PlayerHits hits;
    [Tooltip("Índice do Damage Set que tem SUA única HitBox de ataque")]
    public int damageSetIndex = 0;

    void Reset() { hits = GetComponent<PlayerHits>(); }

    // Chamados pelos Animation Events no clip "Kick"
    public void AE_AttackOn()  { if (hits) hits.SetDamageSet(damageSetIndex); }
    public void AE_AttackOff() { if (hits) hits.SetDamageSet(-1); }
}
