using System;
using UnityEngine;
using UnityEditor;
using Glance.Strategies;
using Object = UnityEngine.Object;

namespace Glance.Core
{
    public static class PreviewStrategyFactory
    {
        public static IGlancePreviewStrategy GetStrategy(Object target, bool isInspectorContext = false)
        {
            if (target == null) return null;

            string path = AssetDatabase.GetAssetPath(target);
            
            if (AssetDatabase.IsValidFolder(path))
            {
                return new FolderPreviewStrategy();
            }
            
            if (target is MonoScript || target is TextAsset)
            {
                return new ScriptTextStrategy();
            }

            if (isInspectorContext && (target is ScriptableObject || PrefabUtility.IsPartOfPrefabAsset(target)))
            {
                return new ObjectInspectorStrategy();
            }

            if (target is GameObject || target is Mesh || target is Material || target is Texture)
            {
                return new InteractivePreviewStrategy();
            }

            // Default fallback
            return new ObjectInspectorStrategy();
        }
    }
}
