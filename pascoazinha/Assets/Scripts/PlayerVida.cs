using UnityEngine;
using UnityEngine.UI;

public class PlayerVida : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public int vidaMaxima = 50;
    public int vidaAtual;

    [Header("Interface (opcional)")]
    public Slider barraDeVida; // arraste um Slider do Canvas, se quiser

    private Animator animator;

    void Start()
    {
        vidaAtual = vidaMaxima;
        animator = GetComponent<Animator>();
        AtualizarBarra();
    }

    public void TomarDano(int dano)
    {
        vidaAtual -= dano;
        if (vidaAtual < 0) vidaAtual = 0;

        Debug.Log("Jogador tomou " + dano + " de dano. Vida restante: " + vidaAtual);
        AtualizarBarra();

        if (vidaAtual <= 0)
            Morrer();
        else if (animator != null)
            animator.Play("Hurt"); // se tiver animação de dano
    }

    void Morrer()
    {
        Debug.Log("Jogador morreu!");
        if (animator != null)
            animator.Play("Morrer");

        // Exemplo: pode desativar movimento
        var movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;
    }

    void AtualizarBarra()
    {
        if (barraDeVida != null)
            barraDeVida.value = (float)vidaAtual / vidaMaxima;
    }

    public void TakeDamage(int dano)
    {
        throw new System.NotImplementedException();
    }
}