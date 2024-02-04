using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using System;


public class SmartWeaponScript : WeaponScript
{
    [SerializeField] private GameObject AutoLockZone;
    [SerializeField] private GameObject LockedTargetPointer;
    private GameObject? lockedTarget;
    private Vector2 mousePos;

    public List<GameObject> targets;
    public int index;


    private void Start()
    {
        base.Start();
        index = 0;
        targets = new List<GameObject>();
    }


    private void Update()
    {
        base.Update();

        if (inHand && !reloading)
        {
            playerState.pointer = 2;
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            AutoLockZone.transform.position = mousePos;

            if (targets.Count > 0)
            {
                if (Input.GetButtonDown("Switch"))
                    index = (index + 1) % targets.Count;

                lockedTarget = targets[index];
                LockedTargetPointer.GetComponent<SpriteRenderer>().enabled = true;
                LockedTargetPointer.transform.position = lockedTarget.transform.position;
            }
            else
            {
                ResetTarget();
            }
        }
        else
        {
            ResetTarget();
        }
    }


    private void ResetTarget()
    {
        lockedTarget = null;
        LockedTargetPointer.GetComponent<SpriteRenderer>().enabled = false;
    }


    public override void Fire(Vector3 direction, float spread)
    {
        if (playerState.Walking)
            spread /= aimAccuracy;

        for (int i = 0; i < bulletNumber; i++)
        {
            Vector3 newDirection = new Vector3(direction.x + NextFloat(-spread, spread), direction.y + NextFloat(-spread, spread), 0).normalized;
            float newAngle = (float)(Math.Atan2(newDirection.y, newDirection.x) * (180 / Math.PI));

            GameObject newBullet = Instantiate(bullet, gunEnd.transform.position, transform.rotation);
            SeekingBulletScript bulletScript = newBullet.GetComponent<SeekingBulletScript>();
            bulletScript.MoveDirection = newDirection;
            bulletScript.transform.eulerAngles = new Vector3(0, 0, newAngle);
            bulletScript.target = lockedTarget;
        }
    }
}