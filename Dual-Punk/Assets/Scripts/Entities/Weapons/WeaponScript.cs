using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using Unity.Netcode;
using UnityEngine.Playables;


public class WeaponScript : NetworkBehaviour
{
    public GameObject bullet;
    public GameObject gunEnd;
    public SpriteRenderer spriteRenderer;

    public float fireRate;
    public float dispersion;
    public float reloadTime;
    public float reloadTimer;
    public float aimAccuracy;
    public float weaponDistance;
    public int magSize;
    public int maxMagSize;
    public int bulletNumber;
    public bool auto;
    public bool reloading;
    public Vector3 weaponOffset;

    [SerializeField] private int reloadAmount;
    [SerializeField] private bool autoReload;
    private float fireTimer;


    void Start()
    {
        reloadTimer = 0;
        fireTimer = fireRate;
        magSize = maxMagSize;
        weaponOffset = new Vector3(0, 0.5f, 0);
    }


    public void Run(Vector3 position, Vector3 direction, float angle, bool walking)
    {
        if (angle > 90 || angle < -90)
            spriteRenderer.flipY = true;
        else
            spriteRenderer.flipY = false;

        transform.position = position + weaponOffset + direction * weaponDistance;
        transform.eulerAngles = new Vector3(0, 0, angle);


        if ((auto && Input.GetButton("Use") || !auto && Input.GetButtonDown("Use")) && fireTimer > fireRate && magSize > 0)
        {
            if (reloading)
                ResetReload();

            if (IsHost)
                Fire(direction, dispersion, walking);

            fireTimer = 0;
            magSize--;
        }
        else
            fireTimer += Time.deltaTime;


        if ((Input.GetButtonDown("Reload") || autoReload) && magSize != maxMagSize)
            reloading = true;


        if (reloading)
        {
            if (reloadTimer >= reloadTime)
            {
                reloadTimer = 0;
                if (magSize + reloadAmount < maxMagSize)
                    magSize += reloadAmount;
                else
                { 
                    reloading = false;
                    magSize = maxMagSize;
                }
            }
            else
                reloadTimer += Time.deltaTime;
        }   
    }


    


    public static float NextFloat(float min, float max)
    {
        System.Random random = new System.Random();
        double val = random.NextDouble() * (max - min) + min;
        return (float)val;
    }


    public void Fire(Vector3 direction, float spread, bool walking)
    {
        if (walking)
            spread /= aimAccuracy;

        for (int i = 0; i < bulletNumber; i++)
        {
            Vector3 newDirection = new Vector3(direction.x + NextFloat(-spread, spread), direction.y + NextFloat(-spread, spread), 0);
            float newAngle = (float)(Math.Atan2(newDirection.y, newDirection.x) * (180 / Math.PI));

            GameObject newBullet = Instantiate(bullet, gunEnd.transform.position, transform.rotation);
            BulletScript bulletScript = newBullet.GetComponent<BulletScript>();
            bulletScript.MoveDirection = newDirection;
            bulletScript.transform.eulerAngles = new Vector3(0, 0, newAngle);
        }
    }



    /*public void FireRound(GameObject bullet, GameObject gunEnd, Vector3 direction, float dispersion, int bulletNumber, float aimAccuracy)
    {
        if (playerState.Walking)
            dispersion /= aimAccuracy;

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
    public void FireRoundServerRPC(NetworkObjectReference bulletRef, NetworkObjectReference gunEndRef, Vector3 direction, float dispersion, int bulletNumber, float aimAccuracy, ulong clientId)
    {
        if (playerState.Walking)
            dispersion /= aimAccuracy;

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
    }*/


    public void ResetReload()
    {
        reloadTimer = 0;
        reloading = false;
    }
}