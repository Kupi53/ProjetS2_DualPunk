using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class MetalBladeScript : MeleeWeaponScript
{
    public override void MovePosition(Vector3 position, Vector3 direction, Vector3 targetPoint)
    {
        // si il n'y a pas d'animation en cours

        if (Math.Sign(targetPoint.x - position.x) != Math.Sign(transform.localScale.y))
        {
            transform.localScale = new Vector2(transform.localScale.x, -transform.localScale.y);
            WeaponOffset = new Vector3(-WeaponOffset.x, _weaponOffset.y, 0);
        }

        transform.position = position + WeaponOffset + direction * _weaponDistance;
        transform.eulerAngles = new Vector3(0, 0, Methods.GetAngle(direction));
    }

    protected override void ResetDefence()
    {
        throw new NotImplementedException();
    }
}
