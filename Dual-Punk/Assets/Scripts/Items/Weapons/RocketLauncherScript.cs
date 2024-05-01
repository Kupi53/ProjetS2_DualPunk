using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;


public class RocketLauncherScript : FireArmScript
{
    [SerializeField] private float _explosionRadius;
    [SerializeField] private float _deviationAngle;
    [SerializeField] private float _deviationSpeed;


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

            _bulletPointIndex = (_bulletPointIndex + 1) % _gunEndPoints.Length;
            rocket.transform.localScale = new Vector2(bulletSize, bulletSize);

            rocketScript.Setup(direction, damage, bulletSpeed, impactForce, transform.position, Vector3.Distance(transform.position, PlayerState.MousePosition),
                _explosionRadius + 0.1f, _deviationAngle, _deviationSpeed);

            Spawn(rocket);
        }
    }
}
