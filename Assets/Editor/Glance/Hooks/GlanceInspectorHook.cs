using UnityEngine;
using UnityEditor;
using System.Reflection;
using Glance.Core;

namespace Glance.Hooks
{
    [InitializeOnLoad]
    public static class GlanceInspectorHook
    {
        static GlanceInspectorHook()
        {
            FieldInfo info = typeof(EditorApplication).GetField("globalEventHandler", BindingFlags.Static | BindingFlags.NonPublic);
            if (info != null)
            {
                EditorApplication.CallbackFunction functions = (EditorApplication.CallbackFunction)info.GetValue(null);
                functions += OnGlobalEvent;
                info.SetValue(null, functions);
            }
        }

        private static void OnGlobalEvent()
        {
            Event evt = Event.current;
            if (evt == null) return;

            if (evt.type == EventType.MouseDown && evt.button == 0 && evt.alt)
            {
                EditorWindow window = EditorWindow.mouseOverWindow;
                if (window != null && window.GetType().Name == "InspectorWindow")
                {
                    // In a full implementation, we would raycast the VisualElements or use IMGUI reflection
                    // to find the exact SerializedProperty under the mouse.
                    // For this prototype, we check if there's an active object or dragged reference.
                    
                    // Example of intercepting the event:
                    // evt.Use();
                    // GlanceWindow.ShowWindow(target, true);
                }
            }
        }
    }
}
