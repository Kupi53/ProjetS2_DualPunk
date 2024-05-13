using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;


public class InstantGrenadeScript : NetworkBehaviour, IDestroyable
{
    [SerializeField] protected GameObject _explosion;
    [SerializeField] protected AudioClip _explosionSound;
    [SerializeField] protected int _damage;
    [SerializeField] private float _explosionRadius;
    [SerializeField] private float _explosionImpact;
    [SerializeField] protected float _rotateSpeed;

    protected Rigidbody2D _rb2d;

    protected Vector3 _startPosition;
    protected Vector3 _moveDirection;
    protected Vector3 _linePosition;
    protected Vector3 _verticalDirection;

    protected float _moveSpeed;
    protected float _distanceUntilStop;
    protected float _curveFactor;
    private bool _exploded;
    private bool _damagePlayer;


    protected void Start()
    {
        _exploded = false;
        _damagePlayer = false;
        _linePosition = transform.position;
        _rb2d = GetComponent<Rigidbody2D>();
    }


    private void FixedUpdate()
    {
        if (!IsServer) return;

        _linePosition += _moveDirection * _moveSpeed;

        float currentDistance = Vector3.Distance(_linePosition, _startPosition);
        float factor = currentDistance / _distanceUntilStop;

        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + _rotateSpeed * Time.deltaTime);
        _rb2d.MovePosition(_linePosition + _verticalDirection * (-factor * factor * _curveFactor + factor * _curveFactor));

        if (currentDistance > _distanceUntilStop)
        {
            Explode();
        }
    }


    public bool Destroy()
    {
        if (!_damagePlayer)
            return false;
        Explode();
        return true;
    }


    private void Explode()
    {
        if (_exploded) return;
        _exploded = true;

        AudioManager.Instance.PlayClipAt(_explosionSound, gameObject.transform.position);
        GameObject explosion = Instantiate(_explosion, transform.position, transform.rotation);
        Spawn(explosion);

        explosion.GetComponent<Explosion>().Explode(_damage, _explosionRadius, _explosionImpact, false);
        Destroy(gameObject);
    }


    public virtual void Setup(Vector3 startPosition, Vector3 moveDirection, Vector3 verticalDirection, float moveSpeed, float explosionTimer, float distanceUntilStop, float curveFactor)
    {
        _startPosition = startPosition;
        _moveDirection = moveDirection;
        _verticalDirection = verticalDirection;
        _moveSpeed = moveSpeed;
        _distanceUntilStop = distanceUntilStop;
        _curveFactor = curveFactor;
    }
}