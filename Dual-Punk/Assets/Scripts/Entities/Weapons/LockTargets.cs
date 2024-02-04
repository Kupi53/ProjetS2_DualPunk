using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockTargets : MonoBehaviour
{
    private SmartWeaponScript weapon;

    private void Start()
    {
        weapon = transform.parent.gameObject.GetComponent<SmartWeaponScript>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ennemy"))
        {
            weapon.targets.Add(collision.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ennemy"))
        {
            weapon.index = 0;
            weapon.targets.Remove(collision.gameObject);
        }
    }
}
