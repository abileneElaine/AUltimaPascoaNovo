using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerVida : MonoBehaviour, IDamageable
{
    public int totalCoracoes = 3;
    public int coracoesAtuais;

    public Image[] coracoesUI;

    private Animator animator;
    private bool vivo = true;

    private bool invencivel = false;
    public float tempoInvencivel = 0.8f;

    void Start()
    {
        coracoesAtuais = totalCoracoes;
        animator = GetComponent<Animator>();
        AtualizarCoracoes();
    }

    public void TakeEnergy(int dano)
    {
        TomarDano(dano);
    }
    public void Curar(int quantidade)
    {
        if (!vivo) return;

        coracoesAtuais += quantidade;

        if (coracoesAtuais > totalCoracoes)
            coracoesAtuais = totalCoracoes;

        AtualizarCoracoes();
    }

    public void TomarDano(int dano)
    {
        if (!vivo) return;
        if (invencivel) return;

        coracoesAtuais -= dano;
        invencivel = true;

        StartCoroutine(InvencivelPiscando());
       
        if (coracoesAtuais < 0) 
            coracoesAtuais = 0;

        AtualizarCoracoes();

        if (coracoesAtuais <= 0)
        {
            Morrer();
        }
        else if (animator != null)
        {
            animator.Play("Hurt");
        }
    }
    
    private IEnumerator InvencivelPiscando()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        float fim = Time.time + tempoInvencivel;

        while (Time.time < fim)
        {
            sr.enabled = false;
            yield return new WaitForSeconds(0.1f);
            sr.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        invencivel = false;
    }

    void AtualizarCoracoes()
    {
        for (int i = 0; i < coracoesUI.Length; i++)
            coracoesUI[i].enabled = (i < coracoesAtuais);
    }

    void Morrer()
    {
        vivo = false;
        Debug.Log("Player morreu!");

        if (animator != null)
            animator.Play("joreldeath");

        var mov = GetComponent<PlayerMovement>();
        if (mov != null)
            mov.enabled = false;

        StartCoroutine(GameOverDepoisDaAnimacao());
    }

    private IEnumerator GameOverDepoisDaAnimacao()
    {
        yield return new WaitForSeconds(1f);
        GameManager.instance.MostrarTelaGameOver(); // <- CORRETO
    }

    public void TakeDamage(int dano)
    {
        throw new System.NotImplementedException();
    }
}
