using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;


public class GrenadeScript : NetworkBehaviour
{
    [SerializeField] private GameObject _explosion;

    [SerializeField] private int _damage;
    [SerializeField] private float _explosionRadius;
    [SerializeField] private float _explosionImpact;
    [SerializeField] private float _stoppingFactor;

    private Rigidbody2D _rb2d;
    private SpriteRenderer _spriteRenderer;
    private ObjectSpawner _objectSpawner;

    private Vector3 _startPosition;
    private Vector3 _moveDirection;
    private Vector3 _linePosition;
    private Vector3 _verticalDirection;

    private float _moveSpeed;
    private float _explosionTimer;
    private float _distanceUntilStop;
    private float _curveFactor;
    private bool _stop;


    private void Start()
    {
        _stop = false;
        _linePosition = transform.position;

        _rb2d = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _objectSpawner = GameObject.Find("ObjectSpawner").GetComponent<ObjectSpawner>();
    }


    private void FixedUpdate()
    {
        if (!IsServer) return;

        _linePosition += _moveDirection * _moveSpeed;
        float currentDistance = Vector3.Distance(_linePosition, _startPosition);

        if (!_stop) {
            float factor = currentDistance / _distanceUntilStop;
            _rb2d.MovePosition(_linePosition + _verticalDirection * (- factor * factor * _curveFactor + factor * _curveFactor));
        } else {
            _rb2d.MovePosition(_linePosition);
            _moveSpeed -= _stoppingFactor * Time.fixedDeltaTime;
        }

        if (!_stop && currentDistance > _distanceUntilStop)
        {
            _stop = true;
            _moveSpeed /= _stoppingFactor;
            _spriteRenderer.sortingOrder = 1;
        }

        if (_explosionTimer > 0) {
            _explosionTimer -= Time.fixedDeltaTime;
        } else {
            Explode();
        }
    }


    private void Explode()
    {
        GameObject explosion = Instantiate(_explosion, transform.position, transform.rotation);
        _objectSpawner.SpawnObjectRpc(explosion);

        Destroy(explosion, 1);
        Destroy(gameObject);
    }


    public void Setup(Vector3 startPosition, Vector3 moveDirection, float moveSpeed, float explosionTimer, float distanceUntilStop, float curveFactor)
    {
        _startPosition = startPosition;
        _moveDirection = moveDirection;
        _moveSpeed = moveSpeed;
        _explosionTimer = explosionTimer;
        _distanceUntilStop = distanceUntilStop;
        _curveFactor = curveFactor;
    }


    private void OnTriggerStay2D(Collider2D collider)
    {
        if (!collider.CompareTag("Ennemy") && !collider.CompareTag("Weapon") && _stop)
            _moveSpeed = 0;
    }
}
