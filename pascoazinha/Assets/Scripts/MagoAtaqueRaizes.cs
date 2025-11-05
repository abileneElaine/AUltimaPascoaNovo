using UnityEngine;

public class MagoAtaqueRaizes : MonoBehaviour
{
    [Header("Configurações do Ataque")]
    public GameObject prefabRaizes;     // Prefab das raízes
    public Transform pontoSpawn;        // Onde as raízes aparecem (abaixo do mago)
    public float tempoParaSumir = 2f;   // Quanto tempo ficam ativas
    public float atrasoSpawn = 0.3f;    // Atraso pra sincronizar com a animação

    private Animator anim;
    private bool podeAtacar = true;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Atacar()
    {
        if (!podeAtacar) return;

        podeAtacar = false;
        anim.SetTrigger("Atacar"); // precisa existir no Animator
        Invoke(nameof(CriarRaizes), atrasoSpawn);
    }

    void CriarRaizes()
    {
        if (prefabRaizes == null) return;

        GameObject raizes = Instantiate(prefabRaizes, pontoSpawn.position, Quaternion.identity);
        Destroy(raizes, tempoParaSumir);

        Invoke(nameof(PermitirNovoAtaque), 1.5f);
    }

    void PermitirNovoAtaque()
    {
        podeAtacar = true;
    }
}