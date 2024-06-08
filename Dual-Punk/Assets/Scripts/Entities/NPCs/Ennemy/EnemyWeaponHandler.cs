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
    private Vector3 _direction;

    private float _currentAngleOffset;
    private float _targetAngleOffset;
    private float _offsetTimer;
    private float _velocity;
    private int _weaponIndex;
    private Vector3 _lastTargetPointBeforeStun;
    private Vector3 _lastVariableDirectionBeforeStun;
    private bool _hasLastTargetPointBeforeStun;

    private void Start()
    {
        _velocity = 0;
        _weaponIndex = 0;
        _offsetTimer = _smoothTime;
        _direction = Vector3.right;
        _enemyState = GetComponent<EnemyState>();
        _hasLastTargetPointBeforeStun = false;

        AssignWeapon();
    }


    private void Update()
    {
        if (!_weaponScript.InHand)
        {
            _weaponScript.PickUp(gameObject);
            return;
        }

        if (_offsetTimer < _smoothTime)
        {
            _currentAngleOffset = Mathf.SmoothDamp(_currentAngleOffset, _targetAngleOffset, ref _velocity, _smoothTime);
            _offsetTimer += Time.deltaTime;
        }
        else
        {
            _targetAngleOffset = Random.Range(-_aimVariationAngleRange, _aimVariationAngleRange);
            _offsetTimer = 0;
        }
        
        if (_enemyState.Target == null || _enemyState.Stop)
        {
            _enemyState.CanAttack = false;
            _weaponScript.EnemyRun(transform.position, VariateDirection(), _enemyState.TargetPoint);
            return;
        }


        _direction = _enemyState.TargetPoint - transform.position;
        float distance = _direction.magnitude;
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

        _enemyState.CanAttack = canAttack;

        if (_enemyState.Stun)
        {
            if (!_hasLastTargetPointBeforeStun)
            {
                _lastTargetPointBeforeStun = _enemyState.TargetPoint;
                _lastVariableDirectionBeforeStun = VariateDirection();
                _hasLastTargetPointBeforeStun = true;
            }
                
            _weaponScript.MovePosition(transform.position, _lastVariableDirectionBeforeStun, _lastTargetPointBeforeStun);
        }
        else
        {
            _weaponScript.EnemyRun(transform.position, VariateDirection(), _enemyState.TargetPoint);
            _hasLastTargetPointBeforeStun = false;
        }
            


        //used to flip the ennemy sprite renderer and animation
        bool flippedRenderer = gameObject.GetComponent<SpriteRenderer>().flipX;

        if (_direction.x < 0 && !flippedRenderer) {
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (_direction.x > 0 && flippedRenderer) {
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
    }


    private Vector3 VariateDirection()
    {
        return Quaternion.Euler(0, 0, _currentAngleOffset) * _direction.normalized;
    }


    public void AssignWeapon()
    {
        if (_weaponIndex >= _weapons.Length) return;
        if (_weaponIndex > 0) DropWeapon();
        
        GameObject weapon = Instantiate(_weapons[_weaponIndex], transform.position, Quaternion.identity);
        weapon.transform.SetParent(this.gameObject.transform);
        Spawn(weapon);

        _weaponScript = weapon.GetComponent<WeaponScript>();
        _weaponIndex++;
    }

    public void DropWeapon()
    {
        _weaponScript.Drop();
        _weaponScript = null;
    }
}