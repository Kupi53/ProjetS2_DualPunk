using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public static class Methods
{
    public static float GetAngle(Vector3 direction)
    {
        return (float)(Math.Atan2(direction.y, direction.x) * (180 / Math.PI)); ;
    }


    public static float GetDirectionFactor(Vector3 direction)
    {
        direction = direction.normalized;
        return (float)Math.Sqrt(direction.x * direction.x + direction.y*direction.y/4);
    }


    public static bool SameDirection(float angle1, float angle2, int margin)
    {
        if (angle1 + margin > 180 && (angle2 > angle1 - margin || angle2 < -360 + angle1 + margin))
            return true;
        else if (angle1 - margin < -180 && (angle2 < angle1 + margin || angle2 > 360 - angle1 - margin))
            return true;
        else if (angle2 < angle1 + margin && angle2 > angle1 - margin)
            return true;
        return false;
    }


    public static float NextFloat(float min, float max)
    {
        System.Random random = new System.Random();
        double val = random.NextDouble() * (max - min) + min;
        return (float)val;
    }
}