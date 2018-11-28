using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIFDrawer : PropertyDrawer
{

    private bool CanShowProperty (SerializedProperty property)
    {
        ShowIfAttribute showIf = attribute as ShowIfAttribute;
        //if ((attribute as ProgressBarAttribute).hideWhenZero && property.floatValue <= 0)
        //    return;

        var enumField = property.serializedObject.FindProperty(showIf.EnumField);
        var enumValue = showIf.EnumValue;

        //bool ok = enumField.enumValueIndex & enumValue;
        //object enumFieldValue = enumField.objectReferenceValue;

        int enumFieldInt = enumField.intValue;
        int enumValueInt = (int)enumValue;

        // Only draw if it matches
        bool ok = (enumFieldInt & enumValueInt) != 0;

        /*
        Debug.Log("");
        Debug.Log("Field  : [" + enumField.name + "]");
        Debug.Log("Value  : [" + enumValue + "]");
        Debug.Log("Field a: [" + enumField.intValue + "]");
        Debug.Log("Value a: [" + (int) enumValue + "]");
        Debug.Log("Ok?    : [" + ok + "]");
        */

        return ok;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        /*
        if (property.propertyType != SerializedPropertyType.Float)
        {
            GUI.Label(position, "ERROR: can only apply progress bar onto a float");
            return;
        }
        */
        
        if (CanShowProperty(property))
            EditorGUI.PropertyField(position, property);

            //var enumField = showIf.enumField;

        //var dynamicLabel = property.serializedObject.FindProperty((attribute as ProgressBarAttribute).label);

        //EditorGUI.ProgressBar(position, property.floatValue/1f, dynamicLabel == null ? property.name : dynamicLabel.stringValue);
    }

    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        //if ((attribute as ProgressBarAttribute).hideWhenZero && property.floatValue <= 0)
        //    return 0;

        // If the property can't be shown,
        // it takes no space
        if (!CanShowProperty(property))
            return 0;


        return base.GetPropertyHeight(property, label);
    }
}
