using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float playerVelocity = 10f;            // Velocidade de movimento
    public float jumpForce = 10f;                 // Força do pulo
    public PlayerAnimationController playerAnim;  // Controlador de animações
    public Rigidbody2D rb;                        // Corpo rígido 2D do player

    private bool isGrounded = true;               // Verifica se está no chão
    private int jumpCount = 0;                    // Conta quantos pulos foram feitos
    public int maxJumps = 2;                      // Quantos pulos o personagem pode dar (2 = pulo duplo)

    private bool isDead = false; 
    public GameObject cenouraNormal;
    public GameObject cenouraFogo;
    public GameObject cenouraGelo;

    public AudioSource somDoPulo;

    // Flag para não mover após morrer

    // Ajustes para o "descer ao morrer"
    [Header("Death drop settings")]
    [Tooltip("Quanto em unidades Unity o player desce ao morrer")]
    public float deathDropAmount = 0.25f;
    [Tooltip("Tempo (s) que leva para descer")]
    public float deathDropDuration = 0.15f;

    private void Update()
    {
        // Se estiver morto, não executar nada
        if (isDead) return;

        // Se o player apertar a tecla 4 -> inicia morte com queda
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            // marca morto e para movimento imediato
            isDead = true;
            rb.linearVelocity = Vector2.zero;

            // inicia a queda suave antes de tocar a animação de death
            StartCoroutine(FallSlightlyThenDie(deathDropAmount, deathDropDuration));
            return;
        }

        // Captura o eixo horizontal (setas esquerda/direita)
        float horizontal = Input.GetAxisRaw("Horizontal");

        // --- PULO COM PULO DUPLO ---
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++;
            isGrounded = false;
            playerAnim.SetIsJumping(true);
            somDoPulo.Play();
        }

        // --- MOVIMENTO HORIZONTAL ---
        if (Mathf.Abs(horizontal) > 0)
        {
            // Move o personagem no eixo X
            transform.position += Vector3.right * horizontal * (Time.deltaTime * playerVelocity);

            // Faz o flip da sprite conforme a direção
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(horizontal) * Mathf.Abs(scale.x);
            transform.localScale = scale;

            // Define animação de andar se estiver no chão
            if (isGrounded)
                playerAnim.SetIsWalking(true);
        }
        else
        {
            // Parou de andar
            playerAnim.SetIsWalking(false);
        }
    }

    // --- DETECTA CONTATO COM O CHÃO ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0; // Reseta os pulos ao tocar o chão
            playerAnim.SetIsJumping(false);
        }
    }

    // Coroutine que move o player um pouco para baixo e só depois toca a animação de morte
    private System.Collections.IEnumerator FallSlightlyThenDie(float dropAmount, float duration)
    {
        // Segurança: zera a velocidade e trava física pra controlar pela transform
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
            // suaviza movimento (ease out)
            float ease = 1f - Mathf.Pow(1f - t, 2f);
            transform.position = Vector3.Lerp(startPos, targetPos, ease);
            yield return null;
        }

        // garante posição final exata
        transform.position = targetPos;

        // toca animação de morte (método no seu controller)
        playerAnim.PlayDeathAnimation();

        // se precisar, reativa a física depois (normalmente não precisa porque o player fica morto)
        rb.isKinematic = previousKinematic;
    }
}
