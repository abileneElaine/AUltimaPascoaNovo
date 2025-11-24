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

        // Procura qualquer objeto que tenha IDamageable (pai ou próprio)
        IDamageable dano = collision.collider.GetComponentInParent<IDamageable>();

        if (dano != null)
        {
            dano.TakeEnergy(1);
            Destroy(gameObject);
            return;
        }

        // Bateu em parede/superfície
        Destroy(gameObject);
    }
}