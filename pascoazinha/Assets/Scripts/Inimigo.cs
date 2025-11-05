using System.Collections;
using UnityEngine;

public class Inimigo : MonoBehaviour
{
    public int vida = 30;
    private bool queimando = false;
    private bool congelado = false;
    private SpriteRenderer sr;
    private Color corOriginal;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            corOriginal = sr.color;
    }

    public void TomarDano(int dano)
    {
        vida -= dano;
        if (vida <= 0)
        {
            Morrer();
        }
    }

    public void Queimar(float duracao, float danoPorSegundo)
    {
        if (!queimando)
            StartCoroutine(EfeitoFogo(duracao, danoPorSegundo));
    }

    public void Congelar(float duracao)
    {
        if (!congelado)
            StartCoroutine(EfeitoGelo(duracao));
    }

    private IEnumerator EfeitoFogo(float duracao, float danoPorSegundo)
    {
        queimando = true;

        float tempo = 0f;
        while (tempo < duracao)
        {
            TomarDano((int)danoPorSegundo);
            if (sr != null) sr.color = Color.red;
            yield return new WaitForSeconds(1f);
            tempo += 1f;
        }

        if (sr != null) sr.color = corOriginal;
        queimando = false;
    }

    private IEnumerator EfeitoGelo(float duracao)
    {
        congelado = true;

        // Se quiser impedir movimento:
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (sr != null) sr.color = Color.cyan;

        yield return new WaitForSeconds(duracao);

        if (sr != null) sr.color = corOriginal;
        congelado = false;
    }

    void Morrer()
    {
        Destroy(gameObject);
    }
}