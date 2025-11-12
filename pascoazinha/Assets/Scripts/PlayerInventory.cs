using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int normalCount;
    public int fogoCount;
    public int geloCount;
    public int boomerangCount;

    public void AdicionarCenoura(string tipo)
    {
        switch (tipo)
        {
            case "Normal": normalCount++; break;
            case "Fogo": fogoCount++; break;
            case "Gelo": geloCount++; break;
            case "Boomerang": boomerangCount++; break;
        }

        Debug.Log($"Cenoura coletada: {tipo}");
    }

    public bool UsarCenoura(string tipo)
    {
        switch (tipo)
        {
            case "Normal": if (normalCount > 0) { normalCount--; return true; } break;
            case "Fogo": if (fogoCount > 0) { fogoCount--; return true; } break;
            case "Gelo": if (geloCount > 0) { geloCount--; return true; } break;
            case "Boomerang": if (boomerangCount > 0) { boomerangCount--; return true; } break;
        }

        Debug.Log($"Sem cenouras do tipo: {tipo}");
        return false;
    }
}