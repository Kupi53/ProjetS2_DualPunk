using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using System;
using Unity.Netcode;
using FishNet.Object;
using FishNet.Connection;


public class SmartWeaponScript : FireArmScript
{
    [SerializeField] private GameObject _lockedTargetIndicator;
    [SerializeField] private float _bulletRotateSpeed;

    private List<GameObject> _targetsIndicators;
    private float _resetTimer;
    private int _index;


    private new void Start()
    {
        base.Start();

        _index = 0;
        _resetTimer = 0;
        _targetsIndicators = new List<GameObject>();
    }


    public override void Run(Vector3 position, Vector3 direction)
    {
        if (Input.GetButtonUp("Switch") && !ContainsTarget(PlayerState.PointerScript.Target))
        {
            GameObject newTargetIndicator = Instantiate(_lockedTargetIndicator);
            newTargetIndicator.GetComponent<TargetIndicatorScript>().Target = PlayerState.PointerScript.Target;
            _targetsIndicators.Add(newTargetIndicator);
        }
        else if (Input.GetButton("Switch"))
        {
            _resetTimer += Time.deltaTime;

            if (_resetTimer > 0.25)
                ResetWeapon();
        }
        else
            _resetTimer = 0;
        
        base.Run(position, direction);
    }


    private bool ContainsTarget(GameObject target)
    {
        if (target == null)
            return true;

        for (int i = 0; i < _targetsIndicators.Count; i++)
        {
            if (_targetsIndicators[i] == null || _targetsIndicators[i].GetComponent<TargetIndicatorScript>().Target == target)
            {
                Destroy(_targetsIndicators[i]);
                _targetsIndicators.Remove(_targetsIndicators[i]);
                return true;
            }
        }
        return false;
    }


    public override void ResetWeapon()
    {
        base.ResetWeapon();

        for (int i = 0; i < _targetsIndicators.Count; i++)
        {
            Destroy(_targetsIndicators[i]);
        }
        _targetsIndicators.Clear();
    }


    public override void Fire(Vector3 direction, int damage, float bulletSpeed, float bulletSize, float impactForce, float dispersion, int collisionsAllowed)
    {
        _ammoLeft--;
        _fireTimer = 0;
        AudioManager.Instance.PlayClipAt(_fireSound, gameObject.transform.position);

        if (PlayerState.Walking)
            dispersion /= _aimAccuracy;

        FireSeekingBulletRpc(PlayerState, ClientManager.Connection, direction, damage, bulletSpeed, impactForce, dispersion);
    }


    [ServerRpc(RequireOwnership = false)]
    private void FireSeekingBulletRpc(PlayerState playerState, NetworkConnection networkConnection, Vector3 direction, int damage, float bulletSpeed, float impactForce, float dispersion)
    {
        for (int i = 0; i < _bulletNumber; i++)
        {
            GameObject newBullet = Instantiate(_bullet, _gunEndPoints[_bulletPointIndex].transform.position, Quaternion.identity);

            if (_warriorLuckBullet)
            {
                SpriteRenderer bulletRenderer = newBullet.GetComponent<SpriteRenderer>();

                if (bulletRenderer != null)
                {
                    bulletRenderer.color = new Color(255f, 0f, 0f, 255f);
                }

                _warriorLuckBullet = false;
            }
            
            SeekingBulletScript bulletScript = newBullet.GetComponent<SeekingBulletScript>();

            _bulletPointIndex = (_bulletPointIndex + 1) % _gunEndPoints.Length;
            newBullet.transform.localScale = new Vector2(_bulletSize, _bulletSize);
            Vector3 newDirection = new Vector3(direction.x + Methods.NextFloat(-dispersion, dispersion), direction.y + Methods.NextFloat(-dispersion, dispersion), 0).normalized;

            bulletScript.Setup(newDirection, damage, bulletSpeed, impactForce, _bulletRotateSpeed);
            Spawn(newBullet);
            AssignTargetClientRPC(bulletScript, playerState, networkConnection);
        }
    }


    [ObserversRpc]
    private void AssignTargetClientRPC(SeekingBulletScript bulletScript, PlayerState playerState, NetworkConnection networkConnection)
    {
        if (!networkConnection.IsLocalClient) return;

        if (_targetsIndicators.Count == 0)
        {
            AssignTargetBulletScriptRPC(bulletScript, null);
        }
        else
        {
            _index = (_index + 1) % _targetsIndicators.Count;

            if (_targetsIndicators[_index] == null)
            {
                _targetsIndicators.Remove(_targetsIndicators[_index]);
                AssignTargetClientRPC(bulletScript, playerState, networkConnection);
            }
            else
            {
                AssignTargetBulletScriptRPC(bulletScript,_targetsIndicators[_index].GetComponent<TargetIndicatorScript>().Target);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AssignTargetBulletScriptRPC(SeekingBulletScript bulletScript, GameObject target)
    {
        if (bulletScript != null)
            bulletScript.Target = target;
    }
}