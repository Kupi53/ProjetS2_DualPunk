using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public static class Methods
{
    public static float GetAngle(Vector3 direction)
    {
        return (float)(Math.Atan2(direction.y, direction.x) * (180 / Math.PI));
    }


    public static float GetAbsoluteAngle(Vector3 direction)
    {
        return (float)Math.Atan2(Math.Abs(direction.y), Math.Abs(direction.x));
    }


    public static float GetDirectionFactor(Vector3 direction)
    {
        float angle = GetAbsoluteAngle(direction);
        double k = Math.Sqrt(Math.Pow(0.6 * Math.Cos(angle), 2) + Math.Pow(Math.Sin(angle), 2));
        float x = (float)(0.5 * Math.Cos(angle) / k);
        float y = (float)(0.5 * Math.Sin(angle) / k);

        return new Vector2(x, y).magnitude;
    }


    public static float ChangeFactorWithCos(float cosValue, float maxFactor, float minFactor)
    {
        cosValue = (cosValue + 1) / 2;
        return minFactor + cosValue * (maxFactor - minFactor);
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