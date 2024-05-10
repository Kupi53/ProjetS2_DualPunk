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

    public override void EnemyRun(Vector3 position, Vector3 direction, Vector3 targetPoint)
    {
        _targetPoint = targetPoint;
        base.EnemyRun(position, direction, targetPoint);
    }


    public override void Fire(Vector3 direction, int damage, float dispersion, bool damagePlayer)
    {
        _ammoLeft--;
        _fireTimer = 0;
        AudioManager.Instance.PlayClipAt(_fireSound, gameObject.transform.position);

        bool warriorLuckBullet = false;
        if (WarriorLuck && UnityEngine.Random.Range(0, DropPercentage) == 0)
        {
            damage *= DamageMultiplier;
            warriorLuckBullet = true;
        }

        FireRocketRpc(direction, damage, warriorLuckBullet, damagePlayer);
    }


    [ServerRpc(RequireOwnership = false)]
    private void FireRocketRpc(Vector3 direction, int damage, bool warriorLuckBullet, bool damagePlayer)
    {
        for (int i = 0; i < _bulletNumber; i++)
        {
            GameObject rocket = Instantiate(_bullet, _gunEndPoints[_bulletPointIndex].transform.position, Quaternion.identity);
            RocketScript rocketScript = rocket.GetComponent<RocketScript>();

            if (warriorLuckBullet)
            {
                rocket.GetComponent<SpriteRenderer>().color = new Color(255f, 0f, 0f, 255f);
            }

            _bulletPointIndex = (_bulletPointIndex + 1) % _gunEndPoints.Length;
            rocket.transform.localScale = new Vector2(_bulletSize, _bulletSize);

            rocketScript.Setup(direction, damage, _bulletSpeed, _impactForce, transform.position, Vector3.Distance(transform.position, _targetPoint),
                _explosionRadius + 0.1f, _deviationAngle, _deviationSpeed, damagePlayer);

            Spawn(rocket);
        }
    }
}
