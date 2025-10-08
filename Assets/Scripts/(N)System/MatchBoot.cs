using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MatchBoot : MonoBehaviour
{
    public TMP_Text p1Label;
    public TMP_Text p2Label;
    public Image stageBG; // opcional, se seu fundo for uma Image

    void Start()
    {
        var s = FindAnyObjectByType<GameSession>();
        if (!s) return;

        if (p1Label) p1Label.text = s.characterNames[s.p1CharIndex];
        if (p2Label) p2Label.text = s.characterNames[s.p2CharIndex];

        if (stageBG && s.stageThumbs.Length > 0)
            stageBG.sprite = s.stageThumbs[s.stageIndex];

        // Aqui, se precisar, posicione P1/P2 prefabs/conjuntos conforme o characterIndex.
        // Como seu protótipo já tem Player e Bot na cena, você pode só trocar cor/nome/portrait na UI por enquanto.
    }
}
