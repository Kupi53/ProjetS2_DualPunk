using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using UnityEngine;
using UnityEngine.Playables;


//Ce script va gerer les attaques, des implants ou des armes
public class AttacksController : NetworkBehaviour
{
    [SerializeField] private GameObject _targetIndicatorPrefab;

    private PlayerState _playerState;
    private Vector3 _direction;

    // Implant LaserTracker
    private GameObject _targetIndicator;
    private Vector3 _vel;
    private bool _laserTracker;
    private float _lockingRange;
    private float _smoothTime;


    private void Start()
    {
        _playerState = GetComponent<PlayerState>();


        _vel = Vector3.zero;
        _laserTracker = false;
        _lockingRange = 0;
        _smoothTime = 0;
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (_playerState.HoldingWeapon)
        {
            Vector3 targetPoint; ;

            if (_laserTracker && _targetIndicator != null)
            {
                _direction = Vector3.SmoothDamp(_direction, (_targetIndicator.transform.position - transform.position - _playerState.WeaponScript.WeaponOffset).normalized, ref _vel, _smoothTime);
                targetPoint = _direction * Vector3.Distance(transform.position, _targetIndicator.transform.position);
            }
            else
            {
                _direction = (_playerState.MousePosition - transform.position - _playerState.WeaponScript.WeaponOffset).normalized;
                targetPoint = _playerState.MousePosition;
            }
                
            if (_laserTracker && Input.GetButtonDown("SecondaryUse"))
            {
                if (_targetIndicator != null)
                {
                    Destroy(_targetIndicator);
                }
                else
                {
                    GameObject target = GetNearestTarget(_playerState.MousePosition);

                    if (target != null)
                    {
                        _targetIndicator = Instantiate(_targetIndicatorPrefab);
                        _targetIndicator.GetComponent<TargetIndicatorScript>().Target = target;
                    }
                }
            }

            _playerState.AnimAngle = Methods.GetAngle(_direction);
            _playerState.WeaponScript.Run(transform.position, _direction, targetPoint, _playerState.EnablePvp);
        }
    }


    public void SetLaserTracker(bool enableTracker, float lockingRange, float smoothTime)
    {
        _laserTracker = enableTracker;
        _lockingRange = lockingRange;
        _smoothTime = smoothTime;
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
}