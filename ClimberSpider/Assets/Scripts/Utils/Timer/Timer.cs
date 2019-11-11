using System;
using UnityEngine;

[Serializable]
public struct Timer {

    public float startTime;
    public float duration;

    public Func<float> GetTime;

    public void Start (bool restart = true)
    {
        GetTime = () => { return Time.time; };

        // If it is done
        // Or if it is not done, but it is set to restart anyway...
        if (
            IsDone()
            ||
            ( ! IsDone() && restart)    
            )
            startTime = GetTime();
    }

    public bool IsDone ()
    {
        return GetTime() >= startTime + duration;
    }

    // From zero to one, how complete is it
    public float GetNormalised ()
    {
        if (duration == 0)
            return 1f;
        return Mathf.Clamp01(   (GetTime() - startTime) / (duration)    );
    }
}
