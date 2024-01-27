using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class KnifeScript : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    private GameObject? player;
    private WeaponController? weaponController;
    private PlayerState? playerState;

    public bool attacking;
    public float weaponDistance;
    public float resetCooldown;
    public float resetCooldownTimer;
    public float attackSpeed;
    public float attackRange;
    public float attackDistance;

    private bool onGround;
    private int attack;
    private float angle;
    private float rangeTop;
    private float rangeMiddle;
    private float rangeBottom;
    private float currentWeaponDistance;
    private Vector3 direction;
    private Vector3 weaponOffset;


    void Start()
    {
        onGround = true;
        weaponOffset = new Vector3(0, 0.5f, 0);
        currentWeaponDistance = weaponDistance;
    }


    private void Update()
    {
        if (!onGround)
        {
            if (!attacking)
            {
                direction = (weaponController.pointer.transform.position - player.transform.position - weaponOffset).normalized;
                angle = (float)(Math.Atan2(direction.y, direction.x) * (180 / Math.PI));


                if (Input.GetButtonDown("Use") && !attacking && attack < 3)
                {
                    attack += 1;
                    attacking = true;
                    resetCooldownTimer = 0;

                    rangeMiddle = angle;
                    rangeTop = angle + attackRange;
                    rangeBottom = angle - attackRange;

                    if (attack == 2)
                    {
                        angle = rangeBottom;
                        spriteRenderer.flipY = true;
                    }
                    else
                    {
                        angle = rangeTop;
                        spriteRenderer.flipY = false;
                    }
                }

                if (Input.GetButtonDown("Drop"))
                    onGround = weaponController.HoldWeapon(false);
            }


            if (attacking)
            {
                switch (attack)
                {
                    case 1:
                        angle -= attackSpeed * Time.deltaTime;
                        if (angle <= rangeBottom)
                            attacking = false;
                        break;

                    case 2:
                        angle += attackSpeed * Time.deltaTime;
                        if (angle >= rangeTop)
                            attacking = false;
                        break;

                    case 3:
                        if (angle < rangeMiddle)
                            angle -= attackSpeed * Time.deltaTime;
                        else if (currentWeaponDistance < attackDistance)
                            currentWeaponDistance += Time.deltaTime * 10;
                        else
                        {
                            attacking = false;
                            currentWeaponDistance = attackDistance;
                        }
                        break;
                }
            }

            else
            {
                switch (attack)
                {
                    case 1:
                        angle -= attackRange;
                        break;
                    case 2:
                        angle += attackRange;
                        break;
                    case 0:
                        if (angle > 90 || angle < -90)
                            spriteRenderer.flipY = true;
                        else
                            spriteRenderer.flipY = false;
                        break;
                }
            }


            if (attack != 0)
            {
                resetCooldownTimer += Time.deltaTime;
                if (resetCooldownTimer > resetCooldown)
                {
                    attack = 0;
                    resetCooldownTimer = 0;
                    currentWeaponDistance = weaponDistance;
                }

                direction = new Vector2((float)Math.Cos(angle * Math.PI / 180), (float)Math.Sin(angle * Math.PI / 180)).normalized;
            }

            transform.position = player.transform.position + weaponOffset + direction * currentWeaponDistance;
            transform.eulerAngles = new Vector3(0, 0, angle);
        }
    }


    private void MoveAt()
    {

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
