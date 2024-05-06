using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyState : NPCState
{
    [SerializeField] private float _lockDistance;
    [SerializeField] private float _unlockDistance;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private GameObject[] _weapons;

    private GameObject _target;
    private int _weaponIndex;

    public override Vector3 TargetPoint { get => _target == null ? Vector3.zero : _target.transform.position; }
    public WeaponScript WeaponScript { get; private set; }
    public bool CanAttack { get; set; }


    private new void Start()
    {
        base.Start();

        _weaponIndex = 0;

        GameObject weapon = Instantiate(_weapons[0], transform.position, Quaternion.identity);
        Spawn(weapon);

        WeaponScript = weapon.GetComponent<WeaponScript>();
        WeaponScript.EnemyState = this;
        // WeaponScript.UserRecoil = GetComponent<IImpact>();
    }


    private void Update()
    {
        CanAttack = false;
        MoveSpeed = _walkSpeed;
        WeaponScript.PickUp();

        if (_target == null)
        {
            WeaponScript.EnemyRun(transform.position, MoveDirection, TargetPoint);

            GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < targets.Length; i++)
            {
                if (Vector2.Distance(targets[i].transform.position, transform.position) < _lockDistance)
                {
                    _target = targets[i];
                    break;
                }
            }
            return;
        }

        float distance = Vector2.Distance(transform.position, TargetPoint);
        if (distance > _unlockDistance)
        {
            _target = null;
            return;
        }

        if (distance < WeaponScript.Range)
        {
            int hits = Physics2D.RaycastAll(transform.position, _target.transform.position, distance, _layerMask).Length;

            if (WeaponScript is ChargeWeaponScript && hits <= ((ChargeWeaponScript)WeaponScript).CollisionsAllowed)
            {
                CanAttack = true;
                ((ChargeWeaponScript)WeaponScript).ObstaclesEnemy = hits;
            }
            else if (hits == 0)
            {
                CanAttack = true;
            }
        }

        if (!CanAttack)
        {
            MoveSpeed = _runSpeed;
        }

        WeaponScript.EnemyRun(transform.position, (TargetPoint - transform.position).normalized, TargetPoint);
    }
}