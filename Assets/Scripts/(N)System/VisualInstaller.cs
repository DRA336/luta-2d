using UnityEngine;

public class VisualInstaller : MonoBehaviour
{
    // Mantido só para não quebrar referências antigas.
    // No plano A (2 prefabs por personagem), nada é instalado em runtime.
    [Header("Legacy (não usado no plano A)")]
    public Transform visualRoot;
    public PlayerHits hits;
    public CharacterMotor2D motor;
    public FighterActions actions;

    // Assinatura sem dependências para não quebrar chamadas antigas:
    public void Install(object _ = null) { /* no-op */ }

    void Reset()
    {
        if (!visualRoot) visualRoot = transform;
        if (!hits)   hits   = GetComponent<PlayerHits>();
        if (!motor)  motor  = GetComponent<CharacterMotor2D>();
        if (!actions)actions= GetComponent<FighterActions>();
    }
}
