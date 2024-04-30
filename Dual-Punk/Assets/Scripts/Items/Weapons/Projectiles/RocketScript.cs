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
    private float _explosionDistance;
    private float _explosionImpact;
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

        if (_moveSpeed < 5 || Vector3.Distance(transform.position, _startPosition) > _distanceUntilExplosion)
        {
            Destroy();
        }
    }


    private Vector3 DeviateDirection()
    {
        Vector3 newDirection = Quaternion.Euler(0, 0, _deviationAngle * Mathf.Sin(Time.time * _deviationSpeed)) * _moveDirection;
        _moveFactor = Methods.GetDirectionFactor(newDirection);
        transform.eulerAngles = new Vector3(0, 0, Methods.GetAngle(newDirection));
        return newDirection;
    }


    public void Setup(int damage, float moveSpeed, Vector3 moveDirection, Vector3 startPosition, float distanceUntilExplosion,
        float explosionDistance, float explosionImpact, float deviationAngle, float deviationSpeed)
    {
        Setup(damage, moveSpeed, moveDirection, 0);

        _startPosition = startPosition;
        _distanceUntilExplosion = distanceUntilExplosion;
        _explosionDistance = explosionDistance;
        _explosionImpact = explosionImpact;
        _deviationAngle = deviationAngle;
        _deviationSpeed = deviationSpeed;
    }


    public void Destroy()
    {
        if (_exploded) return;
        _exploded = true;

        GameObject explosion = Instantiate(_explosion, transform.position, transform.rotation);
        explosion.GetComponent<Explosion>().Explode(_damage, _explosionDistance, _explosionImpact);
        AudioManager.Instance.PlayClipAt(_explosionSound, gameObject.transform.position);

        StopParticles();

        Spawn(explosion);
        Destroy(explosion, 1);
        Destroy(gameObject);
    }

    private void StopParticles()
    {
        _smokeTrail.GetComponent<StopSmokeTrail>().StopParticles();
    }


    protected void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.CompareTag("Weapon") && !collider.CompareTag("Projectile") && !collider.CompareTag("UI"))
        {
            Destroy();
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