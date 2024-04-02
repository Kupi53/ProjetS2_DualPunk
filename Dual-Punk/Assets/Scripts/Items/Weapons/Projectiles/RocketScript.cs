using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RocketScript : BulletScript
{
    [SerializeField] private GameObject _explosion;

    private Vector3 _startPosition;
    private float _distanceUntilExplosion;
    private float _explosionDistance;
    private float _explosionImpact;
    private float _deviationAngle;
    private float _deviationSpeed;


    private new void FixedUpdate()
    {
        _rb2d.velocity = DeviateDirection() * _moveSpeed * _moveFactor;

        if (!GetComponent<Renderer>().isVisible || _moveSpeed < 5 || Vector3.Distance(transform.position, _startPosition) > _distanceUntilExplosion)
        {
            DestroyThis();
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
        Setup(damage, moveSpeed, moveDirection);

        _startPosition = startPosition;
        _distanceUntilExplosion = distanceUntilExplosion;
        _explosionDistance = explosionDistance;
        _explosionImpact = explosionImpact;
        _deviationAngle = deviationAngle;
        _deviationSpeed = deviationSpeed;
    }


    protected override void DestroyThis()
    {
        GameObject explosion = Instantiate(_explosion, transform.position, transform.rotation);
        Destroy(explosion, 1);

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            player.GetComponent<PlayerState>().CameraController.ShakeCamera(_explosionImpact / 5, 0.3f);

            Vector3 hitDirection = player.transform.position - transform.position;
            if (hitDirection.magnitude <= _explosionDistance)
            {
                player.GetComponent<IImpact>().Impact(hitDirection, _explosionImpact * (_explosionDistance - hitDirection.magnitude));
            }
        }

        GameObject[] ennemies = GameObject.FindGameObjectsWithTag("Ennemy");
        foreach(GameObject ennemy in ennemies)
        {
            EnnemyState health = ennemy.GetComponent<EnnemyState>();

            Vector3 hitDirection = ennemy.transform.position - transform.position;
            if (hitDirection.magnitude <= _explosionDistance)
            {
                health.OnDamage(_damage * (_explosionDistance - hitDirection.magnitude));
            }
        }

        Destroy(gameObject);
    }


    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.CompareTag("Weapon") && !collider.CompareTag("Projectile") && !collider.CompareTag("UI"))
        {
            DestroyThis();
        }
    }
}