using FishNet.Object;
using System;
using System.Collections.Generic;
using FishNet.Demo.AdditiveScenes;
using GameKit.Utilities;
using UnityEngine;


public class LaserGunScript : FireArmScript
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private GameObject _gunEndPoint;
    [SerializeField] private GameObject _startVFX;
    [SerializeField] private GameObject _endVFX;
    [SerializeField] private LayerMask _layerMask;

    [SerializeField] private float _fireTime;
    [SerializeField] private float _coolDownSpeed;
    [SerializeField] private float _smoothTime;
    [SerializeField] private float _disablingSpeed;
    [SerializeField] private int _hitProjectileCount;

    private List<ParticleSystem> _particles;
    private LineRenderer _lineRenderer;
    private Vector3 _startPosition;
    
    private bool _fire;
    private bool _coolDown;
    private bool _disableFire;
    private float _damageTimer;
    private float _coolDownLevel;
    private float _laserLength;
    private float _resetTimer;
    private float _velocity;
    private int _hitProjectileCounter;
    
    // Implant static amplifier
    private Color _oldColor;
    private float _staticAmplifierFactor;

    public float FireTime { get => _fireTime; set => _fireTime = value; }
    public float CoolDownLevel { get => _coolDownLevel; set => _coolDownLevel = value; }

    public override bool DisplayInfo { get => _coolDownLevel > 0; }
    public override float InfoMaxTime { get => _fireTime; }
    public override float InfoTimer { get => _fireTime - _coolDownLevel; }



    private void Start()
    {
        _fire = false;
        _coolDown = false;
        _disableFire = false;

        _velocity = 0;
        _resetTimer = 0;
        _laserLength = 0;
        _coolDownLevel = 0;
        _hitProjectileCounter = 0;
        _staticAmplifierFactor = 0;

        _damageTimer = _fireRate;
        _particles = new List<ParticleSystem>();
        _lineRenderer = GetComponentInChildren<LineRenderer>();
        _oldColor = _lineRenderer.gameObject.GetComponent<Renderer>().material.GetColor("_Color");

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

        DisableLaser();
        DisableLaserServerRPC();
    }


    private new void Update()
    {
        base.Update();

        _startPosition = _gunEndPoint.transform.position;

        if (_fire)
        {
            _coolDownLevel += Time.deltaTime;

            if (_coolDownLevel > _fireTime)
            {
                _coolDownLevel = _fireTime;
                _fire = false;
                _coolDown = true;
                _disableFire = true;
            }
        }
        else
        {
            if (_coolDownLevel > 0)
            {
                _coolDown = true;
            }
            if (_coolDown)
            {
                _coolDownLevel -= Time.deltaTime * _coolDownSpeed;

                if (_coolDownLevel < 0)
                {
                    _coolDownLevel = 0;
                    _coolDown = false;
                    _disableFire = false;
                }
            }
        }        
    }


    public override void Run(Vector3 position, Vector3 direction, Vector3 targetPoint)
    {
        base.Run(position, direction, targetPoint);
        
        if (_disableFire && Input.GetButton("Use"))
            PlayerState.PointerScript.CanShoot = false;

        if (Input.GetButtonDown("Use") && _canAttack)
        {
            _coolDown = false;
            _disableFire = false;
        }
        if (Input.GetButton("Use") && _canAttack && !_fire && !_disableFire)
        {
            _fire = true;

            EnableLaser();
            EnableLaserServerRPC();
        }
        else if (Input.GetButtonUp("Use") || !_canAttack)
        {
            _fire = false;
            _coolDown = true;
        }

        if (_fire)
        {
            PlayerState.CameraController.ShakeCamera(_cameraShake, 0.1f);
            Fire(direction, _damage, 0, Vector3.Distance(_startPosition, targetPoint), false);
        }
        else
        {
            ResetLaser(direction);
        }
    }


    public override void EnemyRun(Vector3 position, Vector3 direction, Vector3 targetPoint)
    {
        MovePosition(position, direction, targetPoint);

        if (!_disableFire && EnemyState.CanAttack)
        {
            if (!_lineRenderer.enabled)
                EnableLaserServerRPC();
            _fire = true;
        }
        if (!EnemyState.CanAttack)
        {
            _fire = false;
            _disableFire = false;

            if (_coolDownLevel > 0)
                _coolDown = true;
        }

        if (_fire)
        {
            Fire(direction, _damage, 0, Vector3.Distance(_startPosition, targetPoint), true);
        }
        else
        {
            ResetLaser(direction);
        }
    }



    public void ChangeLaser(bool staticAmplifier, float damageMultiplier)
    {
        if (staticAmplifier)
        {
            _staticAmplifierFactor = damageMultiplier;
            _lineRenderer.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);

            foreach (var particule in _particles)
                particule.gameObject.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.red);
        }
        else
        {
            _staticAmplifierFactor = 0;
            _lineRenderer.gameObject.GetComponent<Renderer>().material.SetColor("_Color", _oldColor);

            foreach (var particule in _particles)
                particule.gameObject.GetComponent<Renderer>().material.SetColor("_BaseColor", _oldColor);
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

    private void DrawLaser(Vector3 startPosition, Vector3 direction, float distance)
    {
        _laserLength = Mathf.SmoothDamp(_laserLength, distance, ref _velocity, _smoothTime);
        _lineRenderer.SetPosition(0, startPosition);
        _lineRenderer.SetPosition(1, startPosition + direction * _laserLength);
        _endVFX.transform.position = startPosition + direction * _laserLength;
    }



    public override void ResetWeapon()
    {
        _fire = false;
        _coolDown = true;
        _hitProjectileCounter = 0;

        if (PlayerState != null)
            DisableLaser();
        DisableLaserServerRPC();
    }


    protected override void Fire(Vector3 direction, int damage, float dispersion, float distance, bool damagePlayer)
    {
        if (_damageTimer < _fireRate)
            _damageTimer += Time.deltaTime;

        UserRecoil.Impact(-direction, _recoilForce * Time.deltaTime * 100);
        RaycastHit2D hit = Physics2D.Raycast(_startPosition, direction, distance, _layerMask);

        if (hit)
        {
            distance = hit.distance;

            if (_damageTimer >= _fireRate)
            {
                _damageTimer = 0;
                if (_staticAmplifierFactor > 0)
                    damage = (int)(damage * _staticAmplifierFactor);

                if (hit.collider.CompareTag("Projectile"))
                {
                    if (_hitProjectileCounter < _hitProjectileCount)
                    {
                        _hitProjectileCounter++;
                    }
                    else
                    {
                        _hitProjectileCounter = 0;
                        hit.collider.GetComponent<IDestroyable>().DestroyObject();
                    }
                }
                else if (hit.collider.CompareTag("Ennemy") && !damagePlayer || hit.collider.CompareTag("Player") && damagePlayer)
                {
                    hit.collider.GetComponent<IDamageable>().Damage(damage, _fireRate, _staticAmplifierFactor > 0);
                    hit.collider.GetComponent<IImpact>().Impact(direction, _impactForce);
                }
            }
        }

        if (PlayerState != null)
            DrawLaser(_startPosition, direction, distance);
        DrawLaserServerRPC(_startPosition, direction, distance);
    }


    private void ResetLaser(Vector3 direction)
    {
        if (_resetTimer < _smoothTime * _disablingSpeed)
        {
            _hitProjectileCounter = 0;
            _resetTimer += Time.deltaTime;
            _audioSource.volume = 1 - _resetTimer / (_smoothTime * _disablingSpeed);

            if (PlayerState != null)
                DrawLaser(_startPosition, direction, 0);
            DrawLaserServerRPC(_startPosition, direction, 0);
        }
        else if (_lineRenderer.enabled)
        {
            if (PlayerState != null)
                DisableLaser();
            DisableLaserServerRPC();
        }
    }


    // --------------------------------------------------


    [ServerRpc(RequireOwnership = false)]
    private void EnableLaserServerRPC()
    {
        if (PlayerState != null)
            EnableLaserClient();
        else
            EnableLaserObservers();
    }

    [ObserversRpc]
    private void EnableLaserObservers()
    {
        EnableLaser();
    }

    [ObserversRpc(ExcludeOwner = true)]
    private void EnableLaserClient()
    {
        EnableLaser();
    }



    [ServerRpc(RequireOwnership = false)]
    private void DisableLaserServerRPC()
    {
        if (PlayerState != null)
            DisableLaserClient();
        else
            DisableLaserObservers();
    }

    [ObserversRpc]
    private void DisableLaserObservers()
    {
        DisableLaser();
    }

    [ObserversRpc(ExcludeOwner = true)]
    private void DisableLaserClient()
    {
        DisableLaser();
    }



    [ServerRpc(RequireOwnership = false)]
    private void DrawLaserServerRPC(Vector3 startPosition, Vector3 direction, float distance)
    {
        if (PlayerState != null)
            DrawLaserClient(startPosition, direction, distance);
        else
            DrawLaserObservers(startPosition, direction, distance);
    }

    [ObserversRpc]
    private void DrawLaserObservers(Vector3 startPosition, Vector3 direction, float distance)
    {
        DrawLaser(startPosition, direction, distance);
    }

    [ObserversRpc(ExcludeOwner = true)]
    private void DrawLaserClient(Vector3 startPosition, Vector3 direction, float distance)
    {
        DrawLaser(startPosition, direction, distance);
    }
}