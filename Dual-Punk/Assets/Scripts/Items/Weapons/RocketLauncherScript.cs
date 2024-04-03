using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RocketLauncherScript : FireArmScript
{
    [SerializeField] private float _explosionDistance;
    [SerializeField] private float _explosionImpact;
    [SerializeField] private float _deviationAngle;
    [SerializeField] private float _deviationSpeed;

    public override void Fire(Vector3 direction, int damage, float bulletSpeed, float dispersion)
    {
        for (int i = 0; i < _bulletNumber; i++)
        {
            GameObject rocket = Instantiate(_bullet, _gunEndPoints[i % _gunEndPoints.Length].transform.position, transform.rotation);
            RocketScript rocketScript = rocket.GetComponent<RocketScript>();

            rocketScript.Setup(damage, bulletSpeed, direction, transform.position, Vector3.Distance(transform.position, PlayerState.MousePosition),
                _explosionDistance + 0.1f, _explosionImpact, _deviationAngle, _deviationSpeed);

            NetworkObject bulletNetwork = rocket.GetComponent<NetworkObject>();
            bulletNetwork.Spawn();
        }
    }
}
