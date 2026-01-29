using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AbstractPixel.Utility.Save
{
    public class SaveManager : MonoSingleton<SaveManager>
    {
        [SerializeField] private SaveSystemConfigSO saveConfig;
        private SaveProfileManager profileManager;
        private Dictionary<string, ISaveable> SaveableObjectsRegistry;
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

            SaveableObjectsRegistry = new Dictionary<string, ISaveable>();
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
            Dictionary<string, ISaveable>.ValueCollection allSaveables = SaveableObjectsRegistry.Values;
            IEnumerable<IGrouping<SaveCategory, ISaveable>> groupedSaveables = allSaveables.GroupBy(saveable => saveable.saveCategory);
            string fileExtension = SavePathGenerator.PrimaryFileExtension;
            string activeProfileId = profileManager.CurrentProfileID;
            foreach (IGrouping<SaveCategory, ISaveable> group in groupedSaveables)
            {
                SaveCategory category = group.Key;
                string profileId = profileManager.CurrentProfileID;
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
                    categorizedSaveFileData = new SaveFileData(fileName,activeProfileId);
                }

                foreach (ISaveable saveable in group)
                {
                    object capturedData = saveable.CaptureData();
                    if (capturedData == null)
                    {
                        continue;
                    }
                    categorizedSaveFileData.DataMap.Add(saveable.Guid, capturedData);
                }

                if(serializer.TrySerialize(categorizedSaveFileData, out string  json))
                {
                    fileStorageService.SaveFile(json, fullSavePath);

                }
            }
        }

        public void LoadALL()
        {

        }

        public void RegisterSaveableObject(string _uniqueId, ISaveable _saveable)
        {
            if (IsInstanceNull()) return;
            if (!SaveableObjectsRegistry.ContainsKey(_uniqueId))
            {
                SaveableObjectsRegistry.Add(_uniqueId, _saveable);
            }
        }

        public void UnregisterSaveableObject(string _uniqueId)
        {
            if (IsInstanceNull()) return;
            if (SaveableObjectsRegistry.ContainsKey(_uniqueId))
            {
                SaveableObjectsRegistry.Remove(_uniqueId);
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
