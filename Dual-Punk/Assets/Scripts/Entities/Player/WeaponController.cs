using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using Unity.Netcode;


public class WeaponController : NetworkBehaviour
{
    public GameObject pointer;
    public GameObject reloadCooldown;
    public PlayerState playerState;


    private void Start()
    {
        playerState = gameObject.GetComponent<PlayerState>();
    }


    public bool HoldWeapon(bool hold)
    {
        if (!hold)
        {
            playerState.HoldingWeapon = false;
            return true;
        }
        else if (hold && !playerState.HoldingWeapon)
        {
            playerState.HoldingWeapon = true;
            return false;
        }
        return hold;
    }


    public static float NextFloat(float min, float max)
    {
        System.Random random = new System.Random();
        double val = random.NextDouble() * (max - min) + min;
        return (float)val;
    }

    
    public void FireRound(GameObject bullet, GameObject gunEnd, Vector3 direction, float dispersion, int bulletNumber)
    {
        for (int i = 0; i < bulletNumber; i++)
        {
            Vector3 newDirection = new Vector3(direction.x + NextFloat(-dispersion,dispersion), direction.y + NextFloat(-dispersion, dispersion), 0);
            float newAngle = (float)(Math.Atan2(newDirection.y, newDirection.x) * (180 / Math.PI));

            GameObject newBullet = Instantiate(bullet, gunEnd.transform.position, transform.rotation);
            BulletScript bulletScript = newBullet.GetComponent<BulletScript>();
            bulletScript.MoveDirection = newDirection;
            bulletScript.transform.eulerAngles = new Vector3(0, 0, newAngle);
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void FireRoundServerRPC(NetworkObjectReference bulletRef, NetworkObjectReference gunEndRef, Vector3 direction, float dispersion, int bulletNumber, ulong clientId)
    {
        for (int i = 0; i < bulletNumber; i++)
        {
            bulletRef.TryGet(out NetworkObject netBullet);
            GameObject bullet = netBullet.gameObject;
            gunEndRef.TryGet(out NetworkObject netGunEnd);
            GameObject gunEnd = netGunEnd.gameObject;
            Vector3 newDirection = new Vector3(direction.x + NextFloat(-dispersion, dispersion), direction.y + NextFloat(-dispersion, dispersion), 0);
            float newAngle = (float)(Math.Atan2(newDirection.y, newDirection.x) * (180 / Math.PI));
            GameObject newBullet = Instantiate(bullet, gunEnd.transform.position, transform.rotation);
            newBullet.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            BulletScript bulletScript = newBullet.GetComponent<BulletScript>();
            bulletScript.MoveDirection = newDirection;
            bulletScript.transform.eulerAngles = new Vector3(0, 0, newAngle);
        }
    }
}