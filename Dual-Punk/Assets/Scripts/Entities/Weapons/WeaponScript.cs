using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using Unity.Netcode;


public class WeaponScript : NetworkBehaviour
{
    public GameObject bullet;
    public GameObject gunEnd;
    private GameObject? player;
    public SpriteRenderer spriteRenderer;
    private WeaponController? weaponController;

    public float fireRate;
    public float dispersion;
    public float weaponDistance;
    public float reloadTime;
    public int maxMagSize;
    public int bulletNumber;
    public bool isAuto;

    private bool onGround;
    private float fireTimer;
    private float reloadTimer;
    private int magSize;
    private bool isReloading;


    // Start is called before the first frame update
    void Start()
    {
        onGround = true;
        reloadTimer = 0;
        fireTimer = fireRate;
        magSize = maxMagSize;
    }


    private void Update()
    {
        if (!onGround)
        {
            onGround = weaponController.DropWeapon();


            Vector3 direction = (weaponController.pointer.transform.position - player.transform.position).normalized;
            float angle = (float)(Math.Atan2(direction.y, direction.x) * (180 / Math.PI));

            if (angle > 90 || angle < -90)
                spriteRenderer.flipY = true;
            else
                spriteRenderer.flipY = false;

            transform.position = player.transform.position + direction * weaponDistance;
            transform.eulerAngles = new Vector3(0, 0, angle);


            if ((isAuto && Input.GetButton("Use") || !isAuto && Input.GetButtonDown("Use")) && fireTimer > fireRate && magSize > 0)
            {
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
  

            if (Input.GetButtonDown("Reload"))
            {
                isReloading = true;
            }
            if (isReloading)
            {
                if (reloadTimer > reloadTime)
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
            weaponController = player.GetComponent<WeaponController>();

            if (!weaponController.holdingWeapon)
            {
                weaponController.IsHolding(true);
                onGround = false;
            }
        }
    }
}