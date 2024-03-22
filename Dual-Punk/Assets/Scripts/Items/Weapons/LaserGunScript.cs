using System;
using System.Collections.Generic;
using UnityEngine;


public class LaserGunScript : WeaponScript
{
    [SerializeField] private GameObject _gunEndPoint;
    [SerializeField] private GameObject _startVFX;
    [SerializeField] private GameObject _endVFX;

    [SerializeField] private float _coolDownSpeed;
    [SerializeField] private float _coolDownTime;
    [SerializeField] private float _smoothTime;
    [SerializeField] private LayerMask _layerMask;

    private List<ParticleSystem> _particles;
    private LineRenderer _lineRenderer;
    private Vector3 _startPosition;

    private bool _fire;
    private bool _coolDown;
    private float _coolDownLevel;

    private float _velocity;
    private float _resetTimer;
    private float _laserLength;

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

        FillList();
        DisableLaser();
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
        else if (_fire)
        {
            _coolDownLevel += Time.deltaTime * _coolDownSpeed / 3;

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
        if (!_coolDown)
            PlayerState.PointerScript.SpriteNumber = 1;
        else
            PlayerState.PointerScript.SpriteNumber = 0;

        MovePosition(position, direction, _weaponOffset, _weaponDistance);

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

        Fire(direction);
    }


    public override void Reset()
    {
        _fire = false;
        _coolDown = true;
        DisableLaser();
        _startVFX.SetActive(false);
        _laserLength = 0;
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
        _lineRenderer.enabled = true;

        foreach (ParticleSystem particleSystem in _particles)
        {
            particleSystem.Play();
        }
    }

    private void DisableLaser()
    {
        _lineRenderer.enabled = false;

        foreach (ParticleSystem particleSystem in _particles)
        {
            particleSystem.Stop();
        }
    }


    private void DrawLaser(Vector3 targetPoint, Vector3 direction)
    {
        _laserLength = Mathf.SmoothDamp(_laserLength, Vector3.Distance(targetPoint, _startPosition), ref _velocity, _smoothTime);
        _lineRenderer.SetPosition(0, _startPosition);
        _lineRenderer.SetPosition(1, _startPosition + direction * _laserLength);
    }


    private void Fire(Vector3 direction)
    {
        if (_fire)
        {
            _resetTimer = 0;

            RaycastHit2D hit = Physics2D.Raycast(_startPosition, direction, Vector3.Distance(PlayerState.MousePosition, _startPosition), _layerMask);
            if (hit)
                DrawLaser(hit.point, direction);
            else
                DrawLaser(PlayerState.MousePosition, direction);
        }
        else
        {
            if (_resetTimer < _smoothTime)
            {
                _resetTimer += Time.deltaTime;

                DrawLaser(_startPosition, direction);
            }
            else
            {
                _laserLength = 0;
                DisableLaser();
            }
        }
    }
}