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
        // Carrega moedas do GameManager
        if (GameManager.instance != null)
        {
            int moedasSalvas = GameManager.instance.ObterMoedasTotais();
            coins = moedasSalvas % 15;

            Debug.Log($"💰 Moedas carregadas: {coins} de {moedasSalvas} totais");
        }

        AtualizarUI();
    }

    // ✅ Este método é chamado automaticamente pela moeda via GameManager
    public void AdicionarMoeda()
    {
        coins++;
        AtualizarUI();

        // A cada 15 moedas → +1 vida
        if (coins >= 15)
        {
            coins = 0;

            if (GameManager.instance != null)
                GameManager.instance.RemoverMoedas(15);

            AtualizarUI();
            playerVida.Curar(1);

            Debug.Log("❤️ +1 vida! Moedas resetadas");
        }
    }

    private void AtualizarUI()
    {
        if (textoMoedas != null)
            textoMoedas.text = coins.ToString();

        if (moedaImage != null)
            moedaImage.enabled = true;
    }

    // Método público para sincronizar com GameManager
    void Update()
    {
        if (GameManager.instance != null)
        {
            int totalMoedas = GameManager.instance.ObterMoedasTotais();
            int moedasAtuais = totalMoedas % 15;

            if (moedasAtuais != coins)
            {
                coins = moedasAtuais;
                AtualizarUI();
            }
        }
    }
}