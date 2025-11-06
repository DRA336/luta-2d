using UnityEngine;

[CreateAssetMenu(fileName = "CharSpec_", menuName = "Game/Character Spec")]
public class CharacterSpec : ScriptableObject
{
    [Header("Identidade / UI")]
    public string displayName;
    public Sprite portrait;
    public Color uiColor = Color.white;

    [Header("Prefabs (um para cada lado)")]
    public GameObject fighterPrefabP1;   // variante já configurada para Player 1
    public GameObject fighterPrefabP2;   // variante para Player 2 / CPU

    [Header("Overrides de Atributos (opcional)")]
    public float overrideMaxHP = -1f;    // -1 = ignora (usa valor do prefab)
    public int   overrideBaseDamage = -1;// -1 = ignora (usa valor do prefab)
}
