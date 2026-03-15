using UnityEngine;
using UnityEditor;
using Glance.Core;

namespace Glance.Hooks
{
    [InitializeOnLoad]
    public static class GlanceProjectHook
    {
        static GlanceProjectHook()
        {
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
        }

        private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            Event evt = Event.current;

            if (evt.type == EventType.MouseDown && evt.button == 0 && evt.alt)
            {
                if (selectionRect.Contains(evt.mousePosition))
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    Object target = AssetDatabase.LoadAssetAtPath<Object>(path);

                    if (target != null)
                    {
                        if (ProjectWindowUtil.IsFolder(target.GetInstanceID()) || AssetDatabase.IsValidFolder(path))
                        {
                            evt.Use(); // Suppress default folder enter
                        }
                        
                        GlanceWindow.ShowWindow(target, false);
                        evt.Use();
                    }
                }
            }
        }
    }
}
