using System.Collections;
using System.Collections.Generic;
using FishNet.Managing;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;


public class RocketScript : BulletScript, IDestroyable
{
    [SerializeField] private GameObject _explosion;
    [SerializeField] private GameObject _smokeTrail;
    [SerializeField] private GameObject _trailPosition;
    [SerializeField] private AudioClip _explosionSound;

    private Vector3 _startPosition;
    private float _distanceUntilExplosion;
    private float _explosionRadius;
    private float _deviationAngle;
    private float _deviationSpeed;
    private string _shooter;
    private bool _exploded;


    private new void Start()
    {
        if (!IsServer) return;
        base.Start();

        _exploded = false;
        _smokeTrail = Instantiate(_smokeTrail, transform.position, transform.rotation);
        Spawn(_smokeTrail);
    }


    private new void FixedUpdate()
    {
        if (!IsServer) return;

        _rb2d.velocity = DeviateDirection() * _moveSpeed * _moveFactor;
        _smokeTrail.transform.position = _trailPosition.transform.position;
        _smokeTrail.transform.rotation = transform.rotation;

        if (Vector3.Distance(transform.position, _startPosition) > _distanceUntilExplosion || _moveSpeed < 5)
        {
            Explode();
        }
    }


    private Vector3 DeviateDirection()
    {
        Vector3 newDirection = Quaternion.Euler(0, 0, _deviationAngle * Mathf.Sin(Time.time * _deviationSpeed)) * _moveDirection;
        transform.eulerAngles = new Vector3(0, 0, Methods.GetAngle(newDirection));
        _moveFactor = Methods.GetDirectionFactor(newDirection);

        return newDirection;
    }


    public void Setup(Vector3 moveDirection, int damage, float moveSpeed, float impactForce, Vector3 startPosition, float distanceUntilExplosion,
        float explosionRadius, float deviationAngle, float deviationSpeed, bool damagePlayer, bool warriorLuck, string shooter, PlayerState playerState)
    {
        base.Setup(moveDirection, damage, moveSpeed, impactForce, 0, damagePlayer, warriorLuck, playerState);

        _startPosition = startPosition;
        _distanceUntilExplosion = distanceUntilExplosion;
        _explosionRadius = explosionRadius;
        _deviationAngle = deviationAngle;
        _deviationSpeed = deviationSpeed;
        _shooter = shooter;
    }


    public override bool DestroyObject()
    {
        if (!_damagePlayer || _exploded)
            return false;

        Explode();
        _exploded = true;
        return true;
    }


    private void Explode()
    {
        if (_exploded) return;
        _exploded = true;

        AudioManager.Instance.PlayClipAt(_explosionSound, gameObject.transform.position, _shooter);
        
        GameObject explosion = Instantiate(_explosion, transform.position, transform.rotation);
        Spawn(explosion);

        explosion.GetComponent<Explosion>().Explode(_damage, _explosionRadius, _impactForce, _damagePlayer, _warriorLuck);
        _smokeTrail.GetComponent<StopSmokeTrail>().StopParticles();

        Destroy(gameObject);
    }


    protected void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Ennemy") || collider.CompareTag("Wall") || collider.CompareTag("Player") && _damagePlayer)
        {
            Explode();
        }
    }
}