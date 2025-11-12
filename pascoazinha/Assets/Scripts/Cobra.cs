using UnityEngine;

public class Cobra : Inimigo
{
    // 🩸 Método público para receber dano externamente (ex: cenouras, traps, etc.)
    public void TomarDano(int dano)
    {
        LevarDano(dano);
    }
}