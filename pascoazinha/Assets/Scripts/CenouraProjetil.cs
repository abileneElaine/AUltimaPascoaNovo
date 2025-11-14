using UnityEngine;

public class CenouraProjetil : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Ignora player
        if (collision.collider.CompareTag("Player"))
            return;

        // Acertou inimigo
        if (collision.collider.CompareTag("Enemy"))
        {
            // Pega o IDamageable no inimigo OU no pai dele
            IDamageable dano = collision.collider.GetComponentInParent<IDamageable>();

            if (dano != null)
                dano.TakeEnergy(1);  // <-- chama sua função correta

            Destroy(gameObject);
            return;
        }

        // Bateu em outra coisa
        Destroy(gameObject);
    }
}