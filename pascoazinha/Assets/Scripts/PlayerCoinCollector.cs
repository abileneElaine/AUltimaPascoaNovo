using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerCoinCollector : MonoBehaviour
{
    public int coins = 0;
    public PlayerVida playerVida;        // Referência ao script de vida

    [Header("UI de moedas")]
    public TextMeshProUGUI textoMoedas; // Número de moedas
    public Image moedaImage;             // Ícone da moeda

    private void Start()
    {
        AtualizarUI();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            coins++;
            Destroy(other.gameObject);

            AtualizarUI();

            // A cada 15 moedas → +1 vida
            if (coins >= 15)
            {
                coins = 0;
                AtualizarUI();
                playerVida.Curar(1);
            }
        }
    }

    private void AtualizarUI()
    {
        if (textoMoedas != null)
            textoMoedas.text = coins.ToString();

        if (moedaImage != null)
            moedaImage.enabled = true; // garante que a imagem fique visível
    }
}