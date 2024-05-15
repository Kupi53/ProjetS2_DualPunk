using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(EnemyState))]
public class EnemyWeaponHandler : NetworkBehaviour
{
    [SerializeField] private GameObject[] _weapons;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _aimVariationAngleRange;
    [SerializeField] private float _smoothTime;

    private EnemyState _enemyState;
    private WeaponScript _weaponScript;
    private float _currentAngleOffset;
    private float _targetAngleOffset;
    private float _offsetTimer;
    private float _velocity;
    private int _weaponIndex;


    private void Start()
    {
        _velocity = 0;
        _weaponIndex = 0;
        _offsetTimer = _smoothTime;
        _enemyState = GetComponent<EnemyState>();

        AssignWeapon();
    }


    private void Update()
    {
        if (!_weaponScript.InHand)
        {
            _weaponScript.PickUp();
        }

        if (_offsetTimer < _smoothTime)
        {
            _currentAngleOffset = Mathf.SmoothDamp(_currentAngleOffset, _targetAngleOffset, ref _velocity, _smoothTime);
            _offsetTimer += Time.deltaTime;
        }
        else
        {
            _targetAngleOffset = UnityEngine.Random.Range(-_aimVariationAngleRange, _aimVariationAngleRange);
            _offsetTimer = 0;
        }
        
        if (_enemyState.Target == null)
        {
            _enemyState.Attack = false;
            _weaponScript.EnemyRun(transform.position, VariateDirection(Vector3.right), _enemyState.TargetPoint);

            return;
        }


        Vector3 direction = _enemyState.TargetPoint - transform.position;
        float distance = direction.magnitude;
        bool canAttack = false;

        if (distance < _weaponScript.Range)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, _enemyState.Target.transform.position, distance, _layerMask);

            if (_weaponScript is ChargeWeaponScript)
            {
                canAttack = true;
                ((ChargeWeaponScript)_weaponScript).ChargeMax = hit;
            }
            else if (!hit)
            {
                canAttack = true;
            }
        }

        _enemyState.Run = !canAttack;
        _enemyState.Attack = canAttack;

        _weaponScript.EnemyRun(transform.position, VariateDirection(direction), _enemyState.TargetPoint);
    }


    private Vector3 VariateDirection(Vector3 direction)
    {
        return Quaternion.Euler(0, 0, _currentAngleOffset) * direction.normalized;
    }


    public void AssignWeapon()
    {
        if (_weaponIndex < _weapons.Length)
        {
            GameObject weapon = Instantiate(_weapons[_weaponIndex], transform.position, Quaternion.identity);
            Spawn(weapon);

            _weaponScript = weapon.GetComponent<WeaponScript>();
            _weaponScript.UserRecoil = GetComponent<IImpact>();
            _weaponScript.EnemyState = _enemyState;

            _weaponIndex++;
        }
    }

    public void DropWeapon()
    {
        _weaponScript.Drop();
        _weaponScript.EnemyState = null;
    }
}