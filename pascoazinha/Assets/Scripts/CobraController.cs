using UnityEngine;
using System.Collections;

public class CobraController : MonoBehaviour, IDamageable
{
    [Header("ID Único - IMPORTANTE")]
    public string enemyID = "cobra_1"; // ← Defina um ID único no Inspector

    [Header("Configurações")]
    public int maxEnergy = 3;
    public int damage = 1;
    public float moveSpeed = 2f;
    public bool useTransform;
    public bool shouldFlip;

    [Header("Movimento")]
    [SerializeField] private Vector2 movePosition;
    [SerializeField] private Transform moveDestination;

    [Header("Efeitos")]
    [SerializeField] private int blinkHitTimes = 3;
    [SerializeField] private float blinkHitDuration = 0.1f;

    [Header("Sons")]
    public AudioClip somDeMorte;
    public AudioClip somDano;

    private Vector2 _initialPosition;
    private Vector2 _moveTarget;
    private Vector2 _currentMoveDirection;
    private bool _isReturning;
    private float _originalLocalScaleX;
    private int _currentEnergy;
    private Animator _animator;
    private bool _isAlive;
    private Collider2D _collider2D;
    private AudioSource _audioSource;
    private SpriteRenderer _spriteRenderer;

    private float damageCooldown = 1f;
    private float lastDamageTime = 0f;

    void Start()
    {
        // ✅ Verifica se já morreu antes
        if (GameManager.instance != null && GameManager.instance.InimigoJaMorreu(enemyID))
        {
            Debug.Log($"🐍 Cobra {enemyID} já estava morta");
            gameObject.SetActive(false);
            return;
        }

        _animator = GetComponent<Animator>();
        _collider2D = GetComponent<Collider2D>();
        _audioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _isAlive = true;

        if (shouldFlip) _originalLocalScaleX = transform.localScale.x;

        // ✅ SEMPRE pega a posição atual como inicial
        _initialPosition = transform.position;

        if (useTransform && moveDestination != null)
            _moveTarget = moveDestination.localPosition;
        else
            _moveTarget = movePosition;

        // ✅ Recalcula a direção inicial
        Vector2 targetPos = _initialPosition + _moveTarget;
        _currentMoveDirection = (targetPos - _initialPosition).normalized;

        // ✅ Verifica se a direção é válida
        if (_currentMoveDirection.magnitude < 0.01f)
        {
            Debug.LogWarning($"⚠️ Cobra {enemyID} tem movePosition muito pequeno! Use valores maiores (ex: 3, 5, 10)");
            _currentMoveDirection = Vector2.right; // Direção padrão
        }

        _isReturning = false;
        _currentEnergy = maxEnergy;

        Debug.Log($"🐍 Cobra {enemyID} iniciada. Pos: {_initialPosition}, Alvo: {targetPos}");
    }

    void Update()
    {
        if (_isAlive) MovePlatform();
    }

    private void MovePlatform()
    {
        // ✅ Calcula as posições de destino
        Vector2 targetPosition = _initialPosition + _moveTarget;

        if (!_isReturning)
        {
            // Indo para o alvo
            float distToTarget = Vector2.Distance(transform.position, targetPosition);

            if (distToTarget < 0.1f)
            {
                _isReturning = true;
                _currentMoveDirection = (_initialPosition - (Vector2)transform.position).normalized;

                // ✅ Garante direção válida
                if (_currentMoveDirection.magnitude < 0.01f)
                    _currentMoveDirection = -_moveTarget.normalized;
            }
        }
        else
        {
            // Voltando para a posição inicial
            float distToInitial = Vector2.Distance(transform.position, _initialPosition);

            if (distToInitial < 0.1f)
            {
                _isReturning = false;
                _currentMoveDirection = (targetPosition - (Vector2)transform.position).normalized;

                // ✅ Garante direção válida
                if (_currentMoveDirection.magnitude < 0.01f)
                    _currentMoveDirection = _moveTarget.normalized;
            }
        }

        // ✅ Flip baseado na direção atual
        if (shouldFlip)
        {
            if (_isReturning)
                transform.localScale = new Vector3(-_originalLocalScaleX, transform.localScale.y, transform.localScale.z);
            else
                transform.localScale = new Vector3(_originalLocalScaleX, transform.localScale.y, transform.localScale.z);
        }

        // ✅ Move apenas se a direção for válida
        if (_currentMoveDirection.magnitude > 0.01f)
        {
            transform.position += (Vector3)_currentMoveDirection * moveSpeed * Time.deltaTime;
        }
    }

    public void TakeEnergy(int dano)
    {
        if (!_isAlive) return;

        _currentEnergy -= dano;

        if (_audioSource != null && somDano != null)
            _audioSource.PlayOneShot(somDano);

        StartCoroutine(HitBlink());

        if (_currentEnergy <= 0)
        {
            _currentEnergy = 0;
            _isAlive = false;

            _collider2D.enabled = false;
            moveSpeed = 0;

            _spriteRenderer.color = Color.red;

            if (_audioSource != null && somDeMorte != null)
                _audioSource.PlayOneShot(somDeMorte);

            // ✅ Registra morte no GameManager
            if (GameManager.instance != null)
            {
                GameManager.instance.RegistrarInimigoMorto(enemyID);
                Debug.Log($"🐍 Cobra {enemyID} morreu");
            }

            Destroy(gameObject, 0.4f);
        }

        if (_currentEnergy > maxEnergy)
            _currentEnergy = maxEnergy;
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

    private void OnCollisionStay2D(Collision2D other)
    {
        if (!_isAlive) return;

        if (other.gameObject.CompareTag("Player"))
        {
            if (Time.time - lastDamageTime >= damageCooldown)
            {
                lastDamageTime = Time.time;
                other.gameObject.GetComponent<IDamageable>()?.TakeEnergy(damage);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isAlive) return;

        if (other.CompareTag("Cenoura"))
        {
            // ✅ DANO VARIÁVEL: Pega o dano da cenoura
            CenouraProjetil cenoura = other.GetComponent<CenouraProjetil>();

            if (cenoura != null)
            {
                Debug.Log($"🐍 Cobra {enemyID} levou {cenoura.dano} de dano");
                TakeEnergy(cenoura.dano);
            }
            else
            {
                // Fallback: dano padrão de 1
                TakeEnergy(1);
            }

            Destroy(other.gameObject);
        }
    }

    // ✅ Gizmos para ver a rota no editor
    private void OnDrawGizmos()
    {
        Vector2 startPos = Application.isPlaying ? _initialPosition : (Vector2)transform.position;
        Vector2 endPos;

        if (useTransform && moveDestination != null)
            endPos = startPos + (Vector2)moveDestination.localPosition;
        else
            endPos = startPos + movePosition;

        // Linha da rota
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPos, endPos);

        // Ponto inicial (verde)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startPos, 0.2f);

        // Ponto final (vermelho)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(endPos, 0.2f);
    }
}