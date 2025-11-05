using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class MiniBossHealth_Explosion : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 10;
    private int currentHealth;

    [Header("Explosão e efeitos")]
    public GameObject explosionPrefab; // arrastar Explosion_Prefab
    public AudioClip explosionSfx;     // arrastar áudio
    public float destroyDelay = 0.25f; // tempo até destruir o objeto após spawn da explosão

    [Header("Feedback visual")]
    public float flashDuration = 0.06f;
    public int flashCount = 6;

    [Header("Knockback (opcional)")]
    public bool applyKnockbackOnDeath = true;
    public Vector2 deathKnockback = new Vector2(0f, 2f); // relativo ao boss (para cima)
    public float knockbackForce = 3f;

    [Header("Drops (opcional)")]
    public GameObject[] dropPrefabs; // cenouras/itens

    // referências internas
    private SpriteRenderer sr;
    private Collider2D mainCollider;
    private Rigidbody2D rb;
    private MonoBehaviour[] aiScripts; // para desativar AI scripts (MiniBossAI etc.)
    private AudioSource audioSource;

    void Awake()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        mainCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // pega scripts de IA (ex.: MiniBossAI)
        aiScripts = GetComponents<MonoBehaviour>();
    }

    // chamado por outros scripts: PlayerAttack, projéteis, etc.
    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        StartCoroutine(FlashRoutine());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashRoutine()
    {
        for (int i = 0; i < flashCount; i++)
        {
            sr.enabled = false;
            yield return new WaitForSeconds(flashDuration);
            sr.enabled = true;
            yield return new WaitForSeconds(flashDuration);
        }
    }

    void Die()
    {
        // 1) desativa AI e comportamento — desliga scripts que não sejam este
        foreach (var s in aiScripts)
        {
            if (s != this) s.enabled = false;
        }

        // 2) desativa colisão principal para evitar checagens futuras
        if (mainCollider != null) mainCollider.enabled = false;

        // 3) para velocidade / movimento
        if (rb != null) {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true; // evita física residual
        }

        // 4) spawn da explosão
        if (explosionPrefab != null) {
            GameObject e = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            // toca sfx no explosionPrefab se tiver AudioSource, senão toca aqui
            var ad = e.GetComponent<AudioSource>();
            if (ad != null && explosionSfx != null) {
                ad.PlayOneShot(explosionSfx);
            } else if (explosionSfx != null) {
                audioSource.PlayOneShot(explosionSfx);
            }
        } else {
            if (explosionSfx != null) audioSource.PlayOneShot(explosionSfx);
        }

        // 5) knockback visual (opcional) - aplica impulso antes de sumir
        if (applyKnockbackOnDeath && rb != null) {
            rb.isKinematic = false;
            Vector2 dir = deathKnockback.normalized * knockbackForce;
            rb.AddForce(dir, ForceMode2D.Impulse);
        }

        // 6) spawn drops
        if (dropPrefabs != null && dropPrefabs.Length > 0) {
            foreach (var d in dropPrefabs) {
                if (d != null) Instantiate(d, transform.position + (Vector3)Random.insideUnitCircle*0.4f, Quaternion.identity);
            }
        }

        // 7) destruir objeto após delay curto (mantém tempo para efeitos)
        Destroy(gameObject, destroyDelay);
    }
}
