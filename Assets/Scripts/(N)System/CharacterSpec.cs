// Assets/Scripts/Characters/CharacterSpec.cs
using UnityEngine;

[CreateAssetMenu(fileName = "CharSpec_", menuName = "Game/Character Spec")]
public class CharacterSpec : ScriptableObject
{
    [Header("Identidade / UI")]
    public string displayName;
    public Sprite portrait;                       // para telas de seleção
    public Color uiColor = Color.white;

    [Header("Prefab do lutador")]
    public GameObject fighterPrefab;   // <<< NEW: prefab do personagem pronto

    // (Opcional) stats “extras” se você quiser centralizar no Spec:
    public float overrideMaxHP = -1f;  // -1 = usa do prefab
    public int   overrideBaseDamage = -1;

    [Header("Sons (opcional)")]
    public AudioClip[] voiceClips;
}
    