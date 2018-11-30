using System;

// Restrict to methods only
[AttributeUsage(AttributeTargets.Method)]
public class ExposeInEditorAttribute : Attribute
{
    public bool RuntimeOnly = true;
}