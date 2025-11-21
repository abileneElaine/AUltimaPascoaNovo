using UnityEngine;

public class AreaDano : MonoBehaviour
{
    public int dano = 1;
    public float intervaloDano = 0.8f;

    private float contador;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            contador -= Time.deltaTime;

            if (contador <= 0)
            {
                PlayerVida vida = other.GetComponent<PlayerVida>();
                if (vida != null)
                {
                    vida.TomarDano(dano);
                }

                contador = intervaloDano; // reinicia o tempo
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            contador = 0;
        }
    }
}
