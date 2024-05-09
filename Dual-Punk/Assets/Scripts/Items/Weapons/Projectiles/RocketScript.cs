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
    private bool _exploded;


    private new void Start()
    {
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
            Destroy();
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
        float explosionRadius, float deviationAngle, float deviationSpeed)
    {
        base.Setup(moveDirection, damage, moveSpeed, impactForce, 0);

        _startPosition = startPosition;
        _distanceUntilExplosion = distanceUntilExplosion;
        _explosionRadius = explosionRadius;
        _deviationAngle = deviationAngle;
        _deviationSpeed = deviationSpeed;
    }


    public void Destroy()
    {
        if (_exploded) return;
        _exploded = true;

        AudioManager.Instance.PlayClipAt(_explosionSound, gameObject.transform.position);
        GameObject explosion = Instantiate(_explosion, transform.position, transform.rotation);
        explosion.GetComponent<Explosion>().Explode(_damage, _explosionRadius, _impactForce);
        _smokeTrail.GetComponent<StopSmokeTrail>().StopParticles();

        Spawn(explosion);
        Destroy(gameObject);
    }


    protected void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Ennemy") || collider.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }


    /*[ObserversRpc]
    private void PlayerRocketRpc()
    {
        GameObject player = _networkManager.GetComponent<LocalPlayerReference>().PlayerState.gameObject;
        player.GetComponent<PlayerState>().CameraController.ShakeCamera(_explosionImpact / 5, 0.3f);

        Vector3 hitDirection = player.transform.position - transform.position;
        if (hitDirection.magnitude <= _explosionDistance)
        {
            player.GetComponent<IImpact>().Impact(hitDirection, _explosionImpact * (_explosionDistance - hitDirection.magnitude) / _explosionDistance);
        }
    }*/
}