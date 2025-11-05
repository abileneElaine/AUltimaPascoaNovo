using UnityEngine;

public class MageAI : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;         // Arraste o jogador aqui
    public Animator animator;        // Animator do mago

    [Header("Configurações de movimento")]
    public float speed = 2f;         // Velocidade de movimento
    public float stopDistance = 2f;  // Distância mínima para parar de andar

    private bool isDead = false;

    void Update()
    {
        if (isDead || player == null) return;

        // Distância até o jogador
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {
            // Anda em direção ao jogador
            animator.SetBool("isWalking", true);

            Vector2 direction = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

            // Flip na direção certa (mantém o tamanho original)
            if (direction.x > 0)
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x); // garante positivo
                transform.localScale = scale;
            }
            else if (direction.x < 0)
            {
                Vector3 scale = transform.localScale;
                scale.x = -Mathf.Abs(scale.x); // inverte sem mudar tamanho
                transform.localScale = scale;
            }

        }
        else
        {
            // Perto demais → fica parado
            animator.SetBool("isWalking", false);
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        animator.SetBool("isWalking", false);
        animator.SetTrigger("Die");
    }
}