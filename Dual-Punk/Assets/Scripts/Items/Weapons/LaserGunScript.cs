using System;
using System.Collections.Generic;
using UnityEngine;


public class LaserGunScript : WeaponScript
{
    [SerializeField] private GameObject _gunEndPoint;
    [SerializeField] private GameObject _startVFX;
    [SerializeField] private GameObject _endVFX;
    [SerializeField] private LayerMask _layerMask;

    [SerializeField] private float _damage;
    [SerializeField] private float _damageFrequency;
    [SerializeField] private float _fireTime;
    [SerializeField] private float _coolDownSpeed;
    [SerializeField] private float _smoothTime;
    [SerializeField] private float _disableSpeed;
    
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _fireSound;

    private List<ParticleSystem> _particles;
    private LineRenderer _lineRenderer;
    private Vector3 _startPosition;
    
    private bool _fire;
    private bool _coolDown;
    private float _damageTimer;
    private float _coolDownLevel;
    private float _laserLength;
    private float _resetTimer;
    private float _velocity;


    public override bool DisplayInfo { get => _coolDownLevel > 0; }
    public override float InfoMaxTime { get => _fireTime; }
    public override float InfoTimer { get => _fireTime - _coolDownLevel; }


    private new void Start()
    {
        base.Start();

        _velocity = 0;
        _resetTimer = 0;
        _laserLength = 0;
        _coolDownLevel = 0;
        _fire = false;
        _coolDown = false;
        _damageTimer = _damageFrequency;

        _lineRenderer = GetComponentInChildren<LineRenderer>();
        _particles = new List<ParticleSystem>();

        FillList();
        DisableLaser();
    }


    private new void Update()
    {
        base.Update();

        if (!InHand) return;

        if (_coolDown && Input.GetButton("Use") || !_canAttack)
            PlayerState.PointerScript.CanShoot = false;
        else
            PlayerState.PointerScript.CanShoot = true;
        

        if (_fire)
        {
            _coolDownLevel += Time.deltaTime;

            if (_coolDownLevel > _fireTime)
            {
                _coolDownLevel = _fireTime;
                _coolDown = true;
                _fire = false;
            }
        }
        else
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
            if (_resetTimer >= _smoothTime * _disableSpeed)
            {
                DisableLaser();
            }
        }        
    }



    public override void Run(Vector3 position, Vector3 direction)
    {
        _startPosition = _gunEndPoint.transform.position;

        if (Input.GetButtonDown("Use") && _canAttack)
        {
            _fire = true;
            _coolDown = false;
            EnableLaser();
        }
        else if (Input.GetButtonUp("Use") || !_canAttack)
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
        
        _audioSource.clip = _fireSound;
        _audioSource.volume = 1;
        _audioSource.Play();
    }

    private void DisableLaser()
    {
        _laserLength = 0;
        _lineRenderer.enabled = false;

        foreach (ParticleSystem particleSystem in _particles)
        {
            particleSystem.Stop();
        }
        
        _audioSource.Stop();
    }


    public override void ResetWeapon()
    {
        _fire = false;
        _coolDown = true;
        DisableLaser();
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

            RaycastHit2D hit = Physics2D.Raycast(_startPosition, direction, 100, _layerMask);

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
                DrawLaser(PlayerState.MousePosition.normalized * 100, direction);
            }
        }
        else if (_resetTimer < _smoothTime * _disableSpeed)
        {
            _resetTimer += Time.deltaTime;
            _audioSource.volume = 1 - _resetTimer / (_smoothTime * _disableSpeed);
            DrawLaser(_startPosition, direction);
        }
    }
}