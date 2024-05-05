using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;


public class RocketLauncherScript : FireArmScript
{
    [SerializeField] private float _explosionRadius;
    [SerializeField] private float _deviationAngle;
    [SerializeField] private float _deviationSpeed;

    private Vector3 _targetPoint;


    public override void Run(Vector3 position, Vector3 direction, Vector3 targetPoint)
    {
        _targetPoint = targetPoint;
        base.Run(position, direction, targetPoint);
    }

    public override void EnemyRun(EnemyState enemyState, Vector3 position, Vector3 direction, Vector3 targetPoint)
    {
        _targetPoint = targetPoint;
        base.EnemyRun(enemyState, position, direction, targetPoint);
    }


    public override void Fire(Vector3 direction, int damage, float bulletSpeed, float bulletSize, float impactForce, float dispersion, int collisionsAllowed)
    {
        _ammoLeft--;
        _fireTimer = 0;
        AudioManager.Instance.PlayClipAt(_fireSound, gameObject.transform.position);

        FireRocketRpc(direction, damage, bulletSpeed, bulletSize, impactForce);
    }


    [ServerRpc(RequireOwnership = false)]
    private void FireRocketRpc(Vector3 direction, int damage, float bulletSpeed, float bulletSize, float impactForce)
    {
        for (int i = 0; i < _bulletNumber; i++)
        {
            GameObject rocket = Instantiate(_bullet, _gunEndPoints[_bulletPointIndex].transform.position, Quaternion.identity);
            RocketScript rocketScript = rocket.GetComponent<RocketScript>();

            if (_warriorLuckBullet)
            {
                rocket.GetComponent<SpriteRenderer>().color = new Color(255f, 0f, 0f, 255f);
                _warriorLuckBullet = false;
            }

            _bulletPointIndex = (_bulletPointIndex + 1) % _gunEndPoints.Length;
            rocket.transform.localScale = new Vector2(bulletSize, bulletSize);

            rocketScript.Setup(direction, damage, bulletSpeed, impactForce, transform.position, Vector3.Distance(transform.position, _targetPoint),
                _explosionRadius + 0.1f, _deviationAngle, _deviationSpeed);

            Spawn(rocket);
        }
    }
}
