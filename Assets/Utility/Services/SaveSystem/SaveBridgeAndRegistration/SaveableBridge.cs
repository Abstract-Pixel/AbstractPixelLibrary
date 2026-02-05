using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AbstractPixel.Utility.Save
{
    public class SaveableBridge : MonoBehaviour, ISaveableBridge
    {
        [field: SerializeField] public string UniqueId { get; private set; }
        
        [SerializeField] private List<SaveableTarget> saveableTargets = new List<SaveableTarget>();
        [SerializeField] private List<SaveCategory> foundCategoriesList = new List<SaveCategory>();

        private Dictionary<SaveCategory, List<SaveableTarget>> saveableTargetsRegistry;
        
        readonly string isaveableCaptureMethod = "CaptureData";
        readonly string isaveableRestoreMethod = "RestoreData";
        readonly string stringSepratorIdentifier = "#";

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(UniqueId))
            {
                string guid = Guid.NewGuid().ToString();
                UniqueId = $"{gameObject.name} GameObject {stringSepratorIdentifier}[{guid}]";
            }

            if (saveableTargets == null) saveableTargets = new List<SaveableTarget>();

            MonoBehaviour[] allScripts = GetComponents<MonoBehaviour>();
            List<MonoBehaviour> validScripts = new List<MonoBehaviour>();

            foreach (var script in allScripts)
            {
                if (script == null) continue;
                Type type = script.GetType();

                Type interfaceType = type.GetInterface(typeof(ISaveable<>).Name);
                if (interfaceType == null) continue;

                if (type.GetCustomAttribute<SaveableAttribute>() == null) continue;

                validScripts.Add(script);
            }

            //Clean up removed scripts from the list
            for (int i = saveableTargets.Count - 1; i >= 0; i--)
            {
                if (saveableTargets[i] == null || saveableTargets[i].Script == null || !validScripts.Contains(saveableTargets[i].Script))
                {
                    saveableTargets.RemoveAt(i);
                }
            }

            foreach (MonoBehaviour script in validScripts)
            {
                // Check if we already have a registration for this specific script reference
                SaveableTarget existingTarget = saveableTargets.FirstOrDefault(t => t.Script == script);

                if (existingTarget != null)
                {
                    // Update Debug Name (In case class was renamed, but we keep the GUID)
                    if (existingTarget.Identification != null)
                    {
                        existingTarget.Identification.ClassName = script.GetType().Name;
                    }
                }
                else
                {
                    string newGuid = Guid.NewGuid().ToString();
                    SaveableIdentification id = new SaveableIdentification(script.GetType().Name, newGuid);
                    saveableTargets.Add(new SaveableTarget(script, id));
                }
            }
        }

        private void Awake()
        {
            saveableTargetsRegistry = new Dictionary<SaveCategory, List<SaveableTarget>>();
            foundCategoriesList = new List<SaveCategory>();

            foreach (SaveableTarget target in saveableTargets)
            {
                if (target == null || target.Script == null) continue;

                Type componentType = target.Script.GetType();
                SaveableAttribute attribute = componentType.GetCustomAttribute<SaveableAttribute>();
                if (attribute == null) continue;
                
                Type interfaceType = componentType.GetInterface(typeof(ISaveable<>).Name);
                if (interfaceType == null) continue;

                target.CaptureDataMethod = componentType.GetMethod(isaveableCaptureMethod);
                target.RestoreDataMethod = componentType.GetMethod(isaveableRestoreMethod);
                target.DataToSaveType = interfaceType.GetGenericArguments()[0];

                if (!saveableTargetsRegistry.ContainsKey(attribute.Category))
                {
                    saveableTargetsRegistry.Add(attribute.Category, new List<SaveableTarget>());
                }
                saveableTargetsRegistry[attribute.Category].Add(target);

                if (!foundCategoriesList.Contains(attribute.Category))
                {
                    foundCategoriesList.Add(attribute.Category);
                }
            }
        }

        public object CaptureState(SaveCategory categoryFilter)
        {
            Dictionary<string, object> combinedCapturedDataMap = new Dictionary<string, object>();

            if (!saveableTargetsRegistry.TryGetValue(categoryFilter, out List<SaveableTarget> saveableTargetsList))
            {
                return null;
            }

            foreach (SaveableTarget target in saveableTargetsList)
            {
                object capturedData = target.CaptureDataMethod.Invoke(target.Script, null);
                if (capturedData != null)
                {
                    if (target.Identification != null && !string.IsNullOrEmpty(target.Identification.GUID))
                    {
                        string compositeKey = $"{target.Identification.ClassName} Component {stringSepratorIdentifier}{target.Identification.GUID}";
                        combinedCapturedDataMap.Add(compositeKey, capturedData);
                    }
                }
            }

            return combinedCapturedDataMap;
        }

        public void RestoreState(object data, SaveCategory categoryFilter)
        {
            Dictionary<string, object> combinedCapturedDataMap = SaveDataConverter.Convert<Dictionary<string, object>>(data);
            if (combinedCapturedDataMap == null) return;

            if (!saveableTargetsRegistry.TryGetValue(categoryFilter, out List<SaveableTarget> targetsList))
            {
                return;
            }

            Dictionary<string, SaveableTarget> guidToTargetMap = new Dictionary<string, SaveableTarget>();
            foreach (SaveableTarget target in targetsList)
            {
                if (target.Identification != null && !string.IsNullOrEmpty(target.Identification.GUID))
                {
                    if (!guidToTargetMap.ContainsKey(target.Identification.GUID))
                    {
                        guidToTargetMap.Add(target.Identification.GUID, target);
                    }
                }
            }

            foreach (KeyValuePair<string, object> kvp in combinedCapturedDataMap)
            {
                string compositeKey = kvp.Key;
                
                // Extract GUID from "ClassName#GUID"
                // We split by the LAST '#' to ensure we get the GUID at the end.
                int separatorIndex = compositeKey.LastIndexOf(stringSepratorIdentifier) + 1;
                
                string extractedGuid = (separatorIndex != -1) ? compositeKey.Substring(separatorIndex) : compositeKey;

                if (guidToTargetMap.TryGetValue(extractedGuid, out SaveableTarget target))
                {
                     object typedData = SaveDataConverter.Convert(kvp.Value, target.DataToSaveType);
                     target.RestoreDataMethod.Invoke(target.Script, new object[] { typedData });
                }
            }
        }

        private void OnEnable()
        {
            if (foundCategoriesList.Count > 0)
            {
                SaveManager.Instance.RegisterSaveableObject(this, foundCategoriesList);
            }
        }

        private void OnDisable()
        {
            if (SaveManager.Instance != null && foundCategoriesList.Count > 0)
            {
                SaveManager.Instance.UnregisterSaveableObject(this, foundCategoriesList);
            }
        }
    }
}