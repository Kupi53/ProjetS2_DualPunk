using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using System;
using Unity.Netcode;
using FishNet.Object;
using FishNet.Connection;
using System.Linq;


public class SmartWeaponScript : FireArmScript
{
    [SerializeField] private GameObject _lockedTargetIndicator;
    [SerializeField] private float _bulletRotateSpeed;
    [SerializeField] private float _lockingRange;

    private List<GameObject> _targetsIndicators;
    private float _resetTimer;
    private int _index;

    // Permet de lock que sur un ennemi car on lock en maintenant la touche, si on maintient trop ca reset
    private bool _waitForNextTarget;


    private new void Start()
    {
        base.Start();

        _index = 0;
        _resetTimer = 0;
        _waitForNextTarget = false;
        _targetsIndicators = new List<GameObject>();
    }


    public override void Run(Vector3 position, Vector3 direction, Vector3 targetPoint)
    {
        if (Input.GetButtonDown("Switch"))
        {
            _waitForNextTarget = false;
        }
        if (Input.GetButton("Switch"))
        {
#nullable enable
            GameObject? target;
#nullable disable
            target = GetNearestTarget(PlayerState.MousePosition);

            if (target != null && !_waitForNextTarget)
            {
                if (!_waitForNextTarget && CheckTarget(target))
                {
                    GameObject newTargetIndicator = Instantiate(_lockedTargetIndicator);
                    newTargetIndicator.GetComponent<TargetIndicatorScript>().Target = target;
                    _targetsIndicators.Add(newTargetIndicator);
                }
                _waitForNextTarget = true;
            }
            
            _resetTimer += Time.deltaTime;
            if (_resetTimer > 0.4)
                ResetWeapon();
        }
        else
            _resetTimer = 0;
        
        base.Run(position, direction, targetPoint);
    }



    private bool CheckTarget(GameObject target)
    {
        for (int i = 0; i < _targetsIndicators.Count; i++)
        {
            if (_targetsIndicators[i] == null || _targetsIndicators[i].GetComponent<TargetIndicatorScript>().Target == target)
            {
                Destroy(_targetsIndicators[i]);
                _targetsIndicators.Remove(_targetsIndicators[i]);
                return false;
            }
        }
        return true;
    }


    public override void ResetWeapon()
    {
        base.ResetWeapon();

        for (int i = 0; i < _targetsIndicators.Count; i++)
        {
            Destroy(_targetsIndicators[i]);
        }
        _targetsIndicators.Clear();
    }



#nullable enable
    private GameObject? GetNearestTarget(Vector3 position)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Ennemy");

        return (from enemy in enemies
                let distance = Vector2.Distance(position, enemy.transform.position + Vector3.up / 2)
                where distance < _lockingRange
                orderby distance
                select enemy).FirstOrDefault();
    }
#nullable disable


    private GameObject AssignTarget()
    {
        if (EnemyState != null)
            return EnemyState.Target;

        if (_targetsIndicators.Count == 0)
        {
            return null;
        }

        _index = (_index + 1) % _targetsIndicators.Count;

        if (_targetsIndicators[_index] == null)
        {
            _targetsIndicators.Remove(_targetsIndicators[_index]);
            return AssignTarget();
        }

        return _targetsIndicators[_index].GetComponent<TargetIndicatorScript>().Target;
    }



    public override void Fire(Vector3 direction, int damage, float dispersion, bool damagePlayer)
    {
        _ammoLeft--;
        _fireTimer = 0;
        UserRecoil.Impact(-direction, _recoilForce);
        if (PlayerState != null)
            AudioManager.Instance.PlayClipAt(_fireSound, gameObject.transform.position, "Player");
        else
            AudioManager.Instance.PlayClipAt(_fireSound, gameObject.transform.position, "Enemy");

        bool warriorLuckBullet = false;
        if (WarriorLuck && UnityEngine.Random.Range(0, 100) < DropPercentage)
        {
            damage *= DamageMultiplier;
            warriorLuckBullet = true;
        }
        if (_aiming)
        {
            dispersion /= _aimAccuracy;
        }

        FireSeekingBulletRpc(AssignTarget(), direction, damage, dispersion, warriorLuckBullet, damagePlayer);
    }


    [ServerRpc(RequireOwnership = false)]
    private void FireSeekingBulletRpc(GameObject target, Vector3 direction, int damage, float dispersion, bool warriorLuckBullet, bool damagePlayer)
    {
        for (int i = 0; i < _bulletNumber; i++)
        {
            GameObject newBullet = Instantiate(_bullet, _gunEndPoints[_bulletPointIndex].transform.position, Quaternion.identity);
            SeekingBulletScript bulletScript = newBullet.GetComponent<SeekingBulletScript>();

            if (warriorLuckBullet)
            {
                newBullet.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 255f);
            }

            _bulletPointIndex = (_bulletPointIndex + 1) % _gunEndPoints.Length;
            newBullet.transform.localScale = new Vector2(_bulletSize, _bulletSize);
            Vector3 newDirection = new Vector3(direction.x + Methods.NextFloat(-dispersion, dispersion), direction.y + Methods.NextFloat(-dispersion, dispersion), 0).normalized;

            bulletScript.Setup(target, newDirection, damage, _bulletSpeed, _impactForce, _bulletRotateSpeed, damagePlayer, warriorLuckBullet);
            Spawn(newBullet);
        }
    }
}