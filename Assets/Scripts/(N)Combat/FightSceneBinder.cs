// Assets/Scripts/Characters/FightSceneBinder.cs
using UnityEngine;

public class FightSceneBinder : MonoBehaviour
{
    public GameSession session;
    [Header("Appliers em cena")]
    public CharacterApplier player1;
    public CharacterApplier player2OrBot;

    void Awake()
    {
        if (!session) session = FindAnyObjectByType<GameSession>();
    }

    void Start()
    {
        if (!session) return;

        var p1 = session.GetP1Spec();
        var p2 = session.GetP2Spec();

        if (player1)      player1.Apply(p1);
        if (player2OrBot) player2OrBot.Apply(p2);

        // (opcional) pintar HUDs com a cor do personagem
        // ex.: se tiver HealthBarCombined com frameImage, aplique spec.uiColor.
    }
}
