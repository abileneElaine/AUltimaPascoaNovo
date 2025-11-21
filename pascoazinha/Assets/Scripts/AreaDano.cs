using UnityEngine;

public class DamageArea : MonoBehaviour
{
    public int danoPorSegundo = 1;
    public float intervaloDano = 1f;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerVida vida = other.GetComponent<PlayerVida>();
            if (vida != null)
            {
                vida.TomarDano(danoPorSegundo);
            }
        }
    }
}
