using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerHits))]
public class FighterActions : MonoBehaviour
{
    public PlayerHits hits;

    [Header("Índices dos Sets")]
    public int damageSet_Jab = 0;        // Set de hitbox ativa do jab (configure no PlayerHits)
    public int defenseSet_High = 0;      // Set de defesa (ex.: guarda alta)

    [Header("Janelas do Jab (segundos)")]
    public float startup = 0.06f;        // antes de ativar hitbox
    public float active = 0.08f;         // hitbox ligada
    public float recovery = 0.14f;       // após desligar hitbox

    [Header("Cooldowns")]
    public float jabCooldown = 0.25f;

    bool attacking;
    bool defending;
    float lastJabTime = -999f;

    void Reset()
    {
        hits = GetComponent<PlayerHits>();
    }

    public bool CanAct() => !attacking && !hits.IsKO;

    // ==== DEFESA ====
    public void StartDefense()
    {
        if (defending || hits.IsKO) return;
        defending = true;
        hits.StartDefense(defenseSet_High);
    }

    public void StopDefense()
    {
        if (!defending) return;
        defending = false;
        hits.StopDefense();
    }

    // ==== ATAQUE (Jab simples) ====
    public void TryJab()
    {
        if (attacking || hits.IsKO) return;
        if (Time.time - lastJabTime < jabCooldown) return;
        StartCoroutine(JabRoutine());
    }

    IEnumerator JabRoutine()
    {
        attacking = true;
        lastJabTime = Time.time;

        // startup
        hits.SetDamageSet(-1);
        yield return new WaitForSeconds(startup);

        // active
        hits.SetDamageSet(damageSet_Jab);
        yield return new WaitForSeconds(active);

        // recovery
        hits.SetDamageSet(-1);
        yield return new WaitForSeconds(recovery);

        attacking = false;
    }
}
