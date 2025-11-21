using UnityEngine;
using System.Collections;

public class MagoPatrulha : MonoBehaviour, IDamageable
{
    [Header("Configurações Gerais")]
    public int maxEnergy = 10;
    public int damage = 5;
    public float moveSpeed = 2f;
    public bool useTransform = false;
    public bool shouldFlip = true;

    [Header("Patrulha")]
    [SerializeField] private Vector2 movePosition;
    [SerializeField] private Transform moveDestination;

    [Header("Ataque")]
    public float distanciaAtaque = 3f;     // Distância mínima para atacar
    public float tempoEntreAtaques = 1.5f; // Delay entre ataques

    [Header("Raízes/Projétil")]
    public GameObject raizPrefab;          // Prefab da raiz com DamageArea
    public Transform pontoDeSaida;         // Onde a raiz começa
    public float tempoCrescimento = 0.5f;  // Tempo que a raiz leva para crescer

    [Header("Referências")]
    public Animator animator;
    public Transform player;

    [Header("Feedback de Dano")]
    [SerializeField] private int blinkHitTimes = 3;
    [SerializeField] private float blinkHitDuration = 0.1f;

    private Vector2 _initialPosition;
    private Vector2 _moveTarget;
    private Vector2 _currentMoveDirection;
    private bool _isReturning;
    private float _originalLocalScaleX;
    private int _currentEnergy;
    private bool _isAlive;
    private Collider2D _collider2D;
    private SpriteRenderer _spriteRenderer;
    private AudioSource _audioSource;
    private bool podeAtacar = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        _collider2D = GetComponent<Collider2D>();
        _audioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _isAlive = true;
        _currentEnergy = maxEnergy;

        if (shouldFlip) _originalLocalScaleX = transform.localScale.x;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (useTransform && moveDestination != null)
            _moveTarget = moveDestination.localPosition;
        else
            _moveTarget = movePosition;

        _initialPosition = transform.position;
        _currentMoveDirection = (_initialPosition + _moveTarget - (Vector2)transform.position).normalized;
    }

    void Update()
    {
        if (!_isAlive) return;

        CheckAttack();
        MovePlatform();
    }

    // ---------------------------------------------------
    // Verifica se o player está perto para atacar
    // ---------------------------------------------------
    private void CheckAttack()
    {
        if (!podeAtacar) return;

        if (Vector2.Distance(transform.position, player.position) <= distanciaAtaque)
        {
            StartCoroutine(Atacar());
        }
    }

    // ---------------------------------------------------
    // Ataque: animação + raiz que causa dano
    // ---------------------------------------------------
    private IEnumerator Atacar()
    {
        podeAtacar = false;

        // Vira para o player
        if (shouldFlip)
        {
            Vector3 escala = transform.localScale;
            escala.x = (player.position.x > transform.position.x)
                ? Mathf.Abs(escala.x)
                : -Mathf.Abs(escala.x);
            transform.localScale = escala;
        }

        // Toca animação de ataque
        animator.SetTrigger("Attack");

        // Instancia a raiz se estiver tudo configurado
        if (raizPrefab != null && pontoDeSaida != null)
        {
            GameObject raiz = Instantiate(raizPrefab, pontoDeSaida.position, Quaternion.identity);

            // Faz a raiz mirar para o player
            Vector2 direcao = (player.position - pontoDeSaida.position).normalized;
            raiz.transform.right = direcao;

            // Faz a raiz crescer visualmente
            StartCoroutine(CrescerRaiz(raiz.transform));
        }

        yield return new WaitForSeconds(tempoEntreAtaques);
        podeAtacar = true;
    }

    // ---------------------------------------------------
    // Coroutine para o efeito visual de crescimento da raiz
    // ---------------------------------------------------
    private IEnumerator CrescerRaiz(Transform raiz)
    {
        Vector3 escalaInicial = raiz.localScale;
        escalaInicial.x = 0.1f; // começa quase invisível
        raiz.localScale = escalaInicial;

        Vector3 escalaFinal = raiz.localScale;
        escalaFinal.x = 1f; // tamanho final da raiz (ajuste conforme sprite)

        float tempo = 0f;
        while (tempo < tempoCrescimento)
        {
            raiz.localScale = Vector3.Lerp(escalaInicial, escalaFinal, tempo / tempoCrescimento);
            tempo += Time.deltaTime;
            yield return null;
        }
        raiz.localScale = escalaFinal;

        // Mantém a raiz ativa por 1 segundo antes de destruir
        yield return new WaitForSeconds(1f);
        Destroy(raiz.gameObject);
    }

    // ---------------------------------------------------
    // Patrulha com flip automático
    // ---------------------------------------------------
    private void MovePlatform()
    {
        if (!_isReturning)
        {
            if (Vector2.Distance(transform.position, _initialPosition + _moveTarget) < 0.1f)
            {
                _isReturning = true;
                _currentMoveDirection = (_initialPosition - (Vector2)transform.position).normalized;
            }
        }
        else
        {
            if (Vector2.Distance(transform.position, _initialPosition) < 0.1f)
            {
                _isReturning = false;
                _currentMoveDirection = (_initialPosition + _moveTarget - (Vector2)transform.position).normalized;
            }
        }

        transform.position += (Vector3)_currentMoveDirection * moveSpeed * Time.deltaTime;

        if (shouldFlip)
        {
            Vector3 escala = transform.localScale;
            escala.x = (_currentMoveDirection.x > 0) ? Mathf.Abs(_originalLocalScaleX) : -Mathf.Abs(_originalLocalScaleX);
            transform.localScale = escala;
        }
    }

    // ---------------------------------------------------
    // Recebe dano
    // ---------------------------------------------------
    public void TakeEnergy(int dano)
    {
        if (!_isAlive) return;

        _currentEnergy -= dano;
        StartCoroutine(HitBlink());

        if (_currentEnergy <= 0)
        {
            _isAlive = false;
            moveSpeed = 0;
            _collider2D.enabled = false;
            _spriteRenderer.color = Color.red;
            Destroy(gameObject, 0.4f);
        }
    }

    private IEnumerator HitBlink()
    {
        _spriteRenderer.color = Color.red;
        for (int i = 0; i < blinkHitTimes - 1; i++)
        {
            yield return new WaitForSeconds(blinkHitDuration);
            _spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(blinkHitDuration);
            _spriteRenderer.color = Color.red;
        }
        yield return new WaitForSeconds(blinkHitDuration);
        _spriteRenderer.color = Color.white;
    }
}
