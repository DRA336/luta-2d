using UnityEngine;
using UnityEngine.UI;   // para Image
using TMPro;            // <<— TextMeshPro

public class CharacterSelectUI : MonoBehaviour
{
    public GameSession session;

    [Header("P1")]
    public Image p1Portrait;
    public TMP_Text p1Name;   // <<— era Text
    int p1Index = 0;

    [Header("P2/Bot (opcional)")]
    public bool letP2Choose = false;
    public Image p2Portrait;
    public TMP_Text p2Name;   // <<— era Text
    int p2Index = 0;

    void Awake()
    {
        if (!session) session = FindAnyObjectByType<GameSession>();
    }

    void OnEnable()
    {
        p1Index = session.p1CharIndex;
        p2Index = session.p2CharIndex;
        Refresh();
    }

    void Refresh()
    {
        if (session == null || session.characterPortraits == null || session.characterPortraits.Length == 0)
            return;

        // P1
        if (p1Portrait) p1Portrait.sprite = session.characterPortraits[p1Index];
        if (p1Name) p1Name.text = session.characterNames[p1Index];

        // P2 (se habilitado)
        if (letP2Choose && p2Portrait && p2Name)
        {
            p2Portrait.sprite = session.characterPortraits[p2Index];
            p2Name.text = session.characterNames[p2Index];
        }
    }

    // Navegação P1
    public void P1Prev() { p1Index = (p1Index - 1 + session.characterNames.Length) % session.characterNames.Length; Refresh(); }
    public void P1Next() { p1Index = (p1Index + 1) % session.characterNames.Length; Refresh(); }

    // Navegação P2 (opcional)
    public void P2Prev() { if (!letP2Choose) return; p2Index = (p2Index - 1 + session.characterNames.Length) % session.characterNames.Length; Refresh(); }
    public void P2Next() { if (!letP2Choose) return; p2Index = (p2Index + 1) % session.characterNames.Length; Refresh(); }

    // Confirmar seleção
    public void Confirm()
    {
        session.SetP1(p1Index);
        if (letP2Choose) session.SetP2(p2Index);
        session.GoStageSelect();
    }

    public void Back() => session.GoMainMenu();
}
