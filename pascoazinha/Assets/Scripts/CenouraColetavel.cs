using UnityEngine;

public class CenouraColetavel : MonoBehaviour
{
    public string tipo; // "Normal", "Fogo", "Gelo" ou "Boomerang"

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventario = other.GetComponent<PlayerInventory>();
            if (inventario != null)
            {
                inventario.AdicionarCenoura(tipo);
                Destroy(gameObject); // Remove a cenoura da cena
            }
        }
    }
}