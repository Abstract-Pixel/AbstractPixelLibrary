using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.IO;
using Glance.Core;

namespace Glance.Strategies
{
    public class FolderPreviewStrategy : IGlancePreviewStrategy
    {
        private Object _targetFolder;
        private ScrollView _scrollView;

        public void OnInitialize(Object target)
        {
            _targetFolder = target;
        }

        public void OnDrawUI(VisualElement root)
        {
            string path = AssetDatabase.GetAssetPath(_targetFolder);
            
            Label title = new Label($"Folder: {path}");
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.fontSize = 14;
            title.style.marginBottom = 10;
            root.Add(title);

            _scrollView = new ScrollView();
            root.Add(_scrollView);

            string[] subFolders = AssetDatabase.GetSubFolders(path);
            foreach (string folder in subFolders)
            {
                AddEntry(folder, true);
            }

            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                if (file.EndsWith(".meta")) continue;
                AddEntry(file, false);
            }
        }

        private void AddEntry(string path, bool isFolder)
        {
            path = path.Replace("\\", "/");
            
            VisualElement row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.paddingBottom = 2;
            row.style.paddingTop = 2;

            Texture icon = AssetDatabase.GetCachedIcon(path);
            if (icon != null)
            {
                Image img = new Image { image = icon };
                img.style.width = 16;
                img.style.height = 16;
                img.style.marginRight = 5;
                row.Add(img);
            }

            Label label = new Label(Path.GetFileName(path));
            row.Add(label);

            if (isFolder)
            {
                row.RegisterCallback<MouseDownEvent>(evt =>
                {
                    if (evt.button == 0 && evt.altKey)
                    {
                        evt.StopPropagation();
                        Object nextFolder = AssetDatabase.LoadAssetAtPath<Object>(path);
                        GlanceWindow.ShowWindow(nextFolder, false);
                    }
                });
            }

            _scrollView.Add(row);
        }

        public void OnCleanup()
        {
            // No cached editors to destroy
        }
    }
}
