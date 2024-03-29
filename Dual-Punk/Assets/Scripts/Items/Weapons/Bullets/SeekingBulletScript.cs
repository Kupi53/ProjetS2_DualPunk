using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;


public class SeekingBulletScript : BulletScript
{
    private float _rotateSpeed;

#nullable enable
    public GameObject? Target { get; set; }
#nullable disable


    private new void FixedUpdate()
    {
        base.FixedUpdate();

        if (Target != null)
        {
            Vector3 heading = (Target.transform.position - transform.position).normalized;
            float angle = Vector3.Cross(_moveDirection, heading).z * 100;

            if (angle > _rotateSpeed * _moveFactor)
                angle = _rotateSpeed;
            else if (-angle > _rotateSpeed * _moveFactor)
                angle = -_rotateSpeed;

            ChangeDirection(Quaternion.Euler(0, 0, angle) * _moveDirection, false);
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + angle);
        }
    }

    
    public void Setup(int damage, float moveSpeed, Vector3 moveDirection, float rotateSpeed)
    {
        Setup(damage, moveSpeed, moveDirection);
        _rotateSpeed = rotateSpeed;
    }
}




/*
Vector3 heading = (Target.transform.position - transform.position).normalized;
float rotation = Vector3.Cross(MoveDirection, heading).z;

MoveDirection = Quaternion.Euler(0, 0, rotation) * MoveDirection;
transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + rotation);
*/



/*
float angle = (float)(Math.Atan2(MoveDirection.y, MoveDirection.x) * (180 / Math.PI));

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