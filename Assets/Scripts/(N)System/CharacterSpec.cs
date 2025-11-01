// Assets/Scripts/Characters/CharacterSpec.cs
using UnityEngine;

[CreateAssetMenu(fileName = "CharSpec_", menuName = "Game/Character Spec")]
public class CharacterSpec : ScriptableObject
{
    [Header("Identidade / UI")]
    public string displayName;
    public Sprite portrait;                       // para telas de seleção
    public Color uiColor = Color.white;

    [Header("Visual de Jogo")]
    public Sprite defaultSprite;                  // caso use SpriteRenderer direto
    public RuntimeAnimatorController animator;    // animator específico

    [Header("Atributos")]
    public float maxHP = 100f;
    public float moveSpeed = 6f;
    public float jumpForce = 12f;
    public int   baseDamage = 10;

    [Header("Sons (opcional)")]
    public AudioClip[] voiceClips;
}
    