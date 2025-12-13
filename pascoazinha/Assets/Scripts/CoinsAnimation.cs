using UnityEngine;

public class CoinAnimation : MonoBehaviour
{
    [Header("ID Único - IMPORTANTE")]
    public string coinID = ""; // ← Defina um ID único no Inspector

    public float rotationSpeed = 100f;
    public float floatAmplitude = 0.2f;
    public float floatFrequency = 3f;

    public int Score;
    public AudioClip somMoeda;

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
            collision.GetComponent<AudioSource>()?.PlayOneShot(somMoeda);

            // ✅ Registra no GameManager
            if (GameManager.instance != null)
                GameManager.instance.RegistrarMoedaColetada(coinID);

            Destroy(gameObject);
        }
    }
}