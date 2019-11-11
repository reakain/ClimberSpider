using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Utils
{
    public static float Sin (float x, float min, float max, float period)
    {
        return (max - min) / 2f * (1f + Mathf.Sin(x * Mathf.PI * 2 / period)) + min;
    }
    public static float ManhattanDistance (Vector3 a, Vector3 b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }



    // http://math.stackexchange.com/questions/76457/check-if-a-point-is-within-an-ellipse
    // centre = centre of the ellipse
    // radius = (x,y) = (semi major axis x, semi major axis y)
    public static bool PointInsideEllipse (Vector2 point, Vector2 centre, Vector2 radius)
    {
       return NormalisedDistanceFromEllipseCentre(point, centre, radius) <= 1f;
    }

    // 0 when point is at the centre of ellipse
    // 1 when point is on the margin of the ellipse
    // <= 1 = point inside the ellipse
    // >  1 = point outside the ellipse
    public static float NormalisedDistanceFromEllipseCentre(Vector2 point, Vector2 centre, Vector2 radius)
    {
        return
           Mathf.Pow((point.x - centre.x) / radius.x, 2f) +
           Mathf.Pow((point.y - centre.y) / radius.y, 2f) ;
    }
    
}
