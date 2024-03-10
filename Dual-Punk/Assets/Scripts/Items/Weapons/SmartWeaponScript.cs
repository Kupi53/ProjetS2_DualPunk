using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using System;
using Unity.Netcode;


public class SmartWeaponScript : FireArmScript
{
    [SerializeField] private GameObject _lockedTargetIndicator;
    [SerializeField] private float _bulletRotateSpeed;

    private List<GameObject> _targetsIndicators;
    private GameObject _newTargetIndicator;

    private float _resetTimer;
    private int _index;


    private new void Start()
    {
        base.Start();

        _index = 0;
        _resetTimer = 0;
        _targetsIndicators = new List<GameObject>();
    }

    private new void Update()
    {
        base.Update();

        if (InHand && !Reloading)
        {
            PointerScript.SpriteNumber = 2;

            if (Input.GetButtonUp("Switch"))
            {
                if (_resetTimer > 0.2)
                {
                    Reset();
                }
                else if (PointerScript.Target != null)
                {
                    _newTargetIndicator = Instantiate(_lockedTargetIndicator);
                    _newTargetIndicator.GetComponent<TargetIndicatorScript>().Target = PointerScript.Target;
                    _targetsIndicators.Add(_newTargetIndicator);
                }
            }

            if (Input.GetButton("Switch"))
                _resetTimer += Time.deltaTime;
            else
                _resetTimer = 0;
        }
    }


    public override void Reset()
    {
        base.Reset();

        foreach (GameObject target in _targetsIndicators)
        {
            Destroy(target);
        }
        _targetsIndicators.Clear();
    }


    public void AssignTarget(SeekingBulletScript bulletScript)
    {
        if (PointerScript.Target != null)
            bulletScript.Target = PointerScript.Target;
        else if (_targetsIndicators.Count == 0)
            bulletScript.Target = null;
        else
        {
            _index = (_index + 1) % _targetsIndicators.Count;

            if (_targetsIndicators[_index] == null)
            {
                _targetsIndicators.Remove(_targetsIndicators[_index]);
                AssignTarget(bulletScript);
            }
            else
            {
                bulletScript.Target = _targetsIndicators[_index].GetComponent<TargetIndicatorScript>().Target;
            }
        }
    }


    public override void Fire(Vector3 direction, int damage, float bulletSpeed, float dispersion)
    {
        if (PlayerState.Walking)
            dispersion /= _aimAccuracy;

        for (int i = 0; i < _bulletNumber; i++)
        {
            Vector3 newDirection = new Vector3(direction.x + Methods.NextFloat(-dispersion, dispersion), direction.y + Methods.NextFloat(-dispersion, dispersion), 0).normalized;
            float newAngle = (float)(Math.Atan2(newDirection.y, newDirection.x) * (180 / Math.PI));

            GameObject newBullet = Instantiate(_bullet, _gunEndPoints[i % _gunEndPoints.Length].transform.position, transform.rotation);
            SeekingBulletScript bulletScript = newBullet.GetComponent<SeekingBulletScript>();

            bulletScript.Damage = damage;
            bulletScript.MoveSpeed = bulletSpeed;
            bulletScript.MoveDirection = newDirection;
            bulletScript.RotateSpeed = _bulletRotateSpeed;

            AssignTarget(bulletScript);

            newBullet.transform.eulerAngles = new Vector3(0, 0, newAngle);
            NetworkObject bulletNetwork = newBullet.GetComponent<NetworkObject>();
            bulletNetwork.Spawn();
        }
    }
}