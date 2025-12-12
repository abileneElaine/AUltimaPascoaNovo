using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class SkipCutscene : MonoBehaviour
{
    public PlayableDirector timeline;
    public string cenaDestino;

    public void Pular()
    {
        timeline.time = timeline.duration; // pula a timeline
        SceneManager.LoadScene(cenaDestino);
    }
}
