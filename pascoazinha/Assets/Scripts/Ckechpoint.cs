using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Salva posição no GameManager
            GameManager.instance.DefinirCheckpoint(transform.position);
            Debug.Log("Checkpoint salvo: " + transform.position);
        }
    }
}