using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;

public class M4Script : MonoBehaviour
{
    public GameObject bullet;
    public GameObject gunEnd;
    public GameObject pointer;
    private GameObject? player;
    public SpriteRenderer spriteRenderer;
    private BulletScript bulletScript;
    private bool inHand;
    private float timer;
    public float fireRate;
    public float weaponDistance;


    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        inHand = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (inHand)
        {
            if (Input.GetButton("Drop"))
            {
                player.GetComponent<Player>().HoldingWeapon = false;
                inHand = false;
            }

            Vector3 direction = (pointer.transform.position - player.transform.position).normalized;
            float angle = (float)(Math.Atan2(direction.y, direction.x) * (180 / Math.PI));
            if (angle > 90 || angle < -90)
                spriteRenderer.flipY = true;
            else
                spriteRenderer.flipY = false;

            transform.position = player.transform.position + direction * weaponDistance;
            transform.eulerAngles = new Vector3(0, 0, angle);

            if (timer < fireRate)
            {
                timer += Time.deltaTime;
            }
            else if (Input.GetButton("Fire1"))
            {
                timer = 0;
                fireRound(direction, angle);
            }
        }
    }

    void fireRound(Vector3 direction, float angle)
    {
        GameObject newBullet = Instantiate(bullet, gunEnd.transform.position, transform.rotation);
        bulletScript = newBullet.GetComponent<BulletScript>();
        bulletScript.MoveDirection = direction;
        bulletScript.transform.eulerAngles = new Vector3(0, 0, angle);
    }

    void OnTriggerStay2D(Collider2D collisionInfo)
    {
        // si sur le sol
        if (!inHand && Input.GetButton("Pickup"))
        {
            player = collisionInfo.gameObject;
            player.GetComponent<Player>().HoldingWeapon = true;
            inHand = true;
        }
    }
}