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

    void Update()
    {
        // Disparo direto por tecla
        if (Input.GetKeyDown(KeyCode.B)) ThrowCenoura(cenouraBoomerang, "Boomerang");
        if (Input.GetKeyDown(KeyCode.N)) ThrowCenoura(cenouraNormal, "Normal");
        if (Input.GetKeyDown(KeyCode.F)) ThrowCenoura(cenouraFogo, "Fogo");
        if (Input.GetKeyDown(KeyCode.G)) ThrowCenoura(cenouraGelo, "Gelo");

        // Virar personagem
        float h = Input.GetAxisRaw("Horizontal");
        if (h < 0 && facingRight) Flip();
        else if (h > 0 && !facingRight) Flip();
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

        // Configura tipo e efeitos
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
