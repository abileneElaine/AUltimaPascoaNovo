using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;

public class PlayerThrow : MonoBehaviour
{
    [Header("Prefabs PROJÉTEIS (não coletáveis!)")]
    public GameObject cenouraBoomerang;
    public GameObject cenouraNormal;
    public GameObject cenouraFogo;
    public GameObject cenouraGelo;

    [Header("Dano de cada cenoura")]
    public int danoBoomerang = 2;
    public int danoNormal = 1;
    public int danoFogo = 3;
    public int danoGelo = 4;

    [Header("Efeitos")]
    public GameObject efeitoFogo;
    public GameObject efeitoGelo;

    [Header("Configurações")]
    public Transform spawnPoint;
    public float throwForce = 10f;

    private bool facingRight = true;
    private PlayerInventory inventario;

    void Start()
    {
        inventario = GetComponent<PlayerInventory>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) TentarAtirar(cenouraBoomerang, "Boomerang", danoBoomerang);
        if (Input.GetKeyDown(KeyCode.N)) TentarAtirar(cenouraNormal, "Normal", danoNormal);
        if (Input.GetKeyDown(KeyCode.F)) TentarAtirar(cenouraFogo, "Fogo", danoFogo);
        if (Input.GetKeyDown(KeyCode.G)) TentarAtirar(cenouraGelo, "Gelo", danoGelo);

        float h = Input.GetAxisRaw("Horizontal");
        if (h < 0 && facingRight) Flip();
        else if (h > 0 && !facingRight) Flip();
    }

    void TentarAtirar(GameObject prefab, string tipo, int dano)
    {
        if (prefab == null)
        {
            Debug.LogError($"❌ Prefab {tipo} não foi arrastado no Inspector!");
            return;
        }

        if (!inventario.UsarCenoura(tipo)) return;

        ThrowCenoura(prefab, tipo, dano);
    }

    void ThrowCenoura(GameObject prefab, string tipo, int dano)
    {
        Debug.Log($"🎯 Atirando cenoura {tipo} com dano {dano}");

        GameObject cenoura = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        // Física
        Rigidbody2D rb = cenoura.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError($"❌ Prefab {tipo} não tem Rigidbody2D!");
            return;
        }

        Collider2D colCenoura = cenoura.GetComponent<Collider2D>();
        Collider2D colPlayer = GetComponent<Collider2D>();

        if (colPlayer && colCenoura)
            Physics2D.IgnoreCollision(colPlayer, colCenoura, true);

        float dir = facingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * throwForce, 0f);

        // Configura script
        CenouraProjetil script = cenoura.GetComponent<CenouraProjetil>();
        if (script != null)
        {
            Debug.Log($"✅ Script encontrado! Configurando...");
            Debug.Log($"   Dano ANTES: {script.dano}");

            script.dano = dano;
            script.tipo = tipo;
            script.efeitoFogo = efeitoFogo;
            script.efeitoGelo = efeitoGelo;

            Debug.Log($"   Dano DEPOIS: {script.dano}");
        }
        else
        {
            Debug.LogError($"❌ Prefab {tipo} NÃO tem o script CenouraProjetil!");
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        Vector3 sp = spawnPoint.localPosition;
        sp.x = Mathf.Abs(sp.x) * (facingRight ? 1 : -1);
        spawnPoint.localPosition = sp;
    }
}