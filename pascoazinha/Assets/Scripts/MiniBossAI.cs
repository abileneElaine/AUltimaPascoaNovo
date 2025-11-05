using UnityEngine;

public class MiniBossAI : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public float attackDistance = 1.5f;
    public int maxHealth = 5;

    private Animator anim;
    private int currentHealth;
    private bool isDead = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > attackDistance)
        {
            // Andando até o jogador
            anim.SetBool("isWalking", true);
            anim.SetBool("isAttacking", false);

            transform.position = Vector2.MoveTowards(transform.position,
                player.position,
                speed * Time.deltaTime);

            Flip();
        }
        else
        {
            // Iniciar ataque
            anim.SetBool("isWalking", false);
            anim.SetBool("isAttacking", true);
        }
    }

    void Flip()
    {
        if ((player.position.x > transform.position.x && transform.localScale.x < 0) ||
            (player.position.x < transform.position.x && transform.localScale.x > 0))
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Dead();
        }
    }

    void Dead()
    {
        isDead = true;
        anim.SetTrigger("Dead");
        Destroy(gameObject, 2.5f);
    }
}