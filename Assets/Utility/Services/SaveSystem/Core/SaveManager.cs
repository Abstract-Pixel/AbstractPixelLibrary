using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AbstractPixel.Utility.Save
{
    public class SaveManager : MonoSingleton<SaveManager>
    {
        [SerializeField] private SaveSystemConfigSO saveConfig;
        private SaveProfileManager profileManager;
        private Dictionary<SaveCategory, Dictionary<string, ISaveableBridge>> SaveableObjectsRegistry;
        private IDataStorageService fileStorageService;
        private ISerializer serializer;

        // Remove Later
        string tempProfileID = "TEST_101";

        protected override void Awake()
        {
            base.Awake();
            saveConfig.Initialize();
            SavePathGenerator.Initialize(saveConfig);

            fileStorageService = new FileDataStorageService();
            serializer = new JsonSerializer();

            SaveableObjectsRegistry = new Dictionary<SaveCategory, Dictionary<string, ISaveableBridge>>();
            profileManager = new SaveProfileManager(fileStorageService, saveConfig, serializer);

            // For Testing purposes, replace and remove with actual profile loading and managment
            string profilePath = profileManager.CreateCustomProfileDirectory(tempProfileID);
            profileManager.SetCurrentActiveProfile(tempProfileID, profilePath);
        }

        public void SaveDataOf(SaveCategory _category)
        {

        }

        public void LoadDataOf(SaveCategory _category)
        {

        }

        public void SaveALL()
        {
            string fileExtension = SavePathGenerator.PrimaryFileExtension;
            string activeProfileId = profileManager.CurrentProfileID;

            foreach (SaveCategory category in SaveableObjectsRegistry.Keys)
            {
                Dictionary<string, ISaveableBridge> BridgesDataMap = SaveableObjectsRegistry[category];
                SaveCatgeoryDefinition categoryDefinition = saveConfig.GetCategoryDefinition(category);

                SaveFileData categorizedSaveFileData;
                string fileName = SavePathGenerator.GetFileNameBasedOnCategoryDefinition(categoryDefinition);
                string fullSavePath = SavePathGenerator.GetPath(categoryDefinition, activeProfileId);

                if (categoryDefinition.DirectoryScope == SaveScope.Global)
                {
                    categorizedSaveFileData = new SaveFileData(fileName);
                }
                else
                {
                    categorizedSaveFileData = new SaveFileData(fileName, activeProfileId);
                }

                foreach (string key in BridgesDataMap.Keys)
                {
                    ISaveableBridge bridge = BridgesDataMap[key];
                    if (bridge == null) continue;



                    object capturedData = bridge.CaptureState(category);
                    if (capturedData == null)
                    {
                        continue;
                    }
                    categorizedSaveFileData.DataMap.Add(bridge.UniqueId, capturedData);
                }

                if (serializer.TrySerialize(categorizedSaveFileData, out string json))
                {
                    fileStorageService.SaveFile(json, fullSavePath);

                }



            }

        public void LoadALL()
        {
            string profileId = profileManager.CurrentProfileID;
            foreach (SaveCatgeoryDefinition defintion in saveConfig.GetAllCategoryDefintions())
            {
                string loadPath = SavePathGenerator.GetPath(defintion, profileId);
                if (!fileStorageService.FileExists(loadPath)) continue;

                string loadedjson = fileStorageService.LoadFile(loadPath);
                if (string.IsNullOrEmpty(loadedjson)) continue;
                if (!serializer.TryDeserialize(loadedjson, out SaveFileData loadedData))
                {
                    Debug.LogError($"Failed to deserialize file at {loadPath}");
                    continue;
                }
          

                if (!SaveableObjectsRegistry.TryGetValue(defintion.Category, out Dictionary<string, ISaveableBridge> bridgesDataMap))
                {
                    // No Live Objects registered Under This Category
                    continue;
                }

                foreach (KeyValuePair<string, object> kvp in loadedData.DataMap)
                {
                    string guid = kvp.Key;
                    object objectData = kvp.Value;

                    if(bridgesDataMap.TryGetValue(guid,out ISaveableBridge bridge))
                    {
                        bridge.RestoreState(objectData,defintion.Category);
                    }   
                    else
                    {
                        // LATER: This is where you spawn objects that don't exist yet
                    }
                }
            }
        }


        public void RegisterSaveableObject(ISaveableBridge bridge, List<SaveCategory> categories)
        {
            if (IsInstanceNull()) return;
            if (bridge == null || categories == null) return;

            foreach (SaveCategory category in categories)
            {
                // 1. Ensure the Bucket (Inner Dictionary) exists for this Category
                if (!SaveableObjectsRegistry.ContainsKey(category))
                {
                    SaveableObjectsRegistry.Add(category, new Dictionary<string, ISaveableBridge>());
                }

                Dictionary<string, ISaveableBridge> categoryBucket = SaveableObjectsRegistry[category];

                // 3. APPEND the Bridge (Safe Check)
                if (!categoryBucket.ContainsKey(bridge.UniqueId))
                {
                    categoryBucket.Add(bridge.UniqueId, bridge);
                }
                else
                {
                    // Optional: Update the reference if it somehow got recreated
                    categoryBucket[bridge.UniqueId] = bridge;
                }
            }
        }

        public void UnregisterSaveableObject(ISaveableBridge bridge, List<SaveCategory> categories)
        {
            if (IsInstanceNull()) return;
            if (bridge == null || categories == null) return;

            foreach (SaveCategory category in categories)
            {
                if (SaveableObjectsRegistry.TryGetValue(category, out var categoryBucket))
                {
                    if (categoryBucket.ContainsKey(bridge.UniqueId))
                    {
                        categoryBucket.Remove(bridge.UniqueId);
                    }
                }
            }
        }

        private bool IsInstanceNull()
        {
            if (instance == null)
            {
                Debug.LogError("SaveManager instance is null. Ensure SaveManager is initialized before use and present in the scene ");
                return true;
            }
            return false;
        }

    }
}
