using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutolockWeapon : WeaponScript
{
    void Start()
    {
        base.Start();
    }

    void Update()
    {
        base.Update();

        if (inHand)
        {
            if (pointerScript.target != null && Input.GetButtonDown("Switch"))
            {
                pointerScript.locked = !pointerScript.locked;
            }
        }
    }
}
