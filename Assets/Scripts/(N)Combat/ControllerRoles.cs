using UnityEngine;

public class ControllerRoles : MonoBehaviour
{
    [Header("Aponte os scripts corretos neste prefab")]
    public MonoBehaviour p1Controller;   // ex.: PlayerController2D
    public MonoBehaviour p2Controller;   // ex.: Player2Controller2D
    public MonoBehaviour botController;  // ex.: BotController2D_Pro

    // Desliga tudo primeiro
    public void DisableAll()
    {
        if (p1Controller)  p1Controller.enabled  = false;
        if (p2Controller)  p2Controller.enabled  = false;
        if (botController) botController.enabled = false;
    }

    // Liga conforme papel
    public void ApplyAsP1()
    {
        DisableAll();
        if (p1Controller) p1Controller.enabled = true;
    }

    public void ApplyAsP2Human()
    {
        DisableAll();
        if (p2Controller) p2Controller.enabled = true;
    }

    public void ApplyAsCPU()
    {
        DisableAll();
        if (botController) botController.enabled = true;
    }
}
