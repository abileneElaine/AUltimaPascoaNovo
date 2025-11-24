using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject MenuDePausa;
    public bool isPaused;

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
        Time.timeScale = 0f; // congela o jogo
        isPaused = true;
    }

    public void Resume()
    {
        MenuDePausa.SetActive(false);
        Time.timeScale = 1f; // volta ao normal
        isPaused = false;
    }
}
