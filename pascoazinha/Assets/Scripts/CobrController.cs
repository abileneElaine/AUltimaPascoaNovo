using UnityEngine;
using System.Collections;


public class CobraController : MonoBehaviour, IDamageable
{
    public int maxEnergy;   // <- você já tinha
    public int damage;      // <- dano ao player
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

    void Start()
    {
        _animator = GetComponent<Animator>();
        _collider2D = GetComponent<Collider2D>();
        _audioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _isAlive = true;

        if (shouldFlip) _originalLocalScaleX = transform.localScale.x;

        if (useTransform)
        {
            _moveTarget = moveDestination.localPosition;
        }
        else
        {
            _moveTarget = movePosition;
        }

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
                transform.localScale =
                    new Vector3(-_originalLocalScaleX, transform.localScale.y, transform.localScale.z);
            else
                transform.localScale = new Vector3(_originalLocalScaleX, transform.localScale.y, transform.localScale.z);
        }

        transform.position += (Vector3)_currentMoveDirection * moveSpeed * Time.deltaTime;
    }


    // -------------------------------------------------------------------------
    // ADIÇÃO: RECEBER DANO DA CENOURA
    // -------------------------------------------------------------------------
    public void TakeEnergy(int dano)  // já existia por causa do IDamageable
    {
        if (!_isAlive) return;

        _currentEnergy -= dano;

        // Fica vermelho ao ser atingida
        StartCoroutine(HitBlink());  

        if (_currentEnergy <= 0)
        {
            _currentEnergy = 0;

            _isAlive = false;

            _collider2D.enabled = false;
            moveSpeed = 0;

            // cor vermelha da morte
            _spriteRenderer.color = Color.red;

            // desaparecer após 0.4s
            Destroy(gameObject, 0.4f);
        }

        // trava no máximo
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


    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if (useTransform)
            {
                Debug.DrawLine(_initialPosition, (Vector3)_initialPosition + moveDestination.localPosition, Color.yellow);
            }
            else
            {
                Debug.DrawLine(_initialPosition, (Vector3)_initialPosition + (Vector3)movePosition, Color.red);
            }
        }
        else
        {
            if (useTransform)
            {
                Debug.DrawLine(transform.position, transform.position + moveDestination.localPosition, Color.yellow);
            }
            else
            {
                Debug.DrawLine(transform.position, transform.position + (Vector3)movePosition, Color.red);
            }
        }
    }


    // -------------------------------------------------------------------------
    // ADIÇÃO: DAR DANO AO PLAYER
    // -------------------------------------------------------------------------
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!_isAlive) return;

        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<IDamageable>()?.TakeEnergy(damage);
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (!_isAlive) return;

        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<IDamageable>()?.TakeEnergy(damage);
        }
    }
    
    // -------------------------------------------------------------------------
// ADIÇÃO: RECEBER DANO DA CENOURA (Trigger)
// -------------------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isAlive) return;

        if (other.CompareTag("Cenoura"))
        {
            TakeEnergy(1);   // dano da cenoura (pode mudar depois)
            Destroy(other.gameObject); // destruir a cenoura ao bater
        }
    }

}
