using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using Unity.Netcode;


public class WeaponController : NetworkBehaviour
{
    [SerializeField] private GameObject pointer;
    private PlayerState playerState;
    private WeaponScript? weaponScript;
    private KnifeScript? knifeScript;

    private List<GameObject> weapons;
    private int index;

    private float angle;
    private Vector3 direction;


    private void Start()
    {
        index = 0;
        weapons = new List<GameObject>();
        playerState = gameObject.GetComponent<PlayerState>();
    }


    private void Update()
    {
        if (Input.GetButtonDown("Switch") && weapons.Count > 0)
        {
            index = (index + 1) % weapons.Count;
        }

        if (Input.GetButtonDown("Pickup") && weapons.Count > 0 && !playerState.HoldingWeapon && !playerState.HoldingKnife)
        {
            playerState.Weapon = weapons[index];
            if (weapons[index].CompareTag("Weapon"))
            {
                playerState.HoldingWeapon = true;
                weaponScript = weapons[index].GetComponent<WeaponScript>();
            }
            else
            {
                playerState.HoldingKnife = true;
                knifeScript = weapons[index].GetComponent<KnifeScript>();
            }
        }

        if (playerState.HoldingWeapon)
        {
            direction = (pointer.transform.position - transform.position - weaponScript.weaponOffset).normalized;
            angle = (float)(Math.Atan2(direction.y, direction.x) * (180 / Math.PI));

            if (Input.GetButtonDown("Drop"))
            { 
                playerState.HoldingWeapon = false;
                weaponScript.ResetReload();
            }

            weaponScript.Run(transform.position, direction, angle, playerState.Walking);
        }

        else if (playerState.HoldingKnife)
        {
            if (!knifeScript.attacking)
            {
                direction = (pointer.transform.position - transform.position - knifeScript.weaponOffset).normalized;
                angle = (float)(Math.Atan2(direction.y, direction.x) * (180 / Math.PI));

                if (Input.GetButtonDown("Drop"))
                    playerState.HoldingKnife = false;
            }

            knifeScript.Run(transform.position, direction, angle);
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


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weapon") || collision.gameObject.CompareTag("Knife"))
        {
            weapons.Add(collision.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weapon") || collision.gameObject.CompareTag("Knife"))
        {
            weapons.Remove(collision.gameObject);
        }
    }
}