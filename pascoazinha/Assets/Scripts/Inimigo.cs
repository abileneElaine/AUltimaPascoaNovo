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

    // ⚔️ Método protegido — só classes filhas (como Cobra) podem chamar
    protected virtual void LevarDano(int danoRecebido)
    {
        if (vidaAtual <= 0) return;

        vidaAtual -= danoRecebido;

        // Flash vermelho ao levar dano
        if (sprite != null)
        {
            sprite.color = Color.red;
            Invoke(nameof(ResetarCor), 0.15f);
        }

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

    protected virtual void Morrer()
    {
        if (animator != null)
            animator.SetTrigger("Morrer");

        Destroy(gameObject, 1f);
    }

    // 🔥 Efeito de queimadura
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
            LevarDano(Mathf.RoundToInt(danoPorSegundo));
            yield return new WaitForSeconds(1f);
            tempo += 1f;
        }

        queimando = false;
    }

    // ❄️ Efeito de congelamento
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
