using UnityEngine;

public class CoinAnimation : MonoBehaviour
{
    public float rotationSpeed = 100f;
    public float floatAmplitude = 0.2f;
    public float floatFrequency = 3f;

    public int Score; //pontuação da moeda

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Gira no eixo Y
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // Flutua para cima e para baixo
        float newY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = startPos + new Vector3(0, newY, 0);
    }
}