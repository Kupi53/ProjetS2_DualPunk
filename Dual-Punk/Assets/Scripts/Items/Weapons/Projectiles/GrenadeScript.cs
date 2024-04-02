using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class GrenadeScript : MonoBehaviour
{
    [SerializeField] private GameObject _explosion;

    [SerializeField] private int _damage;
    [SerializeField] private float _explosionDistance;
    [SerializeField] private float _explosionImpact;
    [SerializeField] private float _moveDecreaseSpeed;
    [SerializeField] private float _stoppingFactor;

    private Rigidbody2D _rb2d;
    private SpriteRenderer _spriteRenderer;

    private Vector3 _startPosition;
    private Vector3 _moveDirection;

    private float _moveSpeed;
    private float _explodeTimer;
    private float _distanceUntilStop;
    private bool _stop;


    private void Start()
    {
        _stop = false;
        _rb2d = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }


    // quand distance est atteinte on active la collision, si collision moveSpeed = 0
    private void FixedUpdate()
    {
        if (_moveSpeed > 0)
        {
            _rb2d.MovePosition(transform.position + _moveDirection * _moveSpeed * Time.fixedDeltaTime);
            _moveSpeed -= _moveDecreaseSpeed * Time.fixedDeltaTime;
        }

        if (!_stop && Vector3.Distance(transform.position, _startPosition) > _distanceUntilStop)
        {
            _stop = true;
            _moveSpeed /= _stoppingFactor;
            _moveDecreaseSpeed *= _stoppingFactor;
            _spriteRenderer.sortingOrder = 1;
        }

        if (_explodeTimer > 0)
        {
            _explodeTimer -= Time.fixedDeltaTime;
        }
        else
        {
            Explode();
        }
    }


    private void Explode()
    {
        GameObject explosion = Instantiate(_explosion, transform.position, transform.rotation);
        Destroy(explosion, 1);
        Destroy(gameObject);
    }


    public void Setup(Vector3 startPosition, Vector3 moveDirection, float moveSpeed, float explodeTimer, float distanceUntilStop)
    {
        _startPosition = startPosition;
        _moveDirection = moveDirection;
        _moveSpeed = moveSpeed;
        _explodeTimer = explodeTimer;
        _distanceUntilStop = distanceUntilStop;
    }


    private void OnTriggerStay2D(Collider2D collider)
    {
        if (!collider.CompareTag("Ennemy") && !collider.CompareTag("Weapon") && _stop)
            _moveSpeed = 0;
    }
}
