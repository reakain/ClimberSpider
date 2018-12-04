using System;
using UnityEngine;
using System.Collections;

[Serializable]
public struct Interpolator
{
    // Input values (time)
    public float minFrom;
    public float maxFrom;

    // Output value (alpha, position, etc...)
    public float minTo;
    public float maxTo;

    public bool clamp;

    public Interpolator (float minFrom, float maxFrom, float minTo, float maxTo, bool clamp = true)
    {
        this.minFrom = minFrom;
        this.maxFrom = maxFrom;
        this.minTo = minTo;
        this.maxTo = maxTo;

        this.clamp = clamp;    
    }


    // Get the value at a specific time
    public float GetValue (float time)
    {
        float value = Lerp(minFrom, maxFrom, minTo, maxTo, time);
        if (clamp)
            return Mathf.Clamp(value, Mathf.Min(minTo,maxTo), Mathf.Max(minTo,maxTo)  );
        return value;
    }



    public float Lerp (float minFrom, float maxFrom, float minTo, float maxTo, float valueFrom)
    {
        //return (valueFrom - minFrom) / (maxFrom - minFrom) * (maxTo - minTo) + minTo;

        // Avoid divison by zero
        if (maxFrom == minFrom)
            return (maxTo + minTo) / 2f; // The average between the final values

        return (valueFrom - minFrom) / (maxFrom - minFrom) * (maxTo - minTo) + minTo;
    }



    // Use it as a interpolator[]
    public float this[float value]
    {
        get
        {
            return GetValue(value);
        }
    }
}