using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class LaserTracker : ImplantScript
{
    [SerializeField] protected float _maximumTargetingDistance;

    private GameObject _currentTarget;
    private GameObject _oldModifiedWeapon;
    private AttacksController _attacksController => PlayerState.gameObject.GetComponent<AttacksController>();
    
    
    void Awake()
    {
        Type = ImplantType.Neuralink;
        SetName = "Laser";
    }

    public override void Run()
    {
        if (IsEquipped)
        {
            GameObject nearestEnemy = FindNearestEnemy();
            
            if (nearestEnemy != null && PlayerState.HoldingWeapon && PlayerState.WeaponScript != null)
            {
                LaserGunScript laserGunScript = PlayerState.WeaponScript as LaserGunScript;

                if (laserGunScript != null)
                {
                    if (_oldModifiedWeapon == null)
                    {
                        _oldModifiedWeapon = laserGunScript.gameObject;
                    }
                    else
                    {
                        _oldModifiedWeapon.GetComponent<LaserGunScript>().LaserTracker = false;
                        _attacksController.LaserTracker = true;

                        _oldModifiedWeapon = laserGunScript.gameObject;
                    }
                    
                    _attacksController.Target = nearestEnemy.transform.position;
                    _attacksController.LaserTracker = true;
                    laserGunScript.LaserTracker = true;
                }
            }
            else if (_oldModifiedWeapon != null)
            {
                _oldModifiedWeapon.GetComponent<LaserGunScript>().LaserTracker = false;
                _attacksController.LaserTracker = true;

                _oldModifiedWeapon = null;
            }
        }
    }
    
    private GameObject FindNearestEnemy()
    {
        EnemyState[] enemyStates = FindObjectsOfType<EnemyState>();
        List<GameObject> enemies = new List<GameObject>();

        foreach (var enemyState in enemyStates)
        {
            enemies.Add(enemyState.gameObject);
        }

        return enemies
            .OrderBy(enemie => Vector3.Distance(enemie.transform.position, PlayerState.transform.position))
            .FirstOrDefault(enemie => Vector3.Distance(enemie.transform.position, PlayerState.transform.position) <= _maximumTargetingDistance);
    }

    public override void ResetImplant()
    {
        if (_oldModifiedWeapon != null)
        {
            _oldModifiedWeapon.GetComponent<LaserGunScript>().LaserTracker = false;
            _attacksController.LaserTracker = true;
        }

        _oldModifiedWeapon = null;
        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }
}
