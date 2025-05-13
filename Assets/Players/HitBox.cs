using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public string nomeHitbox = "Padrão";
    private PlayerHits dadosDoPlayer;

    void Start()
    {
        dadosDoPlayer = GetComponentInParent<PlayerHits>();
        if (dadosDoPlayer == null)
        {
            Debug.LogWarning("Hitbox sem referência ao PlayerHits!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("SacoDePancada"))
        {
            float dano = dadosDoPlayer != null ? dadosDoPlayer.GetDano() : 0f;
            Debug.Log("Hitbox '" + nomeHitbox + "' atingiu " + other.name + " com " + dano + " de dano.");

            // Se o objeto atingido tiver um script com função ReceberDano:
            other.SendMessage("ReceberDano", dano, SendMessageOptions.DontRequireReceiver);
        }
    }
}
