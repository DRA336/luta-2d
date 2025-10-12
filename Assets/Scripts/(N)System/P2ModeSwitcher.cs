// P2ModeSwitcher.cs (versão robusta)
using UnityEngine;
using UnityEngine.UI; // se usar Graphic para trocar cor
using TMPro;

public class P2ModeSwitcher : MonoBehaviour
{
    [Header("Sessão (opcional)")]
    public GameSession session;

    [Header("Controladores (no MESMO personagem)")]
    public MonoBehaviour botController;       // BotController2D_Pro
    public MonoBehaviour player2Controller;   // Player2Controller2D

    [Header("UI (opcional)")]
    public TMP_Text roleLabel;     // "CPU"/"P2"
    public Graphic[] colorTargets;
    [SerializeField] string hexP2 = "#E53939";
    [SerializeField] string hexCPU = "#D1CCCC";

    Color p2Color, cpuColor;

    void Awake()
    {
        if (!session) session = GameSession.I ? GameSession.I : FindAnyObjectByType<GameSession>();
        ColorUtility.TryParseHtmlString(hexP2, out p2Color);
        ColorUtility.TryParseHtmlString(hexCPU, out cpuColor);

        // Desliga tudo primeiro para evitar ficarem os dois ON
        HardDisableBoth();

        // Aplica de acordo com o que veio da CharacterSelect
        bool human = session ? session.p2IsHuman : false;
        Apply(human);
    }

    void HardDisableBoth(){
        if (botController)     botController.enabled = false;
        if (player2Controller) player2Controller.enabled = false;
    }

    public void ToggleMode(){
        bool human = !(session ? session.p2IsHuman : (player2Controller && player2Controller.enabled));
        if (session) session.p2IsHuman = human;
        Apply(human);
    }

    public void Apply(bool human)
    {
        if (botController)     botController.enabled = !human;
        if (player2Controller) player2Controller.enabled = human;

        if (roleLabel) roleLabel.text = human ? "P2" : "CPU";
        if (colorTargets != null){
            var c = human ? p2Color : cpuColor;
            foreach (var g in colorTargets) if (g) g.color = c;
        }
    }
}
