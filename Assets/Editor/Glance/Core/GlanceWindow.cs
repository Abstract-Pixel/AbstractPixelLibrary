using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace Glance.Core
{
    public class GlanceWindow : EditorWindow
    {
        private static GlanceWindow _instance;
        private IGlancePreviewStrategy _currentStrategy;
        private Object _currentTarget;

        public static void ShowWindow(Object target, bool isInspectorContext = false)
        {
            if (_instance == null)
            {
                _instance = GetWindow<GlanceWindow>("Glance Preview");
            }
            
            _instance.ShowUtility();
            _instance.Focus();
            _instance.LoadTarget(target, isInspectorContext);
        }

        private void LoadTarget(Object target, bool isInspectorContext)
        {
            if (_currentStrategy != null)
            {
                _currentStrategy.OnCleanup();
            }

            _currentTarget = target;
            _currentStrategy = PreviewStrategyFactory.GetStrategy(target, isInspectorContext);

            rootVisualElement.Clear();

            if (_currentStrategy != null)
            {
                _currentStrategy.OnInitialize(target);
                _currentStrategy.OnDrawUI(rootVisualElement);
            }
            else
            {
                rootVisualElement.Add(new Label("No preview available for this object."));
            }
        }

        private void OnEnable()
        {
            rootVisualElement.RegisterCallback<KeyDownEvent>(OnKeyDown);
        }

        private void OnDisable()
        {
            rootVisualElement.UnregisterCallback<KeyDownEvent>(OnKeyDown);
            if (_currentStrategy != null)
            {
                _currentStrategy.OnCleanup();
                _currentStrategy = null;
            }
        }

        private void OnDestroy()
        {
            if (_currentStrategy != null)
            {
                _currentStrategy.OnCleanup();
                _currentStrategy = null;
            }
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Escape)
            {
                Close();
            }
        }
    }
}
