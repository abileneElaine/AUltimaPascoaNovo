using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimentação")]
    public float playerVelocity = 10f;
    public float jumpForce = 10f;
    public PlayerAnimationController playerAnim;
    public Rigidbody2D rb;

    private bool isGrounded = true;
    private int jumpCount = 0;
    public int maxJumps = 2;

    private bool isDead = false;

    [Header("Cenouras")]
    public GameObject cenouraNormal;
    public GameObject cenouraFogo;
    public GameObject cenouraGelo;

    public AudioSource somDoPulo;

    [Header("Vida")]
    public int vidaMaxima = 3;
    public int vidaAtual;

    [Header("Death drop settings")]
    public float deathDropAmount = 0.25f;
    public float deathDropDuration = 0.15f;

    private void Start()
    {
        if (GameManager.instance != null && GameManager.instance.TemCheckpoint())
        {
            transform.position = GameManager.instance.ObterCheckpoint();
            Debug.Log("Player voltou ao checkpoint: " + transform.position);
        }
    }

    
    private void Update()
    {
        if (isDead) return;

        // Movimento horizontal
        float horizontal = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(horizontal) > 0)
        {
            transform.position += Vector3.right * horizontal * (Time.deltaTime * playerVelocity);

            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(horizontal) * Mathf.Abs(scale.x);
            transform.localScale = scale;

            if (isGrounded)
                playerAnim.SetIsWalking(true);
        }
        else
        {
            playerAnim.SetIsWalking(false);
        }

        // Pulo + pulo duplo
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++;
            isGrounded = false;
            playerAnim.SetIsJumping(true);
            somDoPulo.Play();
        }
    }

    // -------- VIDA E DANO --------
    public void ReceberDano(int dano)
    {
        if (isDead) return;

        vidaAtual -= dano;

        if (vidaAtual <= 0)
        {
            vidaAtual = 0;
            Morrer();
        }
    }

    private void Morrer()
    {
        if (isDead) return;

        isDead = true;

        rb.linearVelocity = Vector2.zero;

        StartCoroutine(FallSlightlyThenDie(deathDropAmount, deathDropDuration));
    }

    private System.Collections.IEnumerator FallSlightlyThenDie(float dropAmount, float duration)
    {
        rb.linearVelocity = Vector2.zero;
        bool previousKinematic = rb.isKinematic;
        rb.isKinematic = true;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.down * Mathf.Abs(dropAmount);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float ease = 1f - Mathf.Pow(1f - t, 2f);
            transform.position = Vector3.Lerp(startPos, targetPos, ease);
            yield return null;
        }

        transform.position = targetPos;

        // Animação de morte
        playerAnim.PlayDeathAnimation();

        // Dura ~1s (ajuste se sua animação for maior)
        yield return new WaitForSeconds(1f);

        // CHAMA O GAME OVER SEMPRE
        GameManager.instance.MostrarTelaGameOver();

        rb.isKinematic = previousKinematic;
    }

    // Detecta o chão
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0;
            playerAnim.SetIsJumping(false);
        }
    }
}
