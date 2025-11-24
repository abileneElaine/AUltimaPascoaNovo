using UnityEngine;
using UnityEngine.UI;

public class AjustarVolumeSlider : MonoBehaviour
{
    [SerializeField] private MenuPrincipalManager menu;

    [SerializeField] private Slider slider;


    public void SendVolumeToMenu()
    {
        menu.AjustarVolumeMusica(slider.value);
    }
}
