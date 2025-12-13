using UnityEngine;
using System.Collections;

public class CobraController : MonoBehaviour, IDamageable
{
    [Header("ID Único - IMPORTANTE")]
    public string enemyID = ""; // ← Defina um ID único no Inspector

    public int maxEnergy;
    public int damage;
    public float moveSpeed;
    public bool useTransform;
    public bool shouldFlip;

    [SerializeField] private Vector2 movePosition;
    [SerializeField] private Transform moveDestination;
    [SerializeField] private int blinkHitTimes;
    [SerializeField] private float blinkHitDuration;

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

    public AudioClip somDeMorte;
    public AudioClip somDano;

    private float damageCooldown = 1f;
    private float lastDamageTime = 0f;

    void Start()
    {
        // ✅ Verifica se já morreu antes
        if (GameManager.instance != null && GameManager.instance.InimigoJaMorreu(enemyID))
        {
            gameObject.SetActive(false);
            return;
        }

        _animator = GetComponent<Animator>();
        _collider2D = GetComponent<Collider2D>();
        _audioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _isAlive = true;

        if (shouldFlip) _originalLocalScaleX = transform.localScale.x;

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
        if (_isAlive) MovePlatform();
    }

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

        if (shouldFlip)
        {
            if (_isReturning)
                transform.localScale = new Vector3(-_originalLocalScaleX, transform.localScale.y, transform.localScale.z);
            else
                transform.localScale = new Vector3(_originalLocalScaleX, transform.localScale.y, transform.localScale.z);
        }

        transform.position += (Vector3)_currentMoveDirection * moveSpeed * Time.deltaTime;
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
                GameManager.instance.RegistrarInimigoMorto(enemyID);

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
            TakeEnergy(1);
            Destroy(other.gameObject);
        }
    }
}