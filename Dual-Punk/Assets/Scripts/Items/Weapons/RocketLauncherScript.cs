using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using Unity.Netcode;
using UnityEngine;

public class RocketLauncherScript : FireArmScript
{
    [SerializeField] private float _explosionDistance;
    [SerializeField] private float _explosionImpact;
    [SerializeField] private float _deviationAngle;
    [SerializeField] private float _deviationSpeed;


    [ServerRpc(RequireOwnership = false)]
    public override void FireBulletRpc(Vector3 direction, int damage, float bulletSpeed, float dispersion, int collisionsAllowed)
    {
        for (int i = 0; i < _bulletNumber; i++)
        {
            GameObject rocket = Instantiate(_bullet, _gunEndPoints[i % _gunEndPoints.Length].transform.position, Quaternion.identity);
            RocketScript rocketScript = rocket.GetComponent<RocketScript>();

            rocketScript.Setup(damage, bulletSpeed, direction, transform.position, Vector3.Distance(transform.position, PlayerState.MousePosition),
                _explosionDistance + 0.1f, _explosionImpact, _deviationAngle, _deviationSpeed);

            Spawn(rocket);
        }
    }
}
