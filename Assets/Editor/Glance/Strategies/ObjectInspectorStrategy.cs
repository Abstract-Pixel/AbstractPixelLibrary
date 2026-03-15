using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using Glance.Core;

namespace Glance.Strategies
{
    public class ObjectInspectorStrategy : IGlancePreviewStrategy
    {
        private Object _target;
        private Editor _cachedEditor;
        private ScrollView _scrollView;

        public void OnInitialize(Object target)
        {
            _target = target;
            _cachedEditor = Editor.CreateEditor(_target);
        }

        public void OnDrawUI(VisualElement root)
        {
            if (_cachedEditor == null) return;

            _scrollView = new ScrollView();
            _scrollView.style.flexGrow = 1;
            _scrollView.style.paddingTop = 10;
            _scrollView.style.paddingLeft = 10;
            _scrollView.style.paddingRight = 10;
            _scrollView.style.paddingBottom = 10;

            IMGUIContainer imguiContainer = new IMGUIContainer(() =>
            {
                if (_cachedEditor != null)
                {
                    _cachedEditor.OnInspectorGUI();
                }
            });

            _scrollView.Add(imguiContainer);
            root.Add(_scrollView);
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
