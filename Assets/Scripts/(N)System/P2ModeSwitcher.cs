using UnityEngine;
using UnityEngine.UI;

public class P2ModeSwitcher : MonoBehaviour
{
    [Header("Sessão (opcional)")]
    public GameSession session;

    [Header("Controladores")]
    public BotController2D_Pro botController;          // IA
    public Player2Controller2D player2Controller;  // Humano

    [Header("Cores da UI (opcional)")]
    public Graphic[] colorTargets; // Images/Text a colorir
    [SerializeField] string hexP2 = "#E53939";
    [SerializeField] string hexCPU = "#D1CCCC";

    void Awake(){
        if (!session) session = FindAnyObjectByType<GameSession>();
        Apply(session ? session.p2IsHuman : false);
    }

    public void ToggleMode(){
        if (session) session.ToggleP2Human();
        Apply(session ? session.p2IsHuman : !botController.enabled);
    }

    public void Apply(bool p2Human){
        if (botController)     botController.enabled = !p2Human;
        if (player2Controller) player2Controller.enabled = p2Human;

        // Cor
        if (colorTargets != null && colorTargets.Length > 0){
            if (ColorUtility.TryParseHtmlString(p2Human ? hexP2 : hexCPU, out var c)){
                foreach (var g in colorTargets) if (g) g.color = c;
            }
        }
    }
}
