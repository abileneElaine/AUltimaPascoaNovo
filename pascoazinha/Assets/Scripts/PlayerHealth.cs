using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuração de Vida")]
    public int vidaMaxima = 100;
    private int vidaAtual;

    [Header("Referências")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("Feedback de dano")]
    public float tempoPiscando = 0.1f;
    public int vezesPiscando = 4;

    private bool estaMorto = false;

    void Start()
    {
        vidaAtual = vidaMaxima;

        if (animator == null)
            animator = GetComponent<Animator>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void TakeDamage(int damage)
    {
        // diminui a vida, animações etc.
    }


    public void LevarDano(int dano)
    {
        if (estaMorto) return;

        vidaAtual -= dano;

        StartCoroutine(Piscar());

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    IEnumerator Piscar()
    {
        for (int i = 0; i < vezesPiscando; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(tempoPiscando);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(tempoPiscando);
        }
    }

    void Morrer()
    {
        estaMorto = true;
        animator.SetTrigger("Death");
        var movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        Debug.Log("O player morreu!");
    }
}