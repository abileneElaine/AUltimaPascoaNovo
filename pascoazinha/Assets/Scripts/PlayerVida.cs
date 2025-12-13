using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerVida : MonoBehaviour, IDamageable
{
    [Header("Sistema de Vida")]
    public int totalCoracoes = 3;
    public int coracoesAtuais;
    public Image[] coracoesUI;

    [Header("Sistema de Vidas (Mortes)")]
    public int vidasTotais = 3;
    private int vidasRestantes;
    public Text textoVidas;

    [Header("Invencibilidade")]
    public float tempoInvencivel = 0.8f;
    private bool invencivel = false;

    private Animator animator;
    private bool vivo = true;
    private bool estaRenascendo = false;

    private void Start()
    {
        animator = GetComponent<Animator>();

        // Inicializa vidas
        vidasRestantes = vidasTotais;
        AtualizarTextoVidas();

        // Inicializa corações
        coracoesAtuais = totalCoracoes;
        AtualizarCoracoes();

        // ← CORRIGIDO: Só vai pro checkpoint se ele existir
        // Se não existir, NÃO define novo checkpoint aqui
        if (GameManager.instance != null && GameManager.instance.TemCheckpoint())
        {
            transform.position = GameManager.instance.ObterCheckpoint();
            Debug.Log("Player voltou ao checkpoint: " + transform.position);
        }
        // Removido o else que definia checkpoint automaticamente
        // O checkpoint inicial será definido pelo PlayerMovement.Awake()
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

        Debug.Log($"❤️ Curado! Vida atual: {coracoesAtuais}/{totalCoracoes}");
    }

    public void TomarDano(int dano)
    {
        if (!vivo) return;
        if (invencivel) return;
        if (estaRenascendo) return;

        coracoesAtuais -= dano;
        invencivel = true;

        StartCoroutine(InvencivelPiscando());

        if (coracoesAtuais < 0)
            coracoesAtuais = 0;

        AtualizarCoracoes();

        Debug.Log($"💔 Tomou dano! Vida atual: {coracoesAtuais}/{totalCoracoes}");

        if (coracoesAtuais <= 0)
        {
            MorrerERenascer();
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

    void AtualizarTextoVidas()
    {
        if (textoVidas != null)
            textoVidas.text = $"Vidas: {vidasRestantes}";
    }

    void MorrerERenascer()
    {
        if (estaRenascendo) return;

        estaRenascendo = true;
        vivo = false;

        vidasRestantes--;
        AtualizarTextoVidas();

        Debug.Log($"💀 Player morreu! Vidas restantes: {vidasRestantes}/{vidasTotais}");

        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isJumping", false);
            animator.Play("Death");
        }

        var mov = GetComponent<PlayerMovement>();
        if (mov != null)
            mov.enabled = false;

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        StartCoroutine(ProcessarRespawn());
    }

    private IEnumerator ProcessarRespawn()
    {
        yield return new WaitForSeconds(1f);

        if (vidasRestantes > 0)
        {
            // Respawn no checkpoint
            if (GameManager.instance != null && GameManager.instance.TemCheckpoint())
            {
                Debug.Log($"✅ Respawnando no checkpoint! Vidas: {vidasRestantes}");

                transform.position = GameManager.instance.ObterCheckpoint();

                coracoesAtuais = totalCoracoes;
                AtualizarCoracoes();

                var mov = GetComponent<PlayerMovement>();
                if (mov != null)
                    mov.enabled = true;

                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.enabled = true;

                if (animator != null)
                    animator.Play("Idle");

                vivo = true;
                estaRenascendo = false;
                invencivel = true;

                yield return new WaitForSeconds(2f);
                invencivel = false;

                Debug.Log("✅ Respawn completo!");
            }
        }
        else
        {
            // ❌ GAME OVER - Acabaram as 3 vidas
            Debug.Log("❌ GAME OVER! Sem vidas restantes!");

            yield return new WaitForSeconds(0.5f);

            // Reseta vidas para a próxima tentativa
            vidasRestantes = vidasTotais;

            if (GameManager.instance != null)
            {
                // O GameManager já reseta tudo no MostrarTelaGameOver
                GameManager.instance.MostrarTelaGameOver();
            }
        }
    }

    public void TakeDamage(int dano)
    {
        TomarDano(dano);
    }
}