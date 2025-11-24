using UnityEngine;
using TMPro;

public class PlaquinhaDialogue : MonoBehaviour
{
    public string textToShow;
    public string[] textoDaPlaca;

    public GameObject uiObject; // painel da placa com TMP
    TMP_Text t;

    private void Start()
    {
        t = uiObject.GetComponentInChildren<TMP_Text>();
        uiObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            t.text = textToShow;
            uiObject.SetActive(true);
        }
        Debug.Log("Entrou!");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            uiObject.SetActive(false);
        }
    }
}
