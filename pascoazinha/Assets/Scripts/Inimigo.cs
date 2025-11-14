using UnityEngine;

public class Inimigo : MonoBehaviour
{  
    public float speed;
    public bool ground = true;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public bool facingRight = true;

    [Header("Componentes")]
    public Animator animator;  // <<< ADICIONADO

    private void Update()
    {
        // --- Movimento ---
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        // --- Checar borda ---
        ground = Physics2D.Linecast(groundCheck.position, transform.position, groundLayer);

        if (ground == false)
        {
            speed *= -1;
        }

        // --- Flip ---
        if (speed > 0 && !facingRight)
        {
            Flip();
        }
        else if (speed < 0 && facingRight)
        {
            Flip();
        }

        // --- Ativar animação walk ---
        bool andando = Mathf.Abs(speed) > 0.1f;  
        animator.SetBool("walk", andando);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scale = transform.localScale;
        Scale.x *= -1;
        transform.localScale = Scale;
    }
}