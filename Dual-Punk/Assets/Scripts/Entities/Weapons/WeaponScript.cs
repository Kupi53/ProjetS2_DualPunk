using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using Unity.Netcode;
using UnityEngine.Playables;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;


public class WeaponScript : NetworkBehaviour
{
    public GameObject bullet;
    public GameObject gunEnd;
    public PlayerState? playerState;
    public PointerScript? pointerScript;
    public SpriteRenderer spriteRenderer;
    public int damage;
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
    public bool inHand;
    public Vector3 weaponOffset;

    [SerializeField] private int reloadAmount;
    [SerializeField] private bool autoReload;
    private float fireTimer;


    public void Start()
    {
        inHand = false;
        reloadTimer = 0;
        magSize = maxMagSize;
        fireTimer = fireRate;
        weaponOffset = new Vector3(0, 0.5f, 0);
    }


    public void Update()
    {
        //Faire des animations ici
        if (inHand)
        {
            pointerScript = playerState.Pointer.GetComponent<PointerScript>();
            pointerScript.spriteNumber = 1;
        }
        else
            pointerScript = null;
    }


    public void Run(Vector3 position, Vector3 direction)
    {
        Debug.Log("0");
        movePosition(position, weaponOffset, direction, weaponDistance);
        if ((Input.GetButton("Use") && auto && !reloading || Input.GetButtonDown("Use")) && fireTimer > fireRate && magSize > 0)
        {
            if (reloading)
                ResetReload();

            if (IsHost)
                Fire(direction, dispersion);

            fireTimer = 0;
            magSize--;
        }
        else
            fireTimer += Time.deltaTime;


        if (Input.GetButtonDown("Reload") && magSize != maxMagSize || autoReload && magSize == 0)
            reloading = true;

        if (reloading)
        {
            pointerScript.spriteNumber = 0;

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

    public void movePosition(Vector3 position, Vector3 weaponOffset, Vector3 direction, float weaponDistance){
        Debug.Log("2");
        float angle = (float)(Math.Atan2(direction.y, direction.x) * (180 / Math.PI));
        if (angle > 90 || angle < -90)
            spriteRenderer.flipY = true;
        else
            spriteRenderer.flipY = false;
        transform.position = position + weaponOffset + direction * weaponDistance;
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    public void ResetReload()
    {
        reloadTimer = 0;
        reloading = false;
    }


    public static float NextFloat(float min, float max)
    {
        System.Random random = new System.Random();
        double val = random.NextDouble() * (max - min) + min;
        return (float)val;
    }


    public virtual void Fire(Vector3 direction, float spread)
    {
        if (playerState.Walking)
            spread /= aimAccuracy;

        for (int i = 0; i < bulletNumber; i++)
        {
            Vector3 newDirection = new Vector3(direction.x + NextFloat(-spread, spread), direction.y + NextFloat(-spread, spread), 0).normalized;
            float newAngle = (float)(Math.Atan2(newDirection.y, newDirection.x) * (180 / Math.PI));

            GameObject newBullet = Instantiate(bullet, gunEnd.transform.position, transform.rotation);
            BulletScript bulletScript = newBullet.GetComponent<BulletScript>();
            bulletScript.MoveDirection = newDirection;
            bulletScript.transform.eulerAngles = new Vector3(0, 0, newAngle);
            NetworkObject bulletNetwork = newBullet.GetComponent<NetworkObject>();
            bulletNetwork.Spawn();
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
}