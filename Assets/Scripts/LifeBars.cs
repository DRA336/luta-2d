using UnityEngine;
using UnityEngine.UI;

public class LifeBars : MonoBehaviour
{
    [Header("Barras de Vida")]
    public Image lifeBarL;   // Barra de vida do jogador 1
    public Image lifeBarR;   // Barra de vida do jogador 2/CPU

    [Header("Valores de Vida")]
    public float maxLife = 100f;
    private float currentLifeL;
    private float currentLifeR;

    void Start()
    {
        // Inicializa a vida cheia para ambos os jogadores
        currentLifeL = maxLife;
        currentLifeR = maxLife;

        // Atualiza a UI das barras de vida no início
        UpdateLifeBar(lifeBarL, currentLifeL);
        UpdateLifeBar(lifeBarR, currentLifeR);
    }

    void Update()
    {
        // Teste de dano (remover depois)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TakeDamage(true, 10f);  // Jogador 1 recebe dano
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            TakeDamage(false, 10f); // Jogador 2/CPU recebe dano
        }
    }

    // Função para atualizar a barra de vida
    void UpdateLifeBar(Image lifeBar, float currentLife)
    {
        float lifePercent = Mathf.Clamp(currentLife / maxLife, 0, 1);
        lifeBar.fillAmount = lifePercent;

        // Atualiza a cor conforme a vida diminui
        if (lifePercent > 0.5f)
        {
            lifeBar.color = Color.green;
        }
        else if (lifePercent > 0.2f)
        {
            lifeBar.color = Color.yellow;
        }
        else
        {
            lifeBar.color = Color.red;
        }
    }

    // Função para receber dano
    public void TakeDamage(bool isPlayer1, float damage)
    {
        if (isPlayer1)
        {
            currentLifeL = Mathf.Max(currentLifeL - damage, 0);
            UpdateLifeBar(lifeBarL, currentLifeL);
        }
        else
        {
            currentLifeR = Mathf.Max(currentLifeR - damage, 0);
            UpdateLifeBar(lifeBarR, currentLifeR);
        }

        Debug.Log((isPlayer1 ? "Jogador 1" : "Jogador 2/CPU") + " recebeu " + damage + " de dano.");
    }
}
