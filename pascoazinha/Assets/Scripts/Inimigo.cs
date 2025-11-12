using UnityEngine;

public class Inimigo : MonoBehaviour
{
    [Header("Configurações Gerais")]
    public float vidaMaxima = 10f;
    public float vidaAtual;

    [Header("Componentes")]
    public Animator animator;
    public Rigidbody2D rb;
    public SpriteRenderer sprite;

    private bool congelado = false;
    private bool queimando = false;

    void Start()
    {
        vidaAtual = vidaMaxima;
        if (animator == null) animator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (sprite == null) sprite = GetComponent<SpriteRenderer>();
    }

    // Função chamada quando toma dano normal ou de fogo
    public virtual void TomarDano(float dano)
    {
        if (vidaAtual <= 0) return;

        vidaAtual -= dano;

        // Flash visual
        if (sprite != null)
        {
            sprite.color = Color.red;
            Invoke(nameof(ResetarCor), 0.15f);
        }

        // Se a vida zerar
        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    void ResetarCor()
    {
        if (sprite != null)
            sprite.color = Color.white;
    }

    // Função de morte genérica
    public virtual void Morrer()
    {
        if (animator != null)
            animator.SetTrigger("Morrer");

        Destroy(gameObject, 1f); // Destroi após 1 segundo
    }

    // Efeito de queimadura (dano ao longo do tempo)
    public void Queimar(float duracao, float danoPorSegundo)
    {
        if (queimando) return;
        StartCoroutine(EfeitoQueimadura(duracao, danoPorSegundo));
    }

    private System.Collections.IEnumerator EfeitoQueimadura(float duracao, float danoPorSegundo)
    {
        queimando = true;
        float tempo = 0f;

        while (tempo < duracao)
        {
            TomarDano(danoPorSegundo);
            yield return new WaitForSeconds(1f);
            tempo += 1f;
        }

        queimando = false;
    }

    // Efeito de congelamento (impede o movimento)
    public void Congelar(float duracao)
    {
        if (congelado) return;
        StartCoroutine(EfeitoCongelamento(duracao));
    }

    private System.Collections.IEnumerator EfeitoCongelamento(float duracao)
    {
        congelado = true;

        if (animator != null)
            animator.speed = 0f;
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(duracao);

        if (animator != null)
            animator.speed = 1f;

        congelado = false;
    }
}
