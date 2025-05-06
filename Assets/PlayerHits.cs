using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHits : MonoBehaviour
{
   [Header("Dano do personagem")]
    public float danoBase = 10f;

    [Header("Lista de hitboxes")]
    public List<GameObject> hitboxes;

    void Start()
    {
        Debug.Log("Hitboxes registradas para " + gameObject.name + ":");

        foreach (GameObject hitbox in hitboxes)
        {
            Debug.Log("- " + hitbox.name);
        }

        Debug.Log("Dano base atual: " + danoBase);
    }

    // Função para obter o dano
    public float GetDano()
    {
        return danoBase;
    }

    // (Opcional) Função para ativar uma hitbox específica por nome
    public void AtivarHitbox(string nome)
    {
        foreach (GameObject hitbox in hitboxes)
        {
            if (hitbox.name == nome)
            {
                hitbox.SetActive(true);
                Debug.Log("Hitbox " + nome + " ativada.");
                return;
            }
        }

        Debug.LogWarning("Hitbox '" + nome + "' não encontrada.");
    }

    // (Opcional) Função para desativar todas as hitboxes
    public void DesativarTodasHitboxes()
    {
        foreach (GameObject hitbox in hitboxes)
        {
            hitbox.SetActive(false);
        }

        Debug.Log("Todas as hitboxes foram desativadas.");
    }

}
