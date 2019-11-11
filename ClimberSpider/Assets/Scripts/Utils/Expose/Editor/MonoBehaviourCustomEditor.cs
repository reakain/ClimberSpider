using UnityEngine;
using UnityEditor;
using System.Reflection;


[CanEditMultipleObjects] // Don't ruin everyone's day
[CustomEditor(typeof(MonoBehaviour), true)] // Target all MonoBehaviours and descendants
public class MonoBehaviourCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draw the normal inspector

        // Currently this will only work in the Play mode. You'll see why

        //if (Application.isPlaying)
        //{
        // Get the type descriptor for the MonoBehaviour we are drawing
        var type = target.GetType();

        // Iterate over each private or public instance method (no static methods atm)
        foreach (var method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
        {
            // make sure it is decorated by our custom attribute
            var attributes = method.GetCustomAttributes(typeof(ExposeInEditorAttribute), true);
            if (attributes.Length > 0)
            {
                bool GUIEnabled = GUI.enabled;
                ExposeInEditorAttribute attribute = attributes[0] as ExposeInEditorAttribute;
                GUI.enabled = ! (! Application.isPlaying && attribute.RuntimeOnly);

                if (GUILayout.Button("Run: " + method.Name))
                {
                    // If the user clicks the button, invoke the method immediately.
                    // There are many ways to do this but I chose to use Invoke which only works in Play Mode.
                    method.Invoke(target, null);
                    //((MonoBehaviour)target).Invoke(method.Name, 0f);
                }

                GUI.enabled = GUIEnabled;
            }
        }
    }
}