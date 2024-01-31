using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;


public class KnifeScript : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public int attack;
    public bool attacking;
    public float weaponDistance;
    public float resetCooldown;
    public float resetCooldownTimer;
    public float attackSpeed;
    public float attackRange;
    public float attackDistance;
    public Vector3 weaponOffset;

    private float angle;
    private float rangeTop;
    private float rangeMiddle;
    private float rangeBottom;
    private float currentWeaponDistance;


    void Start()
    {
        attacking = false;
        weaponOffset = new Vector3(0, 0.5f, 0);
        currentWeaponDistance = weaponDistance;
    }


    public void Run(Vector3 position, Vector3 direction)
    {
        if (Input.GetButtonDown("Use") && !attacking && attack < 3)
        {
            angle = (float)(Math.Atan2(direction.y, direction.x) * (180 / Math.PI));

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
            angle = (float)(Math.Atan2(direction.y, direction.x) * (180 / Math.PI));

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

        transform.position = position + weaponOffset + direction * currentWeaponDistance;
        transform.eulerAngles = new Vector3(0, 0, angle);
    }
}
