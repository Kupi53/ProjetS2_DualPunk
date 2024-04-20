using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;


public class GrenadeScript : NetworkBehaviour
{
    [SerializeField] private GameObject _explosion;
    [SerializeField] private AudioClip _sound;

    [SerializeField] private int _damage;
    [SerializeField] private float _explosionRadius;
    [SerializeField] private float _explosionImpact;
    [SerializeField] private float _stoppingFactor;
    [SerializeField] private float _rotateSpeed;

    private Rigidbody2D _rb2d;
    private SpriteRenderer _spriteRenderer;

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
    }


    private void FixedUpdate()
    {
        if (!IsServer) return;

        _linePosition += _moveDirection * _moveSpeed;

        if (!_stop)
        {
            float currentDistance = Vector3.Distance(_linePosition, _startPosition);
            float factor = currentDistance / _distanceUntilStop;

            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + _rotateSpeed * Time.deltaTime);
            _rb2d.MovePosition(_linePosition + _verticalDirection * (- factor * factor * _curveFactor + factor * _curveFactor));

            if (currentDistance > _distanceUntilStop)
            {
                _stop = true;
                _spriteRenderer.sortingOrder = 1;
            }
        }
        else
        {
            _rb2d.MovePosition(_linePosition);
            if (_moveSpeed > 0)
                _moveSpeed -= _stoppingFactor * Time.fixedDeltaTime;
            else
                _moveSpeed = 0;
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
        explosion.GetComponent<Explosion>().Explode(_damage, _explosionRadius, _explosionImpact);
        AudioManager.Instance.PlayClipAt(_sound, gameObject.transform.position);

        Spawn(explosion);
        Destroy(explosion, 1);
        Destroy(gameObject);
    }


    public void Setup(Vector3 startPosition, Vector3 moveDirection, Vector3 verticalDirection, float moveSpeed, float explosionTimer, float distanceUntilStop, float curveFactor)
    {
        _startPosition = startPosition;
        _moveDirection = moveDirection;
        _verticalDirection = verticalDirection;
        _moveSpeed = moveSpeed;
        _explosionTimer = explosionTimer;
        _distanceUntilStop = distanceUntilStop;
        _curveFactor = curveFactor;
    }


    private void OnTriggerStay2D(Collider2D collider)
    {
        if (_stop)
            _moveSpeed = 0;
    }
}
