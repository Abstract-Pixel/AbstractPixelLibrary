using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;

namespace AbstractPixel.Utility.Save
{
    [CustomEditor(typeof(SaveManager))]
    public class SaveManagerEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            VisualElement debugContainer = new VisualElement();

            Foldout debugFoldOut = new Foldout { text = "Debug Controls" };
            VisualElement separatorFoldOut = new VisualElement();
            separatorFoldOut.style.height = 1;

            debugContainer.Add(separatorFoldOut);
            debugContainer.Add(debugFoldOut);

            Label editorControlsLabel = new Label("Editor Controls");
            Button deleteAllSavesButton = new Button();
            deleteAllSavesButton.text = "Delete All Saves";
            Button openReleaseSaveDirectory = new Button();
            openReleaseSaveDirectory.text = "Open Release Save Directory";
            Button openDebugSaveDirectory = new Button();
            openDebugSaveDirectory.text = "Open Debug Save Directory";
            VisualElement editorControlsSeparator = new VisualElement();
            editorControlsSeparator.style.height = 1;

            deleteAllSavesButton.clicked += DeleteAllSaves;
            openReleaseSaveDirectory.clicked += OpenReleaseDirectory;
            openDebugSaveDirectory.clicked += OpenDebugDirectory;

            debugFoldOut.Add(editorControlsLabel);
            debugFoldOut.Add(deleteAllSavesButton);
            debugFoldOut.Add(openReleaseSaveDirectory);
            debugFoldOut.Add(openDebugSaveDirectory);
            debugFoldOut.Add(editorControlsSeparator);

            Label runtimeControlsLabel = new Label("Runtime Controls");
            Button saveRuntimeDataButton = new Button();
            saveRuntimeDataButton.text = "SAVE ALL DATA !";
            Button loadRuntimeDataButton = new Button();
            loadRuntimeDataButton.text = "LOAD ALL DATA !";
            VisualElement runtimeControlsSeparator = new VisualElement();
            runtimeControlsSeparator.style.height = 1;

            if (!EditorApplication.isPlaying)
            {
                runtimeControlsLabel.SetEnabled(false);
                saveRuntimeDataButton.SetEnabled(false);
                loadRuntimeDataButton.SetEnabled(false);
            }

                saveRuntimeDataButton.clicked += () =>
            {
                if (Application.isPlaying)
                {
                    SaveManager saveManager = (SaveManager)target;
                    saveManager.SaveALL();
                }
                else
                {
                    Debug.LogWarning("Save and Load buttons only work in Play mode.");
                }
            };

            loadRuntimeDataButton.clicked += () =>
            {
                if (Application.isPlaying)
                {
                    SaveManager saveManager = (SaveManager)target;
                    saveManager.LoadALL();
                }
                else
                {
                    Debug.LogWarning("Save and Load buttons only work in Play mode.");
                }
            };

            debugFoldOut.Add(runtimeControlsLabel);
            debugFoldOut.Add(saveRuntimeDataButton);
            debugFoldOut.Add(loadRuntimeDataButton);
            debugFoldOut.Add(runtimeControlsSeparator);

            root.Add(debugContainer);
            return root;
        }

        private void InitializePathsInEditMode()
        {
            SerializedProperty configProperty = serializedObject.FindProperty("saveConfig");
            SaveSystemConfigSO configSO = configProperty.objectReferenceValue as SaveSystemConfigSO;
            if (configSO != null)
            {
                SavePathGenerator.Initialize(configSO);
            }
        }

        private void OpenReleaseDirectory()
        {
            InitializePathsInEditMode();
            string releasePath = SavePathGenerator.ShipRootPath;
            if (!Directory.Exists(releasePath))
            {
                Directory.CreateDirectory(releasePath);
            }
            EditorUtility.RevealInFinder(releasePath);
        }

        private void OpenDebugDirectory()
        {
            InitializePathsInEditMode();
            string debugPath = SavePathGenerator.DebugRootPath;
            if (!Directory.Exists(debugPath))
            {
                Directory.CreateDirectory(debugPath);
            }
            EditorUtility.RevealInFinder(debugPath);

        }

        private void DeleteAllSaves()
        {
            InitializePathsInEditMode();
            string releasePath = SavePathGenerator.ShipRootPath;
            string debugPath = SavePathGenerator.DebugRootPath;
            if (Directory.Exists(releasePath))
            {
                Directory.Delete(releasePath, true);
            }
            if (Directory.Exists(debugPath))
            {
                Directory.Delete(debugPath, true);
            }
            Debug.Log("All save files for both debug and release have been deleted.");
        }
    }
}
