using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutolockWeapon : WeaponScript
{
    private bool locked;

    void Start()
    {
        base.Start();
        locked = false;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        if (inHand)
        {
            if (pointerScript.target != null)
            {
                if (Input.GetButtonDown("Switch"))
                { locked = !locked;
                    Debug.Log("locked");
                }

                if (locked)
                    playerState.Pointer.transform.position = pointerScript.target.transform.position;
            }
            else
                locked = false;
        }
    }
}
