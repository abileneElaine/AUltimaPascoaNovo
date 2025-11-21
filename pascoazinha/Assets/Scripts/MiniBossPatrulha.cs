using UnityEngine;
using System.Collections;

public class MiniBossPatrulha : MonoBehaviour, IDamageable
{
    public int maxEnergy;
    public int damage;
    public float moveSpeed;
    public bool useTransform;
    public bool shouldFlip;

    [Header("Patrulha")]
    [SerializeField] private Vector2 movePosition;
    [SerializeField] private Transform moveDestination;

    [Header("Ataque")]
    public float distanciaAtaque = 2f;     // distância do player para atacar
    public float tempoEntreAtaques = 1.2f; // delay entre ataques
    private bool podeAtacar = true;

    [Header("Referências")]
    public Animator animator;
    public Transform player;

    [Header("Feedback de Dano")]
    [SerializeField] private int blinkHitTimes;
    [SerializeField] private float blinkHitDuration;

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

    void Start()
    {
        animator = GetComponent<Animator>();
        _collider2D = GetComponent<Collider2D>();
        _audioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _isAlive = true;

        if (shouldFlip) _originalLocalScaleX = transform.localScale.x;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (useTransform)
            _moveTarget = moveDestination.localPosition;
        else
            _moveTarget = movePosition;

        _initialPosition = transform.position;

        _currentMoveDirection = (_initialPosition + _moveTarget - (Vector2)transform.position).normalized;

        _currentEnergy = maxEnergy;
    }

    void Update()
    {
        if (!_isAlive) return;

        CheckAttack();   // 🔥 NOVO - Verifica se o player está perto para atacar

        MovePlatform();
    }

    // ------------------------------------------------------------------------
    // ATAQUE – Quando o player está perto
    // ------------------------------------------------------------------------
    private void CheckAttack()
    {
        if (!podeAtacar) return;

        float distancia = Vector2.Distance(transform.position, player.position);

        if (distancia <= distanciaAtaque)
        {
            StartCoroutine(Atacar());
        }
    }

    private IEnumerator Atacar()
    {
        podeAtacar = false;

        // vira para o player
        if (shouldFlip)
        {
            Vector3 escala = transform.localScale;
            escala.x = (player.position.x > transform.position.x)
                ? Mathf.Abs(escala.x)
                : -Mathf.Abs(escala.x);
            transform.localScale = escala;
        }

        // toca animação de ataque
        animator.SetTrigger("Attack");

        // dá dano ao player
        player.GetComponent<IDamageable>()?.TakeEnergy(damage);

        yield return new WaitForSeconds(tempoEntreAtaques);

        podeAtacar = true;
    }

    // ------------------------------------------------------------------------
    // PATRULHA (igual ao seu código original)
    // ------------------------------------------------------------------------
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

        // Move o inimigo
        transform.position += (Vector3)_currentMoveDirection * moveSpeed * Time.deltaTime;

        // 🔥 Flip da sprite conforme a direção
        if (shouldFlip)
        {
            Vector3 escala = transform.localScale;
            if (_currentMoveDirection.x > 0)
                escala.x = Mathf.Abs(_originalLocalScaleX);
            else if (_currentMoveDirection.x < 0)
                escala.x = -Mathf.Abs(_originalLocalScaleX);
            transform.localScale = escala;
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

    public void TakeEnergy(int damage)
    {
        throw new System.NotImplementedException();
    }
}
