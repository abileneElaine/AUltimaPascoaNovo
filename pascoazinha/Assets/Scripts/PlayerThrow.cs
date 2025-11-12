using UnityEngine;

public class PlayerThrow : MonoBehaviour
{
    [Header("Prefabs de cenouras")]
    public GameObject cenouraBoomerang;
    public GameObject cenouraNormal;
    public GameObject cenouraFogo;
    public GameObject cenouraGelo;

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
        if (Input.GetKeyDown(KeyCode.B)) TentarAtirar(cenouraBoomerang, "Boomerang");
        if (Input.GetKeyDown(KeyCode.N)) TentarAtirar(cenouraNormal, "Normal");
        if (Input.GetKeyDown(KeyCode.F)) TentarAtirar(cenouraFogo, "Fogo");
        if (Input.GetKeyDown(KeyCode.G)) TentarAtirar(cenouraGelo, "Gelo");

        float h = Input.GetAxisRaw("Horizontal");
        if (h < 0 && facingRight) Flip();
        else if (h > 0 && !facingRight) Flip();
    }

    void TentarAtirar(GameObject prefab, string tipo)
    {
        if (!inventario.UsarCenoura(tipo)) return; // só atira se tiver disponível
        ThrowCenoura(prefab, tipo);
    }

    void ThrowCenoura(GameObject prefab, string tipo)
    {
        if (!prefab) return;

        GameObject cenoura = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        Rigidbody2D rb = cenoura.GetComponent<Rigidbody2D>();
        Collider2D colCenoura = cenoura.GetComponent<Collider2D>();
        Collider2D colPlayer = GetComponent<Collider2D>();

        if (colPlayer && colCenoura)
            Physics2D.IgnoreCollision(colPlayer, colCenoura, true);

        float dir = facingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * throwForce, 0f);

        Cenoura scriptC = cenoura.GetComponent<Cenoura>();
        if (scriptC)
        {
            scriptC.tipo = tipo;
            scriptC.efeitoFogo = efeitoFogo;
            scriptC.efeitoGelo = efeitoGelo;
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
