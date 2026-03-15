using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.IO;
using Glance.Core;

namespace Glance.Strategies
{
    public class ScriptTextStrategy : IGlancePreviewStrategy
    {
        private Object _targetScript;

        public void OnInitialize(Object target)
        {
            _targetScript = target;
        }

        public void OnDrawUI(VisualElement root)
        {
            string path = AssetDatabase.GetAssetPath(_targetScript);
            string text = File.ReadAllText(path);

            ScrollView scrollView = new ScrollView();
            scrollView.style.flexGrow = 1;
            scrollView.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
            scrollView.style.paddingTop = 5;
            scrollView.style.paddingBottom = 5;
            scrollView.style.paddingLeft = 5;
            scrollView.style.paddingRight = 5;

            Label textLabel = new Label(text);
            textLabel.style.color = new Color(0.8f, 0.8f, 0.8f);
            textLabel.style.unityFontDefinition = new StyleFontDefinition(Font.CreateDynamicFontFromOSFont("Consolas", 12));
            textLabel.style.whiteSpace = WhiteSpace.Pre;

            scrollView.Add(textLabel);
            root.Add(scrollView);
        }

        public void OnCleanup()
        {
        }
    }
}
