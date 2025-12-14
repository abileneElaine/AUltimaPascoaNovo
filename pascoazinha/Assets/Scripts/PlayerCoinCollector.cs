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
        // Busca referência do PlayerVida se não tiver
        if (playerVida == null)
            playerVida = GetComponent<PlayerVida>();

        // Carrega moedas do GameManager
        if (GameManager.instance != null)
        {
            int moedasSalvas = GameManager.instance.ObterMoedasTotais();
            coins = moedasSalvas % 15;
            Debug.Log($"💰 Moedas carregadas: {coins} de {moedasSalvas} totais");
        }

        AtualizarUI();
    }

    // ✅ Chamado pela moeda coletada
    public void AdicionarMoeda()
    {
        coins++;
        Debug.Log($"💰 Moeda coletada! Total: {coins}/15");

        AtualizarUI();

        // A cada 15 moedas → +1 vida
        if (coins >= 15)
        {
            Debug.Log("🎉 15 MOEDAS! Dando +1 vida...");

            coins = 0;

            // Remove 15 moedas do total do GameManager
            if (GameManager.instance != null)
                GameManager.instance.RemoverMoedas(15);

            AtualizarUI();

            // ← CORRIGIDO: Verifica se playerVida existe antes de curar
            if (playerVida != null)
            {
                playerVida.Curar(1);
                Debug.Log("❤️ +1 vida concedida!");
            }
            else
            {
                Debug.LogWarning("⚠️ PlayerVida não encontrado! Não foi possível dar vida.");
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

    // Sincroniza com GameManager caso as moedas mudem externamente
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