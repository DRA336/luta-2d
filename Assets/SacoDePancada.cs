using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacoDePancada : MonoBehaviour
{
    private int contadorDeHits = 0;
    private float danoTotalRecebido = 0f;
    private float tempoUltimoHit;
    public float tempoComboMaximo = 0.5f; // Tempo para manter o combo (Skullgirls)

    void Start()
    {
        Debug.Log("Saco de pancada ativado!");
    }

    // Método para receber dano
    public void ReceberDano(float dano)
    {
        float tempoAtual = Time.time;

        // Verifica se o tempo entre hits ultrapassou a janela de combo
        if (tempoAtual - tempoUltimoHit > tempoComboMaximo)
        {
            contadorDeHits = 0; // Reinicia combo
            danoTotalRecebido = 0f;
            Debug.Log("Combo encerrado!");
        }

        contadorDeHits++;
        danoTotalRecebido += dano;
        tempoUltimoHit = tempoAtual; // Atualiza o último tempo de hit

        Debug.Log("Saco de pancada atingido!");
        Debug.Log("Hit: " + contadorDeHits + " | Dano: " + dano + " | Dano total: " + danoTotalRecebido);
    }

    // Exemplo de detecção de hit (com uma hitbox)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            float dano = 10f; // Valor fixo de dano para teste
            ReceberDano(dano);
        }
    }

    // Método para resetar manualmente
    public void ResetarCombo()
    {
        contadorDeHits = 0;
        danoTotalRecebido = 0f;
        Debug.Log("Combo manualmente resetado.");
    }
}
