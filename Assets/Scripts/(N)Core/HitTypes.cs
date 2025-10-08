using UnityEngine;

public enum BodyRegion { Head, Torso, ArmL, ArmR, LegL, LegR, Full }

public struct HitData
{
    public float amount;
    public BodyRegion region;
    public Vector2 point;
    public Vector2 normal;
    public Transform attacker;
    public bool blocked;
}

public interface IDamageable
{
    void ReceiveHit(HitData hit);
}
