using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;


public class SeekingBulletScript : BulletScript
{
    public float RotateSpeed { get; set; }

    #nullable enable
    public GameObject? Target { get; set; }
    #nullable disable


    new void FixedUpdate()
    {
        base.FixedUpdate();

        if (Target != null)
        {
            Vector3 heading = (Target.transform.position - transform.position).normalized;
            float rotation = Vector3.Cross(MoveDirection, heading).z * RotateSpeed;

            MoveDirection = Quaternion.Euler(0, 0, rotation) * MoveDirection;
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + rotation);
        }
    }
}





/*float angle = (float)(Math.Atan2(MoveDirection.y, MoveDirection.x) * (180 / Math.PI));

MoveDirection = new Vector3((float)Math.Cos((angle + rotation) * Math.PI / 180), (float)Math.Sin((angle + rotation) * Math.PI / 180)).normalized;
transform.eulerAngles = new Vector3(0, 0, angle + rotation);



    private float GetAngle(Vector3 direction)
    {
        return (float)(Math.Atan2(direction.y, direction.x) * (180 / Math.PI));
    }


    private bool AngleTwoMoreAtRight(float angle1, float angle2)
    {
        if (angle1 > 0 && angle2 < 0)
        {
            return angle1 - angle2 < 180;
        }
        else if (angle1 < 0 && angle2 > 0)
        {
            return -angle1 + angle2 > 180;
        }
        return angle1 > angle2;
    }
*/