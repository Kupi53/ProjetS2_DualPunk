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


    private void Start()
    {
        _weaponIndex = 0;

        GameObject weapon = Instantiate(_weapons[0], transform.position, Quaternion.identity);
        Spawn(weapon);

        WeaponScript = weapon.GetComponent<WeaponScript>();
        WeaponScript.PickUp();
    }


    private void Update()
    {
        if (_target == null)
        {
            CanAttack = false;
            WeaponScript.EnemyRun(this, transform.position, MoveDirection, TargetPoint);

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

        float distance = Vector2.Distance(transform.position, _target.transform.position);
        if (distance < _unlockDistance)
        {
            _target = null;
            return;
        }

        if (WeaponScript is not ChargeWeaponScript)
        {
            CanAttack = Physics2D.Raycast(transform.position, _target.transform.position, distance, _layerMask);
        }

        WeaponScript.EnemyRun(this, transform.position, transform.position - _target.transform.position, TargetPoint);
    }
}