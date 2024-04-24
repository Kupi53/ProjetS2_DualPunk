using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;


public class RocketLauncherScript : FireArmScript
{
    [SerializeField] private float _explosionDistance;
    [SerializeField] private float _explosionImpact;
    [SerializeField] private float _deviationAngle;
    [SerializeField] private float _deviationSpeed;


    protected override void Fire(Vector3 direction, int damage, float bulletSpeed, float bulletSize, float dispersion, int collisionsAllowed)
    {
        _ammoLeft--;
        _fireTimer = 0;

        FireRocketRpc(direction, damage, bulletSpeed, bulletSize);
    }


    [ServerRpc(RequireOwnership = false)]
    private void FireRocketRpc(Vector3 direction, int damage, float bulletSpeed, float bulletSize)
    {
        for (int i = 0; i < _bulletNumber; i++)
        {
            GameObject rocket = Instantiate(_bullet, _gunEndPoints[i % _gunEndPoints.Length].transform.position, Quaternion.identity);
            RocketScript rocketScript = rocket.GetComponent<RocketScript>();

            rocket.transform.localScale = new Vector2(bulletSize, bulletSize);
            rocketScript.Setup(damage, bulletSpeed, direction, transform.position, Vector3.Distance(transform.position, PlayerState.MousePosition),
                _explosionDistance + 0.1f, _explosionImpact, _deviationAngle, _deviationSpeed);

            Spawn(rocket);
        }
    }
}
