using UnityEngine;

public class CenouraProjetil : MonoBehaviour
{
    [Header("Configurações de Dano")]
    public int dano = 1;

    [Header("Tipo e Efeitos")]
    public string tipo = "Normal";
    public GameObject efeitoFogo;
    public GameObject efeitoGelo;

    [Header("Configurações")]
    public float speed = 10f;
    public float lifetime = 3f;

    private bool jaColidiu = false;

    void Start()
    {
        // ✅ DEBUG: Mostra o dano ao criar a cenoura
        Debug.Log($"🥕 Cenoura {tipo} criada com DANO = {dano}");

        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (jaColidiu) return;

        // ✅ DEBUG: Mostra com o que colidiu
        Debug.Log($"🥕 Cenoura {tipo} colidiu com: {collision.collider.name} (Tag: {collision.collider.tag})");

        // Ignora player
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Ignorou colisão com Player");
            return;
        }

        // Procura IDamageable
        IDamageable alvo = collision.collider.GetComponentInParent<IDamageable>();
        if (alvo != null)
        {
            // ✅ DEBUG: Mostra o dano que VAI aplicar
            Debug.Log($"🥕 Cenoura {tipo} VAI APLICAR {dano} de dano!");

            alvo.TakeEnergy(dano);

            Debug.Log($"🥕 Cenoura {tipo} APLICOU {dano} de dano!");

            AplicarEfeito(collision.transform.position);

            jaColidiu = true;
            Destroy(gameObject);
            return;
        }
        else
        {
            Debug.Log($"⚠️ Não encontrou IDamageable em {collision.collider.name}");
        }

        // Bateu em parede/chão
        Debug.Log("Cenoura destruída ao bater em superfície");
        jaColidiu = true;
        Destroy(gameObject);
    }

    void AplicarEfeito(Vector3 posicao)
    {
        GameObject efeito = null;

        switch (tipo)
        {
            case "Fogo":
                if (efeitoFogo != null)
                    efeito = Instantiate(efeitoFogo, posicao, Quaternion.identity);
                break;

            case "Gelo":
                if (efeitoGelo != null)
                    efeito = Instantiate(efeitoGelo, posicao, Quaternion.identity);
                break;
        }

        if (efeito != null)
            Destroy(efeito, 1f);
    }
}