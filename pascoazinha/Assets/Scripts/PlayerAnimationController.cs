using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    public GameOverUI gameOverUI; // arraste o canvas no Inspector

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetIsWalking(bool value)
    {
        animator.SetBool("isWalking", value);
    }

    public void SetIsJumping(bool value)
    {
        animator.SetBool("isJumping", value);
    }

    public void PlayDeathAnimation()
    {
        animator.SetTrigger("Death"); // toca a animação

        // 🔥 CHAMA O PAINEL DE GAME OVER APÓS 1 SEGUNDO
        Invoke(nameof(ShowGameOver), 1f);
    }

    private void ShowGameOver()
    {
        if (gameOverUI != null)
            gameOverUI.ShowGameOver();
        else
            Debug.LogError("GameOverUI NÃO configurado no Inspector!");
    }
}

public abstract class GameOverUI
{
    public void ShowGameOver()
    {
        throw new System.NotImplementedException();
    }
}