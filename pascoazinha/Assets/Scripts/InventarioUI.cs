using UnityEngine;
using UnityEngine.UI;
using TMPro; // ? Importa TextMeshPro

public class InventarioUI : MonoBehaviour
{
    [Header("Referências")]
    public PlayerInventory inventario;

    [Header("Textos de Quantidade (TextMeshPro)")]
    public TextMeshProUGUI textoNormal;
    public TextMeshProUGUI textoFogo;
    public TextMeshProUGUI textoGelo;
    public TextMeshProUGUI textoBoomerang;

    [Header("Imagens dos Ícones (Opcional)")]
    public Image iconNormal;
    public Image iconFogo;
    public Image iconGelo;
    public Image iconBoomerang;

    [Header("Cores quando está vazio")]
    public Color corVazia = new Color(1f, 1f, 1f, 0.3f); // Branco transparente
    public Color corCheia = Color.white;

    void Start()
    {
        // Se não tiver referência, busca no player
        if (inventario == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                inventario = player.GetComponent<PlayerInventory>();
        }
    }

    void Update()
    {
        if (inventario == null) return;

        AtualizarUI();
    }

    void AtualizarUI()
    {
        // Atualiza texto Normal
        if (textoNormal != null)
        {
            textoNormal.text = inventario.normalCount.ToString();
            textoNormal.color = (inventario.normalCount > 0) ? corCheia : corVazia;
        }
        if (iconNormal != null)
            iconNormal.color = (inventario.normalCount > 0) ? corCheia : corVazia;

        // Atualiza texto Fogo
        if (textoFogo != null)
        {
            textoFogo.text = inventario.fogoCount.ToString();
            textoFogo.color = (inventario.fogoCount > 0) ? corCheia : corVazia;
        }
        if (iconFogo != null)
            iconFogo.color = (inventario.fogoCount > 0) ? corCheia : corVazia;

        // Atualiza texto Gelo
        if (textoGelo != null)
        {
            textoGelo.text = inventario.geloCount.ToString();
            textoGelo.color = (inventario.geloCount > 0) ? corCheia : corVazia;
        }
        if (iconGelo != null)
            iconGelo.color = (inventario.geloCount > 0) ? corCheia : corVazia;

        // Atualiza texto Boomerang
        if (textoBoomerang != null)
        {
            textoBoomerang.text = inventario.boomerangCount.ToString();
            textoBoomerang.color = (inventario.boomerangCount > 0) ? corCheia : corVazia;
        }
        if (iconBoomerang != null)
            iconBoomerang.color = (inventario.boomerangCount > 0) ? corCheia : corVazia;
    }
}