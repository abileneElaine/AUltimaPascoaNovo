using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject MenuDePausa;
    public bool isPaused;

    void Start()
    {
        // Garante que o menu começa escondido
        MenuDePausa.SetActive(false);
        isPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                Pause();
            else
                Resume();
        }
    }

    public void Pause()
    {
        MenuDePausa.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        MenuDePausa.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
}