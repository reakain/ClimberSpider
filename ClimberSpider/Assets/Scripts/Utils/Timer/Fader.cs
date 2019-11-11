using System;
using UnityEngine;
using System.Collections;

public struct Fader
{
    // Input values (time)
    private float startTime;
    private float finalTime;

    // Output value (alpha, position, etc...)
    private float startValue;
    private float finalValue;

    //public bool clamp = true;
    public bool clamp;

    // When done, it stops updating
    //private bool done = true;
    private bool done;

    // To get the time
    // Standard behaviour: Unity time
    public Func<float> GetTime;
    //public Func<float> GetTime = delegate () { return Time.time; };
    // Tween: Hermite, ...
    //  Input:  [0,1]
    //  Output: [0,1]
    public Func<float, float> Fx;
    //public Func<float, float> Fx = delegate (float value) { return value;    };

    // An action to perform while the fader is fading
    //public Action<float> OnChange = delegate (float t) { };

    public Fader (float initialValue)
    {
        clamp = true;
        done = true;
        GetTime = delegate () { return Time.time; };
        Fx = delegate (float value) { return value; };

        startTime = finalTime = GetTime();
        startValue = finalValue = initialValue;
    }



    // Get the value at the current time
    public float GetValue ()
    {
        return GetValue (   GetTime()   );
    }

    // Get the value at a specific time
    public float GetValue (float time)
    {
        float value = Lerp(startTime, finalTime, startValue, finalValue, time);
        if (clamp)
            return Mathf.Clamp(value, Mathf.Min(startValue,finalValue), Mathf.Max(startValue,finalValue)  );
        return value;
    }



    public float Lerp (float minFrom, float maxFrom, float minTo, float maxTo, float valueFrom)
    {
        //return (valueFrom - minFrom) / (maxFrom - minFrom) * (maxTo - minTo) + minTo;

        // Avoid divison by zero
        if (maxFrom == minFrom)
            return (maxTo + minTo) / 2f; // The average between the final values

        return Fx(  (valueFrom - minFrom) / (maxFrom - minFrom) ) * (maxTo - minTo) + minTo;
    }


    public void FadeTo (float valueTo, float duration)
    {
        // Current value, final value
        startValue = GetValue();
        finalValue = valueTo;

        // Start time, end time
        startTime = GetTime();
        finalTime = startTime + duration;

        done = false;
    }

    /* While the fader is fading,
     * it keeps executing its update function.
     */
     /*
    public void Update ()
    {
        // Nothing to do
        if (done)
            return;

        // Calls the OnChange
        float value = GetValue();
        OnChange(value);

        // Ends
        if (GetTime() > finalTime)
            done = true;
    }
    */
    public bool IsDone ()
    {
        bool isDone = done;

        // Ends
        if (GetTime() > finalTime)
            done = true;

        return isDone;
    }

    // Use it as a float
    public static implicit operator float(Fader fader)
    {
        return fader.GetValue();
    }







    // Starts a coroutine
    // That will update this object in the background
    public void FadeToCoroutine (float valueTo, float duration, MonoBehaviour mono, Action<float> OnChange)
    {
        FadeTo(valueTo, duration);
        mono.StartCoroutine(UpdateCoroutine(OnChange));
    }

    // An action to perform while the fader is fading
    private IEnumerator UpdateCoroutine (Action<float> OnChange)
    {
        while (! IsDone() )
        {
            OnChange(GetValue());
            yield return new WaitForEndOfFrame();
        }
    }
}