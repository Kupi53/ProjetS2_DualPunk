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

    private new void Update()
    {
        base.Update();

        if (InHand && !Reloading)
        {
            PlayerState.PointerScript.SpriteNumber = 2;

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
                    Reset();
            }
            else
                _resetTimer = 0;
        }
    }


    public bool ContainsTarget(GameObject target)
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
        base.Reset();

        foreach (GameObject target in _targetsIndicators)
        {
            Destroy(target);
        }
        _targetsIndicators.Clear();
    }

    public override void Fire(Vector3 direction, int damage, float bulletSpeed, float dispersion)
    {
        FireSeekingBulletRpc(_bullet, transform.rotation, direction, damage, bulletSpeed, dispersion, PlayerState, ClientManager.Connection);
    }

    [ServerRpc(RequireOwnership = false)]
    public void FireSeekingBulletRpc(GameObject bullet, Quaternion rot, Vector3 direction, int damage, float bulletSpeed, float dispersion, PlayerState playerState, NetworkConnection networkConnection)
    {
        if (playerState.Walking)
            dispersion /= _aimAccuracy;

        for (int i = 0; i < _bulletNumber; i++)
        {
            GameObject newBullet = Instantiate(bullet, _gunEndPoints[i % _gunEndPoints.Length].transform.position, rot);
            SeekingBulletScript bulletScript = newBullet.GetComponent<SeekingBulletScript>();

            Vector3 newDirection = new Vector3(direction.x + Methods.NextFloat(-dispersion, dispersion), direction.y + Methods.NextFloat(-dispersion, dispersion), 0).normalized;

            bulletScript.Setup(damage, bulletSpeed, newDirection, _bulletRotateSpeed);
            Spawn(newBullet);
            AssignTargetClientRPC(bulletScript, playerState, networkConnection);
        }
    }

    [ObserversRpc]
    void AssignTargetClientRPC(SeekingBulletScript bulletScript, PlayerState playerState, NetworkConnection networkConnection){
        if (!networkConnection.IsLocalClient) return;
        Debug.Log(playerState);
        Debug.Log(playerState.PointerScript);
        if (_targetsIndicators.Count == 0)
        {
            AssignTargetBulletScriptRPC(bulletScript, null)
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

    [ServerRpc (RequireOwnership = false)]
    void AssignTargetBulletScriptRPC(SeekingBulletScript bulletScript, GameObject? target){
        bulletScript.Target = target;
    }
}