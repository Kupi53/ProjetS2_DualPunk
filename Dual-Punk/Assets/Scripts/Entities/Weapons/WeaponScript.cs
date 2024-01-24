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
    private GameObject? player;
    public SpriteRenderer spriteRenderer;
    private WeaponController? weaponController;
    private PlayerState? playerState;

    public float fireRate;
    public float dispersion;
    public float reloadTime;
    public float aimAccuracy;
    public float weaponDistance;
    public int maxMagSize;
    public int bulletNumber;
    public bool isAuto;

    private bool onGround;
    private float fireTimer;
    private float reloadTimer;
    private int magSize;

    private Vector3 weaponOffset;
    private bool isReloading;


    void Start()
    {
        weaponOffset = new Vector3(0,0.5f,0);
        onGround = true;
        reloadTimer = 0;
        fireTimer = fireRate;
        magSize = maxMagSize;
    }


    private void Update()
    {
        if (!onGround)
        {
            Vector3 direction = (weaponController.pointer.transform.position - player.transform.position-weaponOffset).normalized;
            float angle = (float)(Math.Atan2(direction.y, direction.x) * (180 / Math.PI));

            if (angle > 90 || angle < -90)
                spriteRenderer.flipY = true;
            else
                spriteRenderer.flipY = false;

            transform.position = player.transform.position + weaponOffset + direction  * weaponDistance;
            transform.eulerAngles = new Vector3(0, 0, angle);


            if ((isAuto && Input.GetButton("Use") || !isAuto && Input.GetButtonDown("Use")) && fireTimer > fireRate && magSize > 0)
            {
                if (playerState.Aiming)
                    dispersion /= aimAccuracy;

                if (IsHost)
                {
                    weaponController.FireRound(bullet, gunEnd, direction, dispersion, bulletNumber);
                }
                else
                {
                    weaponController.FireRoundServerRPC(bullet, gunEnd, direction, dispersion, bulletNumber, NetworkManager.Singleton.LocalClientId);
                }

                fireTimer = 0;
                magSize--;
            }
            else
                fireTimer += Time.deltaTime;


            if (Input.GetButtonDown("Drop"))
                onGround = weaponController.HoldWeapon(false);

            if (Input.GetButtonDown("Reload"))
                isReloading = true;

            
            if (isReloading)
            {
                if (reloadTimer >= reloadTime)
                {
                    reloadTimer = 0;
                    isReloading = false;
                    magSize = maxMagSize;
                }
                else
                    reloadTimer += Time.deltaTime;
            }
        }
    }


    void OnTriggerStay2D(Collider2D collision)
    {
        if (onGround && Input.GetButton("Pickup"))
        {
            player = collision.gameObject;
            if (player.CompareTag("Player"))
            {
                weaponController = player.GetComponent<WeaponController>();
                playerState = player.GetComponent<PlayerState>();
                onGround = weaponController.HoldWeapon(true);
                playerState.Weapon = gameObject;
            }
        }
    }
}