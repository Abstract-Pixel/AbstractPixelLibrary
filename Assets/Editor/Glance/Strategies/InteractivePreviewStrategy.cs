using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using Glance.Core;

namespace Glance.Strategies
{
    public class InteractivePreviewStrategy : IGlancePreviewStrategy
    {
        private Object _target;
        private Editor _cachedEditor;
        private IMGUIContainer _imguiContainer;

        public void OnInitialize(Object target)
        {
            _target = target;
            _cachedEditor = Editor.CreateEditor(_target);
        }

        public void OnDrawUI(VisualElement root)
        {
            if (_cachedEditor == null) return;

            _imguiContainer = new IMGUIContainer(() =>
            {
                if (_cachedEditor != null && _cachedEditor.HasPreviewGUI())
                {
                    Rect rect = GUILayoutUtility.GetRect(256, 256, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                    _cachedEditor.OnInteractivePreviewGUI(rect, EditorStyles.whiteLabel);
                }
                else
                {
                    GUILayout.Label("No interactive preview available.");
                }
            });

            _imguiContainer.style.flexGrow = 1;
            root.Add(_imguiContainer);
        }

        public void OnCleanup()
        {
            if (_cachedEditor != null)
            {
                Object.DestroyImmediate(_cachedEditor);
                _cachedEditor = null;
            }
        }
    }
}
