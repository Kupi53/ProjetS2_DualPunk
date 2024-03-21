using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class LaserGunScript : WeaponScript
{
    [SerializeField] private GameObject _gunEndPoint;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private float _coolDownSpeed;
    [SerializeField] private float _coolDownTime;

    private float _coolDownLevel;
    private bool _coolDown;

    public override bool DisplayInfo { get => _coolDownLevel > 0; }
    public override float InfoMaxTime { get => _coolDownTime; }
    public override float InfoTimer { get => _coolDownLevel; }


    void Start()
    {
        _coolDownLevel = 0;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }


    void Update()
    {
        if (_coolDown)
        {
            _coolDownLevel -= Time.deltaTime * _coolDownSpeed;

            if (_coolDownLevel < 0)
            {
                _coolDownLevel = 0;
                _coolDown = false;
            }
        }
    }


    public override void Run(Vector3 position, Vector3 direction)
    {
        MovePosition(position, direction, _weaponOffset, _weaponDistance);

        if (Input.GetButton("Use") && _coolDownLevel < _coolDownTime)
        {
            Fire(true);
            _coolDownLevel += Time.deltaTime * _coolDownSpeed;
        }
        else if (Input.GetButtonUp("Use") && _coolDownLevel > 0)
        {
            Fire(false);
            _coolDown = true;
        }
    }


    public override void Reset()
    {
        _coolDown = true;
    }


    protected void MovePosition(Vector3 position, Vector3 direction, Vector3 weaponOffset, float weaponDistance)
    {
        float angle = (float)(Math.Atan2(direction.y, direction.x) * (180 / Math.PI));

        if (angle > 90 || angle < -90)
        {
            _spriteRenderer.flipY = true;
            weaponOffset.x = -weaponOffset.x;
        }
        else
            _spriteRenderer.flipY = false;

        transform.position = position + weaponOffset + direction * weaponDistance;
        transform.eulerAngles = new Vector3(0, 0, angle);
    }


    private void Fire(bool display)
    {
        if (display)
        {
            _lineRenderer.enabled = true;
            _lineRenderer.SetPosition(1, _gunEndPoint.transform.position);
            _lineRenderer.SetPosition(0, PlayerState.MousePosition);
        }
        else
            _lineRenderer.enabled = false;
    }
}