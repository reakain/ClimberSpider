using UnityEngine;
using System;

[AttributeUsage
    (
        AttributeTargets.Field,
        AllowMultiple = false,
        Inherited = true
    )
]
public class ShowIfAttribute : PropertyAttribute
{
    public string EnumField;
    public object EnumValue;

    public ShowIfAttribute (string enumField, object enumValue)
    {
        EnumField = enumField;
        EnumValue = enumValue;
    }
}