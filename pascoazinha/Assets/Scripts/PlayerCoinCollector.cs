using UnityEngine;

public class PlayerCoinCollector : MonoBehaviour
{
    public int coins = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            coins++;                              // Soma moeda
            Destroy(other.gameObject);            // Some com a moeda
            Debug.Log("Moedas: " + coins);        // Teste no Console
        }
    }
}