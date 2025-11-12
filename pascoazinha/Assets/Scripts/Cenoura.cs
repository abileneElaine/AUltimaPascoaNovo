using UnityEngine;

public class Cenoura : MonoBehaviour
{
    [Header("Configurações da Cenoura")]
    public float lifetime = 1f;       // Tempo antes de se autodestruir
    public string tipo = "Normal";    // "Normal", "Fogo", "Gelo"

    [Header("Efeitos Visuais")]
    public GameObject efeitoFogo;
    public GameObject efeitoGelo;

    void Start()
    {
        // Instancia o efeito inicial se a cenoura for de fogo ou gelo
        if (tipo == "Fogo" && efeitoFogo != null)
        {
            Instantiate(efeitoFogo, transform.position + Vector3.up * 1f, Quaternion.identity);
        }
        else if (tipo == "Gelo" && efeitoGelo != null)
        {
            Instantiate(efeitoGelo, transform.position + Vector3.up * 1f, Quaternion.identity);
        }

        // Destroi a cenoura após o tempo definido (para não ficar eterna na cena)
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Ground"))
        {
            AplicarEfeito(collision.gameObject);
            CriarEfeitoVisual(collision.contacts[0].point);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Ground"))
        {
            AplicarEfeito(other.gameObject);
            CriarEfeitoVisual(other.transform.position);
            Destroy(gameObject);
        }
    }

    void AplicarEfeito(GameObject alvo)
    {
        // Só aplica efeitos se o alvo tiver tag "Enemy"
        if (!alvo.CompareTag("Enemy")) return;

        // Tenta achar qualquer tipo de script de inimigo
        var inimigoGenerico = alvo.GetComponent<Inimigo>();
        var cobra = alvo.GetComponent<Cobra>();

        // Se for um inimigo comum
        if (inimigoGenerico != null)
        {
            AplicarDanoEfeito(inimigoGenerico);
            return;
        }

        // Se for uma cobra
        if (cobra != null)
        {
            AplicarDanoEfeito(cobra);
            return;
        }
    }

    void AplicarDanoEfeito(object alvo)
    {
        // Aplica o efeito de acordo com o tipo de cenoura
        switch (tipo)
        {
            case "Fogo":
                if (alvo is Inimigo inimigo1)
                {
                    inimigo1.TomarDano(10);
                    inimigo1.Queimar(2f, 3f);
                }
                else if (alvo is Cobra cobra1)
                {
                    cobra1.TomarDano(10);
                    // Cobra ainda não tem efeito "Queimar" — pode ser adicionado depois se quiser
                }
                break;

            case "Gelo":
                if (alvo is Inimigo inimigo2)
                {
                    inimigo2.TomarDano(5);
                    inimigo2.Congelar(2f);
                }
                else if (alvo is Cobra cobra2)
                {
                    cobra2.TomarDano(5);
                    // idem: pode adicionar comportamento de congelar depois
                }
                break;

            default:
                if (alvo is Inimigo inimigo3)
                    inimigo3.TomarDano(10);
                else if (alvo is Cobra cobra3)
                    cobra3.TomarDano(10);
                break;
        }
    }

    void CriarEfeitoVisual(Vector3 pos)
    {
        GameObject efeito = null;

        switch (tipo)
        {
            case "Fogo":
                if (efeitoFogo != null)
                    efeito = Instantiate(efeitoFogo, pos, Quaternion.identity);
                break;

            case "Gelo":
                if (efeitoGelo != null)
                    efeito = Instantiate(efeitoGelo, pos, Quaternion.identity);
                break;
        }

        if (efeito != null)
            Destroy(efeito, 2f); // Efeito dura 2 segundos
    }
}
