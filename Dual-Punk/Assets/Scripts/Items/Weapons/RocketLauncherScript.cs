using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;


public class RocketLauncherScript : PowerWeaponScript
{
    [SerializeField] private float _explosionRadius;
    [SerializeField] private float _deviationAngle;
    [SerializeField] private float _deviationSpeed;


    protected override void Fire(Vector3 direction, int damage, float dispersion, float distance, bool damagePlayer)
    {
        _ammoLeft--;
        _fireTimer = 0;
        UserRecoil.Impact(-direction, _recoilForce);
        AudioManager.Instance.PlayClipAt(_fireSound, gameObject.transform.position, _ownerType);

        bool warriorLuckBullet = false;
        if (WarriorLuck && UnityEngine.Random.Range(0, 100) < DropPercentage)
        {
            damage *= DamageMultiplier;
            warriorLuckBullet = true;
        }

        FireRocketRpc(direction, distance, damage, warriorLuckBullet, damagePlayer);
    }


    [ServerRpc(RequireOwnership = false)]
    private void FireRocketRpc(Vector3 direction, float distance, int damage, bool warriorLuckBullet, bool damagePlayer)
    {
        for (int i = 0; i < _bulletsPerShot; i++)
        {
            GameObject rocket = Instantiate(_bullet, _gunEndPoints[_bulletPointIndex].transform.position, Quaternion.identity);
            RocketScript rocketScript = rocket.GetComponent<RocketScript>();

            if (warriorLuckBullet)
            {
                rocket.GetComponent<SpriteRenderer>().color = new Color(255f, 0f, 0f, 255f);
            }

            _bulletPointIndex = (_bulletPointIndex + 1) % _gunEndPoints.Length;
            rocket.transform.localScale = new Vector2(_bulletSize, _bulletSize);

            rocketScript.Setup(direction, damage, _bulletSpeed, _impactForce, transform.position, distance + 0.5f,
                _explosionRadius + 0.1f, _deviationAngle, _deviationSpeed, damagePlayer, warriorLuckBullet, _ownerType, PlayerState);

            Spawn(rocket);
        }
    }
}
