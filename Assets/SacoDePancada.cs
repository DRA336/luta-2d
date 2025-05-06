using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacoDePancada : MonoBehaviour
{
    private int contadorDeHits = 0;
    private float danoTotalRecebido = 0f;

    // Método para receber dano
    public void ReceberDano(float dano)
    {
        contadorDeHits++;
        danoTotalRecebido += dano;

        Debug.Log("Saco de pancada foi atingido!");
        Debug.Log("Dano do golpe: " + dano);
        Debug.Log("Total de hits consecutivos: " + contadorDeHits);
        Debug.Log("Dano total acumulado: " + danoTotalRecebido);
    }

    // Exemplo de detecção por colisão
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            float dano = 10f; // ou valor variável
            ReceberDano(dano);
        }
    }

    // Você pode resetar o contador manualmente se quiser
    public void ResetarContador()
    {
        contadorDeHits = 0;
        danoTotalRecebido = 0f;
        Debug.Log("Contador de hits e dano total foram resetados.");
    }
}
