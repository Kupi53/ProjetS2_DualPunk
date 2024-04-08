using System;
using System.Collections.Generic;
using UnityEngine;


public class LaserGunScript : WeaponScript
{
    [SerializeField] private GameObject _gunEndPoint;
    [SerializeField] private GameObject _startVFX;
    [SerializeField] private GameObject _endVFX;
    [SerializeField] private LayerMask _layerMask;

    [SerializeField] private float _damageFrequency;
    [SerializeField] private float _coolDownSpeed;
    [SerializeField] private float _coolDownTime;
    [SerializeField] private float _smoothTime;
    [SerializeField] private float _damage;

    private List<ParticleSystem> _particles;
    private LineRenderer _lineRenderer;
    private Vector3 _startPosition;

    private float _coolDownLevel;
    private float _damageTimer;
    private float _velocity;
    private float _resetTimer;
    private float _laserLength;
    private bool _coolDown;
    private bool _fire;


    public override bool DisplayInfo { get => _coolDownLevel > 0; }
    public override float InfoMaxTime { get => _coolDownTime; }
    public override float InfoTimer { get => _coolDownTime - _coolDownLevel; }


    void Start()
    {
        _velocity = 0;
        _resetTimer = 0;
        _laserLength = 0;
        _coolDownLevel = 0;

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _lineRenderer = GetComponentInChildren<LineRenderer>();
        _particles = new List<ParticleSystem>();

        _fire = false;
        _coolDown = false;
        _damageTimer = _damageFrequency;

        FillList();
        DisableLaser();
    }


    void Update()
    {
        if (!InHand)
        {
            transform.position += Vector3.up * (float)Math.Cos(Time.time * 5 + _coolDownTime) * 0.001f;
        }

        if (_coolDown)
        {
            _coolDownLevel -= Time.deltaTime * _coolDownSpeed;

            if (_coolDownLevel < 0)
            {
                _coolDownLevel = 0;
                _coolDown = false;
            }
        }
        else if (_fire)
        {
            _coolDownLevel += Time.deltaTime * _coolDownSpeed / 2;

            if (_coolDownLevel > _coolDownTime)
            {
                _coolDownLevel = _coolDownTime;
                _coolDown = true;
                _fire = false;
            }
        }
    }



    public override void Run(Vector3 position, Vector3 direction)
    {
        if (_coolDown && Input.GetButton("Use"))
            PlayerState.PointerScript.SpriteNumber = 0;
        else
            PlayerState.PointerScript.SpriteNumber = 1;

        _startPosition = _gunEndPoint.transform.position;

        if (Input.GetButtonDown("Use"))
        {
            _fire = true;
            _coolDown = false;
            EnableLaser();
        }
        else if (Input.GetButtonUp("Use"))
        {
            _fire = false;
            _coolDown = true;
        }

        MovePosition(position, direction);
        Fire(direction);
    }


    private void FillList()
    {
        for (int i = 0; i < _startVFX.transform.childCount; i++)
        {
            ParticleSystem particleSystem = _startVFX.transform.GetChild(i).GetComponent<ParticleSystem>();
            if (particleSystem != null)
                _particles.Add(particleSystem);
        }
        for (int i = 0; i < _endVFX.transform.childCount; i++)
        {
            ParticleSystem particleSystem = _endVFX.transform.GetChild(i).GetComponent<ParticleSystem>();
            if (particleSystem != null)
                _particles.Add(particleSystem);
        }
    }

    private void EnableLaser()
    {
        _resetTimer = 0;
        _lineRenderer.enabled = true;

        foreach (ParticleSystem particleSystem in _particles)
        {
            particleSystem.Play();
        }
    }

    private void DisableLaser()
    {
        _laserLength = 0;
        _lineRenderer.enabled = false;

        foreach (ParticleSystem particleSystem in _particles)
        {
            particleSystem.Stop();
        }
    }


    public override void ResetWeapon()
    {
        _fire = false;
        _coolDown = true;
        DisableLaser();
    }

    public override void Drop()
    {
        ResetWeapon();
        InHand = false;
        transform.position = PlayerState.transform.position + PlayerState.WeaponScript.WeaponOffset;
        transform.rotation = Quaternion.identity;

        if (_spriteRenderer.flipY)
        {
            _weaponOffset.x = -_weaponOffset.x;
            _spriteRenderer.flipY = false;
        }
    }


    private void DrawLaser(Vector3 targetPoint, Vector3 direction)
    {
        _laserLength = Mathf.SmoothDamp(_laserLength, Vector3.Distance(targetPoint, _startPosition), ref _velocity, _smoothTime);
        _lineRenderer.SetPosition(0, _startPosition);
        _lineRenderer.SetPosition(1, _startPosition + direction * _laserLength);
        _endVFX.transform.position = _startPosition + direction * _laserLength;
    }


    private void Fire(Vector3 direction)
    {
        if (_fire)
        {
            if (_damageTimer < _damageFrequency)
                _damageTimer += Time.deltaTime;

            PlayerRecoil.Impact(-direction, _recoilForce * Time.deltaTime * 100);
            PlayerState.CameraController.ShakeCamera(_cameraShake, 0.1f);

            RaycastHit2D hit = Physics2D.Raycast(_startPosition, direction, Vector3.Distance(PlayerState.MousePosition, _startPosition), _layerMask);

            if (hit)
            {
                DrawLaser(hit.point, direction);
                
                if (hit.collider.CompareTag("Ennemy") && _damageTimer >= _damageFrequency)
                {
                    EnnemyState health = hit.collider.GetComponent<EnnemyState>();
                    health.OnDamage(_damage);
                    _damageTimer = 0;
                }
            }
            else
            {
                DrawLaser(PlayerState.MousePosition, direction);
            }
        }
        else if (_resetTimer < _smoothTime*3)
        {
            _resetTimer += Time.deltaTime;
            DrawLaser(_startPosition, direction);

            if (_resetTimer >= _smoothTime * 3)
                DisableLaser();
        }
    }
}