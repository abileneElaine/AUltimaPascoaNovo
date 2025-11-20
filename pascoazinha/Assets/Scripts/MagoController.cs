using UnityEngine;
using System.Collections;

public class MageController : MonoBehaviour
{
    [Header("Configurações de Ataque")]
    public float distanciaAtaque = 4f;
    public float tempoEntreAtaques = 3f;

    [Header("Referências")]
    public Animator animator;
    public GameObject prefabRaiz;
    public Transform pontoRaiz;
    public Transform player;

    private bool isDead = false;
    private bool atacando = false;
    private bool podeAtacar = true;
    public GameObject RaizAtaque;
    public Transform alvo; // ou "player"


    void Update()
    {
        if (isDead || player == null) return;

        float distancia = Vector2.Distance(transform.position, player.position);

        // Espelha o mago para o lado do player
        Vector3 escala = transform.localScale;
        escala.x = (player.position.x > transform.position.x)
            ? Mathf.Abs(escala.x)
            : -Mathf.Abs(escala.x);
        transform.localScale = escala;

        if (!atacando && distancia <= distanciaAtaque && podeAtacar)
        {
            StartCoroutine(Atacar());
        }
    }

    IEnumerator Atacar()
    {
        podeAtacar = false;
        atacando = true;
        animator.SetTrigger("Attack");

        // Espera o tempo de ataque terminar (controlado pela animação)
        yield return new WaitForSeconds(tempoEntreAtaques);
        atacando = false;
        podeAtacar = true;
    }

    // 🔹 ESTE MÉTODO É CHAMADO PELO EVENTO NA ANIMAÇÃO DE ATAQUE
    public void InvocarRaiz()
    {
        if (prefabRaiz != null && pontoRaiz != null)
        {
            GameObject raiz = Instantiate(prefabRaiz, pontoRaiz.position, Quaternion.identity);
            RaizAtaque r = raiz.GetComponent<RaizAtaque>();
            if (r != null) r.DefinirAlvo(player);
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        animator.SetTrigger("Die");
    }
}

public class RaizAtaque
{
    public void DefinirAlvo(Transform player)
    {
        throw new System.NotImplementedException();
    }
}