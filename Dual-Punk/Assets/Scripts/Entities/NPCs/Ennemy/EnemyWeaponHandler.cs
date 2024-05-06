using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(EnemyState))]
public class EnemyWeaponHandler : NetworkBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private GameObject[] _weapons;

    private EnemyState _enemyState;
    private int _weaponIndex;


    private void Start()
    {
        _weaponIndex = 0;
        _enemyState = GetComponent<EnemyState>();

        GameObject weapon = Instantiate(_weapons[0], transform.position, Quaternion.identity);
        Spawn(weapon);

        _enemyState.WeaponScript = weapon.GetComponent<WeaponScript>();
        _enemyState.WeaponScript.EnemyState = _enemyState;
        // WeaponScript.UserRecoil = GetComponent<IImpact>();
    }


    private void Update()
    {
        _enemyState.WeaponScript.PickUp();

        if (_enemyState.Target == null) return;

        bool canAttack = false;
        float distance = Vector2.Distance(transform.position, _enemyState.Target.transform.position);

        if (distance < _enemyState.WeaponScript.Range)
        {
            int hits = Physics2D.RaycastAll(transform.position, _enemyState.Target.transform.position, distance, _layerMask).Length;

            if (_enemyState.WeaponScript is ChargeWeaponScript && hits <= ((ChargeWeaponScript)_enemyState.WeaponScript).CollisionsAllowed)
            {
                canAttack = true;
                ((ChargeWeaponScript)_enemyState.WeaponScript).ObstaclesEnemy = hits;
            }
            else if (hits == 0)
            {
                canAttack = true;
            }
        }

        if (!canAttack)
            _enemyState.Run = true;
        _enemyState.CanAttack = canAttack;

        _enemyState.WeaponScript.EnemyRun(transform.position, _enemyState.Direction, _enemyState.Target.transform.position);
    }
}
