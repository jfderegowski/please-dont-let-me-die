using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class LocalAssetStoreManager : EditorWindow
{
    private string assetStorePath = ""; // Path to Asset Store cache folder
    private List<string> assetNames = new List<string>();
    private List<string> assetPaths = new List<string>();
    private List<bool> selectedAssets = new List<bool>();
    private List<bool> importedAssets = new List<bool>(); // Tracks if each asset is already imported
    private Vector2 scrollPos;

    [MenuItem("Tools/Local Asset Store Manager")]
    public static void ShowWindow()
    {
        GetWindow<LocalAssetStoreManager>("Local Asset Store Manager");
    }

    private void OnEnable()
    {
        #if UNITY_EDITOR_WIN
            assetStorePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "Unity", "Asset Store-5.x");
        #elif UNITY_EDITOR_OSX
            assetStorePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Library", "Unity", "Asset Store-5.x");
        #endif

        // Load assets and check import status once on window open
        LoadLocalAssets();
        CheckImportedAssets();
    }

    private void LoadLocalAssets()
    {
        assetNames.Clear();
        assetPaths.Clear();
        selectedAssets.Clear();
        importedAssets.Clear();

        if (!Directory.Exists(assetStorePath))
        {
            Debug.LogError("Asset Store cache folder not found: " + assetStorePath);
            return;
        }

        // Find all .unitypackage files in the cache folder
        string[] files = Directory.GetFiles(assetStorePath, "*.unitypackage", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            string assetName = Path.GetFileNameWithoutExtension(file); // Get asset name
            assetNames.Add(assetName);
            assetPaths.Add(file);
            selectedAssets.Add(false); // Default all checkboxes to unchecked
            importedAssets.Add(false); // Default imported status to false
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Select Local Asset Store Packages to Import", EditorStyles.boldLabel);

        // Refresh button to reload assets and check their import status
        if (GUILayout.Button("Refresh Asset List"))
        {
            LoadLocalAssets();
            CheckImportedAssets();
        }

        // Scroll view for displaying all assets
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        for (int i = 0; i < assetNames.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            // Checkbox for selection
            selectedAssets[i] = EditorGUILayout.Toggle(selectedAssets[i], GUILayout.Width(20));

            // Asset name label
            EditorGUILayout.LabelField(assetNames[i]);

            // Show "Some Matching Name Was Found" if the asset is already imported
            if (importedAssets[i])
            {
                GUI.contentColor = Color.green;
                EditorGUILayout.LabelField("Some Matching Name Was Found");
                GUI.contentColor = Color.white; // Reset color
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Import Selected Assets"))
        {
            ImportSelectedAssets();
        }
    }

    private void ImportSelectedAssets()
    {
        for (int i = 0; i < assetPaths.Count; i++)
        {
            if (selectedAssets[i])
            {
                string assetPath = assetPaths[i];
                
                // Import the .unitypackage file into the project
                AssetDatabase.ImportPackage(assetPath, true);
                Debug.Log("Imported: " + assetPath);
                
                // Update the imported status for this asset
                importedAssets[i] = true;
            }
        }

        AssetDatabase.Refresh();
    }

    private void CheckImportedAssets()
    {
        for (int i = 0; i < assetNames.Count; i++)
        {
            importedAssets[i] = IsAssetImported(assetNames[i]);
        }
    }

    private bool IsAssetImported(string assetName)
    {
        // Use AssetDatabase to check if any asset contains the name in the project's Assets folder
        string[] guids = AssetDatabase.FindAssets(assetName, new[] { "Assets" });
        
        // If any GUID is found, the asset is considered imported
        return guids.Length > 0;
    }
}
