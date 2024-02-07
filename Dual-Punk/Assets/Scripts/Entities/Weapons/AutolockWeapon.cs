using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutolockWeapon : WeaponScript
{
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        if (inHand && pointerScript.target != null)
        {
            if (Input.GetButtonDown("Switch"))
            {
                pointerScript.locked = !pointerScript.locked;
            }
        }
    }
}
