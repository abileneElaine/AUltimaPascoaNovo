using UnityEngine;

public class CoinAnimation : MonoBehaviour
{
    [Header("ID Único - IMPORTANTE")]
    public string coinID = ""; // ← Defina um ID único no Inspector

    [Header("Animação")]
    public float rotationSpeed = 100f;
    public float floatAmplitude = 0.2f;
    public float floatFrequency = 3f;

    [Header("Som")]
    public AudioClip somMoeda;

    public int Score;

    private Vector3 startPos;

    void Start()
    {
        // ✅ Verifica se já foi coletada
        if (GameManager.instance != null && GameManager.instance.MoedaJaColetada(coinID))
        {
            gameObject.SetActive(false);
            return;
        }

        startPos = transform.position;
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        float newY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = startPos + new Vector3(0, newY, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Toca som
            collision.GetComponent<AudioSource>()?.PlayOneShot(somMoeda);

            // ✅ Registra no GameManager
            if (GameManager.instance != null)
                GameManager.instance.RegistrarMoedaColetada(coinID);

            // ← NOVO: Chama o PlayerCoinCollector para adicionar moeda
            PlayerCoinCollector coinCollector = collision.GetComponent<PlayerCoinCollector>();
            if (coinCollector != null)
            {
                coinCollector.AdicionarMoeda();
                Debug.Log("💰 PlayerCoinCollector.AdicionarMoeda() chamado!");
            }
            else
            {
                Debug.LogWarning("⚠️ PlayerCoinCollector não encontrado no Player!");
            }

            Destroy(gameObject);
        }
    }
}