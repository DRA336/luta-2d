using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectP2UI : MonoBehaviour
{
    [Header("Sessão (persistência)")]
    public GameSession session;             // opcional; se vazio, acha sozinho

    [Header("Alvos visuais")]
    public TMP_Text roleLabel;              // o “P2”/“CPU” (TextMeshPro)
    public Image[] colorTargets;            // todas as Images do quadro a recolorir (bordas, header, fundo)

    [Header("Cores e rótulos")]
    [SerializeField] string hexHuman = "#E53939";   // vermelho P2
    [SerializeField] string hexCPU   = "#D1CCCC";   // cinza CPU
    public string labelHuman = "P2";
    public string labelCPU   = "CPU";

    Color p2Color, cpuColor;

    void Awake()
    {
        if (!session) session = FindAnyObjectByType<GameSession>();
        ColorUtility.TryParseHtmlString(hexHuman, out p2Color);
        ColorUtility.TryParseHtmlString(hexCPU, out cpuColor);
    }

    void OnEnable()
    {
        // aplica estado atual da sessão ao abrir a tela
        bool isHuman = session ? session.p2IsHuman : false;
        Apply(isHuman);
    }

    // Chamado pelo botão
    public void Toggle()
    {
        bool isHuman = !(session ? session.p2IsHuman : GameSession.I && GameSession.I.p2IsHuman);
    if (session) session.p2IsHuman = isHuman; else if (GameSession.I) GameSession.I.p2IsHuman = isHuman;
    Apply(isHuman);
    }

    // Também útil se quiser trocar via setinha/escolha
    public void SetHuman(bool isHuman)
    {
        if (session) session.p2IsHuman = isHuman;
        Apply(isHuman);
    }

    void Apply(bool isHuman)
    {
        // texto
        if (roleLabel)
        {
            roleLabel.text  = isHuman ? labelHuman : labelCPU;
            roleLabel.color = isHuman ? p2Color    : cpuColor;
        }

        // cores dos painéis
        if (colorTargets != null)
        {
            var c = isHuman ? p2Color : cpuColor;
            foreach (var img in colorTargets)
                if (img) img.color = c;
        }
    }
}
