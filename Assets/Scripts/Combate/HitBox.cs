using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HitBox : MonoBehaviour
{
    [Header("Identificação")]
    public string nomeHitbox = "Ofensiva";

    [Header("Parâmetros")]
    public float damageOverride = -1f; // <0 usa GetDano() do dono
    public LayerMask validLayers;

    PlayerHits owner;
    Collider2D col;

    void Awake()
    {
        owner = GetComponentInParent<PlayerHits>();
        col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
        if (!owner) Debug.LogWarning("HitBox sem PlayerHits no pai.");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!enabled || owner == null) return;
        if (((1 << other.gameObject.layer) & validLayers) == 0) return;
        if (other.transform.root == transform.root) return; // ignora próprio corpo

        var targetDamageable = other.GetComponentInParent<IDamageable>();
        if (targetDamageable == null) return;

        var targetOwner = other.GetComponentInParent<PlayerHits>();
        BodyRegion region = BodyRegion.Torso;
        bool blocked = false;

        float amount = (damageOverride >= 0f ? damageOverride : owner.GetDano());

        // Defesa tem prioridade
        var defense = other.GetComponent<DefenseBox>();
        if (defense != null && targetOwner != null && targetOwner.IsDefending)
        {
            blocked = true;
            region = defense.region;
            amount *= defense.damageMultiplier; // chip/zero
        }
        else
        {
            var hurt = other.GetComponent<HurtBox>();
            if (hurt != null) region = hurt.region;
        }

        Vector2 point = other.ClosestPoint(transform.position);
        Vector2 normal = (other.transform.position - transform.position).normalized;

        var hit = new HitData
        {
            amount = amount,
            region = region,
            point = point,
            normal = normal,
            attacker = owner.transform,
            blocked = blocked
        };

        targetDamageable.ReceiveHit(hit);

        Debug.Log($"[{nomeHitbox}] {(blocked ? "BLOCK" : "HIT")} reg={region} dmg={amount:0.##} em {other.name}");
    }
}
