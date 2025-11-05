using UnityEngine;
using UnityEngine.UI;
using System.Collections;
    

public class GameOverMenu : MonoBehaviour
{
    public Text pointsText;
    public void SetUp(int score)
    {
        gameObject.SetActive(true);
        pointsText.text = score.ToString();
    }
}
