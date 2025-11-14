using UnityEngine;

public class CenouraColetavel : MonoBehaviour
{
    public string tipo; // "Normal", "Fogo", "Gelo" ou "Boomerang"
    private bool coletada = false;

    void Start()
    {
        coletada = false;
    }

    void Update()
    {
        if (coletada)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (coletada) return;

        if (other.CompareTag("Player"))
        {
            PlayerInventory inventario = other.GetComponent<PlayerInventory>();

            if (inventario != null)
            {
                inventario.AdicionarCenoura(tipo);
                coletada = true;
            }
        }
    }
}