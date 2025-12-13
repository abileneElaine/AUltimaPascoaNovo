using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerCoinCollector : MonoBehaviour
{
    public int coins = 0;
    public PlayerVida playerVida;

    [Header("UI de moedas")]
    public TextMeshProUGUI textoMoedas;
    public Image moedaImage;

    private void Start()
    {
        // ✅ Carrega moedas salvas no GameManager
        if (GameManager.instance != null)
        {
            int moedasSalvas = GameManager.instance.ObterMoedasTotais();
            coins = moedasSalvas % 15; // Pega apenas o resto (0-14)

            Debug.Log($"💰 Moedas carregadas: {coins} (Total: {moedasSalvas})");
        }

        AtualizarUI();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            coins++;
            Destroy(other.gameObject);

            // ✅ Salva no GameManager também
            if (GameManager.instance != null)
                GameManager.instance.AdicionarMoeda();

            AtualizarUI();

            // A cada 15 moedas → +1 vida
            if (coins >= 15)
            {
                coins = 0;

                // ✅ Remove as 15 moedas do GameManager também
                if (GameManager.instance != null)
                    GameManager.instance.RemoverMoedas(15);

                AtualizarUI();
                playerVida.Curar(1);

                Debug.Log("❤️ Ganhou +1 vida! Moedas resetadas para 0");
            }
        }
    }

    private void AtualizarUI()
    {
        if (textoMoedas != null)
            textoMoedas.text = coins.ToString();

        if (moedaImage != null)
            moedaImage.enabled = true;
    }
}