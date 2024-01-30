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

    private GameObject? player;
    private WeaponController? weaponController;
    private PlayerState? playerState;

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

    [SerializeField] private int reloadAmount;
    [SerializeField] private bool autoReload;
    private bool onGround;
    private float fireTimer;
    private Vector3 weaponOffset;


    void Start()
    {
        onGround = true;
        reloadTimer = 0;
        fireTimer = fireRate;
        magSize = maxMagSize;
        weaponOffset = new Vector3(0, 0.5f, 0);
    }


    private void Update()
    {
        if (!onGround)
        {
            Vector3 direction = (weaponController.pointer.transform.position - player.transform.position - weaponOffset).normalized;
            float angle = (float)(Math.Atan2(direction.y, direction.x) * (180 / Math.PI));

            if (angle > 90 || angle < -90)
                spriteRenderer.flipY = true;
            else
                spriteRenderer.flipY = false;

            transform.position = player.transform.position + weaponOffset + direction * weaponDistance;
            transform.eulerAngles = new Vector3(0, 0, angle);


            if ((auto && Input.GetButton("Use") || !auto && Input.GetButtonDown("Use")) && fireTimer > fireRate && magSize > 0)
            {
                if (reloading)
                    ResetReload();

                if (IsHost)
                    weaponController.FireRound(bullet, gunEnd, direction, dispersion, bulletNumber, aimAccuracy);
                else
                    weaponController.FireRoundServerRPC(bullet, gunEnd, direction, dispersion, bulletNumber, aimAccuracy, NetworkManager.Singleton.LocalClientId);

                fireTimer = 0;
                magSize--;
            }
            else
                fireTimer += Time.deltaTime;


            if (Input.GetButtonDown("Drop"))
                onGround = weaponController.HoldWeapon(false);

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

        else if (reloading)
        {
            ResetReload();
        }
    }


    private void ResetReload()
    {
        reloadTimer = 0;
        reloading = false;
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
                if (!(onGround = weaponController.HoldWeapon(true)))
                    playerState.Weapon = gameObject;
            }
        }
    }
}