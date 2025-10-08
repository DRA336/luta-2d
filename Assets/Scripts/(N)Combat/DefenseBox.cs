using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DefenseBox : MonoBehaviour
{
    public BodyRegion region = BodyRegion.Torso;
    [Range(0f, 1f)] public float damageMultiplier = 0f; // 0 = bloqueio total
    // IsTrigger também
}
