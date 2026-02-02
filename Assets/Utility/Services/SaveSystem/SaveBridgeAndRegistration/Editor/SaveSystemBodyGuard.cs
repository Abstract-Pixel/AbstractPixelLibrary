using UnityEngine;
using UnityEditor.Callbacks;
using UnityEditor;
using System;


using System.Reflection;
using System.Linq;

namespace AbstractPixel.Utility.Save
{
    [InitializeOnLoad]
    public static class SaveSystemBodyGuard
    {

        static SaveSystemBodyGuard()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }


        [DidReloadScripts]
        public static void OnScriptsReloaded()
        {
            ValidateCodebase(out bool _errorFound);

        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingEditMode) return;

            ValidateCodebase(out bool _errorFound);

            if (_errorFound)
            {
                EditorApplication.isPlaying = false;
                EditorUtility.DisplayDialog("Save System Integrity Error",
                        "Critical Save System errors found in your scripts.\n\n" +
                        "Play Mode has been blocked to prevent data corruption.\n\n" +
                        "Check the Console for specific script names.", "Ok,Will fix it");
            }
        }

        private static void ValidateCodebase(out bool errorFound)
        {
            errorFound = false;

            TypeCache.TypeCollection allScriptTypes = TypeCache.GetTypesDerivedFrom<MonoBehaviour>();

            foreach (Type type in allScriptTypes)
            {
                if (type.Assembly.FullName.StartsWith("Unity"))
                {
                    continue;
                }

                SaveableAttribute saveableAttribute = type.GetCustomAttribute<SaveableAttribute>();
                bool hasSaveInterface = ImplementsGenericInterface(type, typeof(ISaveable<>));

                if ((saveableAttribute != null))
                {
                    if ((!hasSaveInterface))
                    {
                        errorFound = true;
                        Debug.LogError($"<color=red>[SaveSystem Critical]</color> The script <b>'{type.Name}'</b> has the <b>[Saveable]</b> attribute but DOES NOT implement <b>ISaveable<></b> Interface.\n" +
                               $"You must implement the interface to ensure data Capture/Restore works.");

                    }
                }
                else
                {
                    if (hasSaveInterface)
                    {
                        Debug.LogError($"<color=red>[SaveSystem Critical]</color> The script <b>'{type.Name}'</b> implements <b>ISaveable<></b> Interface but is missing the <b>[Saveable(Category)]</b> attribute.\n" +
                           $"The Save System will ignore this script without the attribute.");
                        errorFound = true;
                    }
                }
            }
        }

        private static bool ImplementsGenericInterface(Type _type, Type _genericInterface)
        {
            return _type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == _genericInterface);
        }

    }
}
