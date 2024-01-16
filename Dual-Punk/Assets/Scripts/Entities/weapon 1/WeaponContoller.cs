using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System;
using UnityEditor.Experimental.GraphView;

public class WeaponContoller : MonoBehaviour
{
    public GameObject bullet;
    public GameObject pointer;
    public GameObject? player;
    private BulletScript bulletScript;
    private float timer;
    public float fireRate;
    public float weaponDistance;
    public bool onGround;
    


    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        onGround = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!onGround)
        {
            if (Input.GetButton("Drop"))
            {
                player.GetComponent<Player>().HoldingWeapon = false;
                onGround = true;
            }

            Vector3 direction = (pointer.transform.position - player.transform.position).normalized;
            float angle = (float)(Math.Atan2(direction.y, direction.x) * (180 / Math.PI));
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
        GameObject newBullet = Instantiate(bullet, new Vector3(transform.position.x, transform.position.y, 0), transform.rotation);
        bulletScript = newBullet.GetComponent<BulletScript>();
        bulletScript.MoveDirection = direction;
        bulletScript.transform.eulerAngles = new Vector3(0, 0, angle);
    }

    // lorsqu'on joueur collide avec le weapon
    void OnTriggerStay2D(Collider2D collisionInfo)
    {
        Debug.Log("Collision");
        // si sur le sol
        if (onGround && Input.GetButton("Pickup"))
        {
            player = collisionInfo.gameObject;
            player.GetComponent<Player>().HoldingWeapon = true;
            onGround = false;
        }
    }
}