using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerVida : MonoBehaviour, IDamageable
{
    [Header("Vida")]
    public int totalCoracoes = 3;
    public int coracoesAtuais;

    [Header("UI")]
    public Image[] coracoesUI;

    [Header("Estado")]
    public bool vivo = true;
    public bool invencivel = false;
    public float tempoInvencivel = 0.8f;

    private Animator animator;
    private SpriteRenderer sr;

    void Start()
    {
        // 🔥 FIX ABSOLUTO: impede corrotinas da cena anterior de sobreviverem
        StopAllCoroutines();

        vivo = true;
        invencivel = false;

        coracoesAtuais = totalCoracoes;

        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        AtualizarCoracoes();
    }

    // chamado pelo GameManager ao respawn
    public void OnRespawn()
    {
        StopAllCoroutines();        // 🔥 FIX ANTI-BUG
        vivo = true;
        invencivel = false;

        if (sr != null)
            sr.enabled = true;

        var mov = GetComponent<PlayerMovement>();
        if (mov != null)
            mov.enabled = true;

        AtualizarCoracoes();
    }

    // =============================
    //           DANO / CURA
    // =============================

    public void TakeEnergy(int dano)
    {
        TomarDano(dano);
    }

    public void TakeDamage(int dano)
    {
        TomarDano(dano);
    }

    public void Curar(int qtd)
    {
        if (!vivo) return;

        coracoesAtuais += qtd;
        if (coracoesAtuais > totalCoracoes)
            coracoesAtuais = totalCoracoes;

        AtualizarCoracoes();
    }

    public void TomarDano(int dano)
    {
        if (!vivo) return;
        if (invencivel) return;

        coracoesAtuais -= dano;

        if (coracoesAtuais < 0)
            coracoesAtuais = 0;

        AtualizarCoracoes();

        invencivel = true;
        StartCoroutine(InvencivelPiscando());

        if (coracoesAtuais <= 0)
        {
            Morrer();
        }
        else
        {
            if (animator != null)
                animator.Play("Hurt");
        }
    }

    private IEnumerator InvencivelPiscando()
    {
        float fim = Time.time + tempoInvencivel;

        while (Time.time < fim)
        {
            if (sr != null)
            {
                sr.enabled = false;
                yield return new WaitForSeconds(0.1f);
                sr.enabled = true;
                yield return new WaitForSeconds(0.1f);
            }
            else
                yield return null;
        }

        invencivel = false;
    }

    // =============================
    //            MORTE
    // =============================

    void Morrer()
    {
        if (!vivo) return;
        vivo = false;
        invencivel = false;

        Debug.Log("Player morreu!");

        if (animator != null)
            animator.Play("joreldeath");

        var mov = GetComponent<PlayerMovement>();
        if (mov != null)
            mov.enabled = false;

        StartCoroutine(GameOverDepoisDaAnimacao());
        
        
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator GameOverDepoisDaAnimacao()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("Game Over");
        // 🔥 nunca mais falha, pois invencibilidade anterior não existe mais
        GameManager.instance.MostrarTelaGameOver();
    }

    // =============================
    //           INTERFACE
    // =============================

    void AtualizarCoracoes()
    {
        for (int i = 0; i < coracoesUI.Length; i++)
            coracoesUI[i].enabled = (i < coracoesAtuais);
    }
}
