using System;
using UnityEngine;


public class KnifeScript : WeaponScript
{
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _attackDistance;

    private float _angle;
    private float _rangeTop;
    private float _rangeMiddle;
    private float _rangeBottom;
    private float _currentWeaponDistance;

    public float Attack { get; set; }

    public float ResetCooldown { get; set; }
    public float ResetCooldownTimer { get; set; }



    void Start()
    {
        Attack = 0;
        ResetCooldownTimer = 0;
        _currentWeaponDistance = WeaponDistance;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }


    public void Update()
    {
        //Faire des animations ici
        if (InHand)
        {
            PointerScript = PlayerState.Pointer.GetComponent<PointerScript>();
            PointerScript.SpriteNumber = 1;
        }
        else
            PointerScript = null;
    }


    public override void Run(Vector3 position, Vector3 direction)
    {
        if (Input.GetButtonDown("Use") && !PlayerState.Attacking && Attack < 3)
        {
            _angle = (float)(Math.Atan2(direction.y, direction.x) * (180 / Math.PI));

            Attack += 1;
            ResetCooldownTimer = 0;
            PlayerState.Attacking = true;

            _rangeMiddle = _angle;
            _rangeTop = _angle + _attackRange;
            _rangeBottom = _angle - _attackRange;

            if (Attack == 2)
            {
                _angle = _rangeBottom;
                _spriteRenderer.flipY = true;
            }
            else
            {
                _angle = _rangeTop;
                _spriteRenderer.flipY = false;
            }
        }

        if (PlayerState.Attacking)
        {
            switch (Attack)
            {
                case 1:
                    _angle -= _attackSpeed * Time.deltaTime;
                    if (_angle <= _rangeBottom)
                        PlayerState.Attacking = false;
                    break;

                case 2:
                    _angle += _attackSpeed * Time.deltaTime;
                    if (_angle >= _rangeTop)
                        PlayerState.Attacking = false;
                    break;

                case 3:
                    if (_angle < _rangeMiddle)
                        _angle -= _attackSpeed * Time.deltaTime;
                    else if (_currentWeaponDistance < _attackDistance)
                        _currentWeaponDistance += Time.deltaTime * 10;
                    else
                    {
                        PlayerState.Attacking = false;
                        _currentWeaponDistance = _attackDistance;
                    }
                    break;
            }
        }

        else
        {
            _angle = (float)(Math.Atan2(direction.y, direction.x) * (180 / Math.PI));

            switch (Attack)
            {
                case 1:
                    _angle -= _attackRange;
                    break;
                case 2:
                    _angle += _attackRange;
                    break;
                case 0:
                    if (_angle > 90 || _angle < -90)
                        _spriteRenderer.flipY = true;
                    else
                        _spriteRenderer.flipY = false;
                    break;
            }
        }


        if (Attack != 0)
        {
            ResetCooldownTimer += Time.deltaTime;
            if (ResetCooldownTimer > ResetCooldown)
            {
                Reset();
            }

            direction = new Vector3((float)Math.Cos(_angle * Math.PI / 180), (float)Math.Sin(_angle * Math.PI / 180)).normalized;
        }

        transform.position = position + WeaponOffset + direction * _currentWeaponDistance;
        transform.eulerAngles = new Vector3(0, 0, _angle);
    }


    public override void Reset()
    {
        Attack = 0;
        ResetCooldownTimer = 0;
        _currentWeaponDistance = WeaponDistance;
    }
}