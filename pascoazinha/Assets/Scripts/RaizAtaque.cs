using UnityEngine;

public class RaizAtaque : MonoBehaviour
{
    public float velocidade = 4f;
    public int dano = 20;
    public float duracao = 3f;

    private Transform alvo;
    private bool atingiu = false;
    private float alturaFixa;

    public void DefinirAlvo(Transform novoAlvo)
    {
        alvo = novoAlvo;
        alturaFixa = transform.position.y; // fixa a altura no momento da criação
    }

    void Start()
    {
        Destroy(gameObject, duracao);
    }

    void Update()
    {
        if (alvo == null || atingiu) return;

        // Move apenas no eixo X (sem subir/descer)
        Vector3 posicaoAtual = transform.position;
        Vector3 destino = new Vector3(alvo.position.x, alturaFixa, posicaoAtual.z);

        transform.position = Vector3.MoveTowards(posicaoAtual, destino, velocidade * Time.deltaTime);

        // Verifica se chegou perto o suficiente para causar dano
        if (Vector2.Distance(transform.position, destino) < 0.2f)
        {
            PlayerHealth vida = alvo.GetComponent<PlayerHealth>();
            if (vida != null)
                vida.TakeDamage(dano);

            atingiu = true;
            Destroy(gameObject);
        }
    }
}