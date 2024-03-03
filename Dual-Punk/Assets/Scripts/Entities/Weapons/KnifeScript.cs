using System;
using UnityEngine;


public class KnifeScript : WeaponScript
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    public int attack;
    public bool attacking;
    public float weaponDistance;
    public float resetCooldown;
    public float resetCooldownTimer;
    public float attackSpeed;
    public float attackRange;
    public float attackDistance;

    private float angle;
    private float rangeTop;
    private float rangeMiddle;
    private float rangeBottom;
    private float currentWeaponDistance;


    void Start()
    {
        WeaponOffset = new Vector3(0, 0.5f, 0);
        currentWeaponDistance = weaponDistance;
    }


    public void Update()
    {
        //Faire des animations ici
        if (InHand)
        {
            PointerScript = PlayerState.Pointer.GetComponent<PointerScript>();
            PointerScript.spriteNumber = 1;
        }
        else
            PointerScript = null;
    }


    public override void Run(Vector3 position, Vector3 direction)
    {
        if (Input.GetButtonDown("Use") && !PlayerState.Attacking && attack < 3)
        {
            angle = (float)(Math.Atan2(direction.y, direction.x) * (180 / Math.PI));

            attack += 1;
            resetCooldownTimer = 0;
            PlayerState.Attacking = true;

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

        if (PlayerAttacking)
        {
            switch (attack)
            {
                case 1:
                    angle -= attackSpeed * Time.deltaTime;
                    if (angle <= rangeBottom)
                        Attacking = false;
                    break;

                case 2:
                    angle += attackSpeed * Time.deltaTime;
                    if (angle >= rangeTop)
                        Attacking = false;
                    break;

                case 3:
                    if (angle < rangeMiddle)
                        angle -= attackSpeed * Time.deltaTime;
                    else if (currentWeaponDistance < attackDistance)
                        currentWeaponDistance += Time.deltaTime * 10;
                    else
                    {
                        Attacking = false;
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
                Reset();
            }

            direction = new Vector3((float)Math.Cos(angle * Math.PI / 180), (float)Math.Sin(angle * Math.PI / 180)).normalized;
        }

        transform.position = position + WeaponOffset + direction * currentWeaponDistance;
        transform.eulerAngles = new Vector3(0, 0, angle);
    }


    public override void Reset()
    {
        attack = 0;
        resetCooldownTimer = 0;
        currentWeaponDistance = weaponDistance;
    }
}