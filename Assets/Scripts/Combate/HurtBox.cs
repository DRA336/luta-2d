using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HurtBox : MonoBehaviour
{
    public BodyRegion region = BodyRegion.Torso;
    // Dica: manter Collider2D como IsTrigger
}
