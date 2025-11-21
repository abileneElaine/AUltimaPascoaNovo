using UnityEngine;

public class PlayerCoinCollector : MonoBehaviour
{
    public int coins = 0;
    public PlayerVida playerVida; // <-- Referência ao script de vida

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            coins++;
            Destroy(other.gameObject);

            Debug.Log("Moedas: " + coins);

            // A cada 15 moedas → +1 vida
            if (coins >= 15)
            {
                coins = 0;  // zera
                FindObjectOfType<PlayerVida>().Curar(1); // +1 vida
            }

        }
    }
}
