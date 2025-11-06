using UnityEngine;

public class P2AutoMode : MonoBehaviour
{
    [Header("Aponte os dois scripts deste prefab (deixe ambos DESLIGADOS no prefab)")]
    public MonoBehaviour p2Controller;   // ex.: Player2Controller2D
    public MonoBehaviour botController;  // ex.: BotController2D_Pro

    void Awake()
    {
        // encontra a sessão
        var session = FindAnyObjectByType<GameSession>();
        bool p2Human = session != null && session.p2IsHuman;

        // desliga tudo e liga só o necessário
        if (p2Controller) p2Controller.enabled = false;
        if (botController) botController.enabled = false;

        if (p2Human) { if (p2Controller) p2Controller.enabled = true; }
        else         { if (botController) botController.enabled = true; }
    }
}
