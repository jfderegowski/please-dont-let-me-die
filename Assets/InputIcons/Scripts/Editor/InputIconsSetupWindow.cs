using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEditor.Compilation;
using System.IO;
using UnityEditorInternal;

namespace InputIcons
{
    public class InputIconsSetupWindow : EditorWindow
    {
        Vector2 scrollPos;

        bool showPart1 = false;
        bool showPart2 = false;

        bool showCopyFontsPart = false;

        bool showAdvanced = false;
        GUIStyle textStyleHeader;
        GUIStyle textStyleHeader2;
        GUIStyle textStyle;
        GUIStyle textStyleYellow;
        GUIStyle textStyleBold;
        GUIStyle buttonStyle;

        private Editor editor;


        private List<InputIconSetBasicSO> iconSetSOs;
        //public List<InputActionAsset> usedInputActionAssets;

        private InputIconsManagerSO managerSO;

        public static SerializedObject serializedManager;
        public static SerializedProperty serializedInputActionAssets;

        private ReorderableList keyboardSchemeNames;
        private ReorderableList gamepadSchemeNames;

        private bool showDeprecatedSection = false;
        public GameObject activationPrefab;

        [MenuItem("Tools/Input Icons/Select Input Icons Manager", priority = 1)]
        public static void SelectManager()
        {
            Selection.activeObject = InputIconsManagerSO.Instance;
        }

        [MenuItem("Tools/Input Icons/Input Icons Setup", priority = 0)]
        public static void ShowWindow()
        {
            const int width = 1200;
            const int height = 610;

            var x = (Screen.currentResolution.width - width) / 2;
            var y = (Screen.currentResolution.height - height) / 2;

            GetWindow<InputIconsSetupWindow>("Input Icons Setup").iconSetSOs = InputIconSetConfiguratorSO.GetAllIconSetsOnConfigurator();
            EditorWindow window = GetWindow<InputIconsSetupWindow>("Input Icons Setup");
            window.position = new Rect(x, y, width, height);
        }

        protected void OnEnable()
        {
            // load values
            var data = EditorPrefs.GetString("InputIconsSetupWindow", JsonUtility.ToJson(this, false));
            JsonUtility.FromJsonOverwrite(data, this);

            position.Set(position.x, position.y, 1000, 800);

            managerSO = InputIconsManagerSO.Instance;

            serializedManager = new SerializedObject(InputIconsManagerSO.Instance);
            serializedInputActionAssets = serializedManager.FindProperty("usedActionAssets");

            activationPrefab = Resources.Load("InputIcons/II_InputIconsActivator") as GameObject;

            //do this so the list does not appear null ... weird bug that otherwise can happen when package just got imported
            serializedInputActionAssets.InsertArrayElementAtIndex(0);
            var elementProperty = serializedInputActionAssets.GetArrayElementAtIndex(0);
            if(elementProperty!=null)
                elementProperty.objectReferenceValue = null;
            serializedInputActionAssets.DeleteArrayElementAtIndex(0);

            DrawCustomContextList();

        }

        protected void OnDisable()
        {
            // save values
            var data = JsonUtility.ToJson(this, false);
            EditorPrefs.SetString("InputIconsSetupWindow", data);
        }

        private bool AllInputActionAssetsNull()
        {
            if (serializedInputActionAssets.arraySize == 0)
                return true;

            for(int i=0; i< serializedInputActionAssets.arraySize; i++)
            {
                if (serializedInputActionAssets.GetArrayElementAtIndex(i).objectReferenceValue as System.Object as InputActionAsset != null)
                    return false;
            }

            return true;

        }
        private void OnGUI()
        {

            textStyleHeader = new GUIStyle(EditorStyles.boldLabel);
            textStyleHeader.wordWrap = true;
            textStyleHeader.fontSize = 16;

            textStyleHeader2 = new GUIStyle(EditorStyles.boldLabel);
            textStyleHeader2.wordWrap = true;
            textStyleHeader2.fontSize = 14;

            textStyle = new GUIStyle(EditorStyles.label);
            textStyle.wordWrap = true;

            textStyleYellow = new GUIStyle(EditorStyles.label);
            textStyleYellow.wordWrap = true;
            textStyleYellow.normal.textColor = Color.yellow;

            textStyleBold = new GUIStyle(EditorStyles.boldLabel);
            textStyleBold.wordWrap = true;

            buttonStyle = EditorStyles.miniButtonMid;

            scrollPos =
               EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true));

            GUILayout.Space(10);
            managerSO = (InputIconsManagerSO)EditorGUILayout.ObjectField("", managerSO, typeof(InputIconsManagerSO), true);
            if (managerSO == null)
            {
                EditorGUILayout.HelpBox("Select the icon manager.", MessageType.Warning);
                EditorGUILayout.EndScrollView();
                return;
            }

            GUILayout.Space(10);

#if !ENABLE_INPUT_SYSTEM
            // New input system backends are enabled.
                        EditorGUILayout.HelpBox("Enable the new Input System in Project Settings for full functionality.\n" +
                "Project Settings -> Player -> Other Settings. Set Active Input Handling to 'Input System Package (new)' or 'Both'.", MessageType.Warning);
#endif


           

            //DrawUILine(Color.grey);

           


            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(400));


            DrawUILine(Color.grey);
            DrawUILine(Color.grey);
            GUILayout.Label("1) Base Setup (for Sprites and Images)", textStyleHeader);

            GUILayout.Label("When looking for matching bindings, the tool will search for bindings specified in the following lists.", textStyle);
            GUILayout.Space(5);
            DrawControlSchemePart();

            GUILayout.Space(5);
            DrawCurrentInputActionAssetsList();
            GUILayout.Label("When using the rebind buttons, overrides to bindings in the input action assets in the list will be automatically " +
    "saved to player prefs and reloaded when the manager becomes active.", textStyle);


            GUILayout.Space(10);
            // Create a foldout
            GUILayout.Label("Required step in earlier versions, should not be needed anymore", textStyleBold);
            EditorGUI.indentLevel++;
            showDeprecatedSection = EditorGUILayout.Foldout(showDeprecatedSection, "Show Deprecated Section");
            
            if (showDeprecatedSection)
            {
                EditorGUI.indentLevel++;

                
                GUILayout.Label("Add the activation prefab to your first scene (or one of your early scenes). This ensures the InputIconsManager will be active in our builds " +
                    "and will update the displayed icons when needed.", textStyle);
                EditorGUI.BeginDisabledGroup(true);
                activationPrefab = (GameObject)EditorGUILayout.ObjectField("", activationPrefab, typeof(GameObject), true);
                EditorGUI.EndDisabledGroup();

                EditorGUI.indentLevel--;
            }



            GUILayout.Space(10);
            GUILayout.Label("Now you can already display bindings with the following components.", textStyle);

            GUILayout.Space(5);

            GUILayout.Label("II_ImagePrompt\n" +
               "II_SpritePrompt\n" +
               "II_LocalMultiplayerImagePrompt\n" +
               "II_LocalMultiplayerSpritePrompt", textStyleBold);

            GUILayout.Space(5);

            GUILayout.Label("You can also use the II_UIRebindInputActionImageBehaviour to rebind your actions.", textStyle);

            GUILayout.Space(15);
            DrawUILine(Color.grey);
            GUILayout.Label("Customization", textStyleHeader);
            DrawCustomPartPackSpriteAssets();


            GUILayout.Space(10);
            GUILayout.Label("For additional settings and customizations, select the manager or configurator scriptable object below.", textStyle);
            EditorGUI.BeginDisabledGroup(true);
            managerSO = (InputIconsManagerSO)EditorGUILayout.ObjectField("", managerSO, typeof(InputIconsManagerSO), true);
            InputIconSetConfiguratorSO.Instance = (InputIconSetConfiguratorSO)EditorGUILayout.ObjectField("", InputIconSetConfiguratorSO.Instance, typeof(InputIconsManagerSO), true);
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(5);


            EditorGUILayout.EndVertical();





            DrawUILineVertical(Color.grey, 2, 30);




            EditorGUILayout.BeginVertical(GUILayout.Width(400));
            DrawUILine(Color.grey);
            DrawUILine(Color.grey);

            GUILayout.Label("2) Setup for TextMeshPro", textStyleHeader);
            GUILayout.Label("To display bindings inline with other TextMeshPro text, we need to create TMP_SpriteAssets in the default sprite asset folder.\n\n" +
                "Additionally if we want to use the style tag to display bindings, we have to add entries to the default TMP style sheet for each of our actions within our Input Action Assets.", textStyle);

            GUILayout.Space(10);
            DrawUILine(Color.grey);


            GUILayout.Label("Setup text components", textStyleHeader2);
            managerSO.TEXTMESHPRO_SPRITEASSET_FOLDERPATH = EditorGUILayout.TextField("Default Sprite Asset folder: ", managerSO.TEXTMESHPRO_SPRITEASSET_FOLDERPATH);

            GUILayout.Space(20);

            DrawFontPart();

            GUILayout.Space(20);
            GUILayout.Label("Lets create the TMP_SpriteAssets we need to display sprites within text using the sprite-tag.", textStyle);
            if (GUILayout.Button("Create Sprite Assets (and font assets if enabled above)"))
            {
                if (managerSO.isUsingFonts)
                {
                    CopyUsedFontAssetsToTMProDefaultFolder();
                }

                InputIconsSpritePacker.PackIconSets();
                InputIconsLogger.Log("Packing button icons completed");
            }

            if (GUILayout.Button("Recompile (might be needed for the \nnew Sprite Asset to take effect)", GUILayout.Height(35)))
            {
                CompilationPipeline.RequestScriptCompilation();
            }

            GUILayout.Space(5);
            GUILayout.Label("With the TMP_SpriteAssets created we can now use the following components to display bindings within text:", textStyle);
            GUILayout.Space(5);
            GUILayout.Label("II_TextPrompt\n" +
                "II_LocalMultiplayerTextPrompt", textStyleBold);

            GUILayout.Space(5);
            GUILayout.Label("You can recreate the Sprite Assets at any time when needed using the button above, " +
                "for example if you select different icon sets or make changes to the icon sets.", textStyle);


            GUILayout.Space(10);
            DrawUILine(Color.grey);
            DrawUILine(Color.grey);

            GUILayout.Label("3) Setup for TextMeshPro style tag", textStyleHeader);

            GUILayout.Label("Note: Although the style tag can be a convenient way to display bindings, it also comes with limitations and disadvantages. Some of them are:", textStyle);
            GUILayout.Label("* Multiple device types can not be displayed at the same time", textStyle);
            GUILayout.Label("* Only the first control scheme in the control scheme list(s) can be displayed", textStyle);
            GUILayout.Label("* The option to display all available bindings for actions is global and affects all texts at the same time", textStyle);
            GUILayout.Label("* No inherent support for local multiplayer", textStyle);
            GUILayout.Label("* When adding actions or renaming actions, you need to do the setup below again to reflect the changes", textStyle);

           
            GUILayout.Space(10);
            GUILayout.Label("If you use style tags, enable the following option to automatically update the style sheet when the user switches devices." +
                "\nKeep it disabled if you don't use this feature to save calculation time.", textStyle);
            managerSO.automaticStyleSheetUpdatingEnabled = EditorGUILayout.Toggle("Style Sheet Autoupdates", managerSO.automaticStyleSheetUpdatingEnabled);

            GUILayout.Space(10);
            GUILayout.Label("Select the Input Action Asset(s) you want to display inline with text using the style tag.\n" +
                   "Note: When you make changes to your Input Action Assets like adding actions, you will need to create the necessary styles for the new actions. You can easily do this in the Manual Setup section or in the Automatic Setup section below.", textStyle);


            DrawCurrentInputActionAssetsList();

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();

            //Quick Setup
            EditorGUILayout.BeginVertical(GUILayout.Width(300));

            GUILayout.Label("Automatic Setup", textStyleHeader2);
            DrawQuickSetup();

            EditorGUILayout.EndVertical();

            
            EditorGUILayout.BeginVertical(GUILayout.Width(300));
            EditorGUILayout.EndVertical();

            DrawUILineVertical(Color.grey);

            EditorGUILayout.BeginVertical(GUILayout.Width(10));
            EditorGUILayout.EndVertical();


            //manual setup
            EditorGUILayout.BeginVertical(GUILayout.Width(350));

            GUILayout.Label("Manual Setup", textStyleHeader2);
            GUILayout.Label("To setup the style tags manually (might be faster if you do this often), complete the following steps.\n", textStyle);

            //GUILayout.Space(10);
            //DrawCustomPartCopyFontAssetsAssets();

            DrawCustomPartStyleSheetUpdate();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(15);

            DrawBottomPart();

            GUILayout.Space(5);
            DrawUILine(Color.grey);
            GUILayout.Space(5);

            DrawAdvanced();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();


            DrawUILine(Color.grey);
            DrawUILine(Color.grey);

            GUILayout.Space(10);

           
            
            EditorGUILayout.EndScrollView();

            serializedManager.ApplyModifiedProperties();
        }

        private void DrawFontPart()
        {
            GUILayout.Label("(Optional) Enable font styles as well.", textStyleBold);
            GUILayout.Label("Enabling this allows you to display Input Icons as a SDF font in TMPro texts as well.\n" +
                "If enabled,  additional styles for fonts will be created in the default style sheet during the setup process.", textStyle);

         
            managerSO.TEXTMESHPRO_FONTASSET_FOLDERPATH = EditorGUILayout.TextField("Default Font folder: ", managerSO.TEXTMESHPRO_FONTASSET_FOLDERPATH);

            EditorGUI.BeginChangeCheck();
            managerSO.isUsingFonts = EditorGUILayout.Toggle("(Optional) Use fonts", managerSO.isUsingFonts);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(InputIconsManagerSO.Instance);
                GameObject temp = Selection.activeGameObject;
                if(temp!=null)
                {
                    II_UIRebindInputActionBehaviour rebindBehaviour = temp.GetComponent<II_UIRebindInputActionBehaviour>();
                    if (rebindBehaviour)
                        EditorUtility.SetDirty(temp);
                }
          
            }

            if (managerSO.isUsingFonts && !CheckAllSelectedIconSetsHaveFontsAssigned())
            {
                GUILayout.Label("WARNING: not all Icon Sets on InputIconsConfigurator have a font asset assigned.\n" +
                    "Make sure each has one before you do the setup. Assign font assets and do the Quick Setup (or in the manual setup the Update TMPro Style Sheet - part) again to ensure functionality.", textStyleYellow);

                InputIconSetConfiguratorSO.Instance = (InputIconSetConfiguratorSO)EditorGUILayout.ObjectField("", InputIconSetConfiguratorSO.Instance, typeof(InputIconSetConfiguratorSO), true);
            }
        }

        private bool CopyObjectToDefaultFontsFolder(UnityEngine.Object obj)
        {
            if(obj != null)
            {
                string sourcePath = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(sourcePath))
                {
                    string destinationFolderPath = managerSO.TEXTMESHPRO_FONTASSET_FOLDERPATH + obj.name+".asset";

                    if (File.Exists(destinationFolderPath))
                    {
                        FileUtil.DeleteFileOrDirectory(destinationFolderPath);
                        AssetDatabase.Refresh();
                    }

                    FileUtil.CopyFileOrDirectory(sourcePath, destinationFolderPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    return true;
                }
            }
            return false;
        }

        private void CopyUsedFontAssetsToTMProDefaultFolder()
        {
            InputIconsLogger.Log("Copying font assets ...");
            List<InputIconSetBasicSO> iconSets = InputIconSetConfiguratorSO.GetAllIconSetsOnConfigurator();
            int c = 0;
            for (int i = 0; i < iconSets.Count; i++)
            {
                if (iconSets[i] == null)
                    continue;
                if (iconSets[i].fontAsset == null)
                    continue;

                if (CopyObjectToDefaultFontsFolder(iconSets[i].fontAsset))
                    c++;
            }
            InputIconsLogger.Log(c+ " font assets copied and ready to be referenced in TMPro text fields");
            
        }

        private bool CheckAllSelectedIconSetsHaveFontsAssigned()
        {
            List<InputIconSetBasicSO> iconSets = InputIconSetConfiguratorSO.GetAllIconSetsOnConfigurator();
            for(int i=0; i<iconSets.Count; i++)
            {
                if (iconSets[i] != null)
                {
                    if (iconSets[i].fontAsset == null)
                        return false;
                }
            }
            return true;
        }

        private void DrawQuickSetup()
        {
            

            if(AllInputActionAssetsNull())
            {
                EditorGUILayout.HelpBox("Select an Input Asset before you continue.", MessageType.Warning);
            }
            else
            {
                GUILayout.Space(3);

                GUILayout.Label("To add the style tag functionality, use the buttons below. Wait for Unity to recompile after the first button press.", textStyle);

                if (GUILayout.Button("Step 1: Prepare the Default Style Sheet with empty values\n" +
                    "(then wait for compilation)"))
                {
                    

                    managerSO.CreateInputStyleData();
                    int c = 0;
                    c += InputIconsManagerSO.PrepareAddingInputStyles(managerSO.inputStyleKeyboardDataList);
                    c += InputIconsManagerSO.PrepareAddingInputStyles(managerSO.inputStyleGamepadDataList);
             
                    InputIconsLogger.Log("TMP style sheet prepared with "+c+" empty values.");
                    if(c==0)
                    {
                        InputIconsLogger.LogWarning(c + " empty entries added which is generally not expected. Try the same step again.");
                    }

                    CompilationPipeline.RequestScriptCompilation();
                }

                if (!EditorApplication.isCompiling)
                {
                    if (GUILayout.Button("Step 2: Add Input Action names to Style Sheet"))
                    {
                        InputIconsLogger.Log("Adding entries to default TMP style sheet ...");

                        managerSO.CreateInputStyleData();
                        int c = 0;
                        c += InputIconsManagerSO.AddInputStyles(managerSO.inputStyleKeyboardDataList);
                        c += InputIconsManagerSO.AddInputStyles(managerSO.inputStyleGamepadDataList);

                        InputIconsLogger.Log("TMP style sheet updated with ("+ c+ ") styles (multiple entries combined to only one)");
                        
                        InputIconsManagerSO.UpdateTMProStyleSheetWithUsedPlayerInputs();
                        TMP_InputStyleHack.RemoveEmptyEntriesInStyleSheet();
                        //TMP_InputStyleHack.RefreshAllTMProUGUIObjects();
                    }
                }
                else
                {

                    GUILayout.Label("... waiting for compilation ...", textStyleYellow);
                }
            }
        }


        private void DrawControlSchemePart()
        {
            GUILayout.Label("Verify below that the control scheme names for the keyboard and gamepad match the case-insensitive names " +
                "configured in your Input Action Asset(s).", textStyleBold);
            GUILayout.Label("Also, changing control scheme names in the Input Action Assets and saving them won't always yield immediate results. Rebuilding the domain, by recompiling " +
             "or entering play mode with 'Enter Play Mode Options' disabled in 'Project Settings -> Editor' updates the Input Action Assets correctly.", textStyle);

            GUILayout.Space(5);
            GUILayout.Label("Control scheme names that are higher up in the list have priority. Order matters.", textStyle);


           GUILayout.Space(5);

            if (managerSO)
            {
                keyboardSchemeNames.DoLayoutList();
                gamepadSchemeNames.DoLayoutList();
            }
            GUILayout.Label("Note: If you make changes to these entries, the example scenes might not work anymore, " +
                "as they are tuned to the default values.", textStyle);

        }

        private void DrawCustomPartPackSpriteAssets()
        {
            //GUILayout.Label("Part 1: Packing Input Icon Sets to Sprite Assets", EditorStyles.boldLabel);
     
            GUILayout.Label("Select your default Icon Sets. Icon Sets hold the sprites that will get displayed.", textStyleBold);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            showPart1 = EditorGUILayout.Foldout(showPart1, "Choose which sprites to use for displaying input actions.");
            if (showPart1)
            {

                EditorGUILayout.BeginVertical(GUI.skin.window);
                InputIconSetConfiguratorSO.Instance = (InputIconSetConfiguratorSO)EditorGUILayout.ObjectField("", InputIconSetConfiguratorSO.Instance, typeof(InputIconSetConfiguratorSO), true);
                EditorGUILayout.HelpBox("The sprites in the sets below " +
                              "will be used to create sprite assets.\n\n" +
                              "If you want to use different sprites, change them before you pack them.", MessageType.None);
                
                if(InputIconSetConfiguratorSO.Instance != null)
                {
                    EditorGUI.BeginChangeCheck();
                    DrawIconSets();
                    if(EditorGUI.EndChangeCheck())
                    {
                        InputIconSetBasicSO guessedNewGamepadIconSet = InputIconSetConfiguratorSO.Instance.GetGuessedNewlyAssignedGamepadIconSet();
                        if(guessedNewGamepadIconSet != null)
                        {
                            InputIconSetConfiguratorSO.SetCurrentIconSet(guessedNewGamepadIconSet);
                            InputIconSetConfiguratorSO.Instance.RememberAssignedGamepadIconSets();
                            InputIconsManagerSO.UpdatePromptDisplayBehavioursManually();
                        }
                        InputIconsManagerSO.UpdateStyleData();

                        InputIconSetConfiguratorSO.SetCurrentIconSet(InputIconSetConfiguratorSO.Instance.keyboardIconSet);
                        InputIconsManagerSO.UpdatePromptDisplayBehavioursManually();

                        EditorUtility.SetDirty(InputIconSetConfiguratorSO.Instance);
                    }
                }
                 
                EditorGUILayout.EndVertical();
            }

            GUILayout.Space(5);
            if (GUILayout.Button("(Re-)Create Sprite Assets"))
            {
                if (managerSO.isUsingFonts)
                {
                    CopyUsedFontAssetsToTMProDefaultFolder();
                }

                InputIconsSpritePacker.PackIconSets();
                InputIconsLogger.Log("Packing button icons completed");
            }

            if (GUILayout.Button("Recompile (might be needed for the \nnew Sprite Asset to take effect)", GUILayout.Height(35)))
            {
                CompilationPipeline.RequestScriptCompilation();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawCustomPartCopyFontAssetsAssets()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("Support displaying bindings as fonts", textStyleBold);
            showCopyFontsPart = EditorGUILayout.Foldout(showCopyFontsPart, "Copy fonts");
            if (showCopyFontsPart)
            {
                EditorGUILayout.HelpBox("By pressing this button, the font assets referenced in the used Input Icon Sets " +
                   "will be copies to the resources folder of TMPro to be accessible when needed.", MessageType.None);

                if (GUILayout.Button("Copy Font Assets to Default Font folder", buttonStyle))
                {
                    CopyUsedFontAssetsToTMProDefaultFolder();
                }

                EditorGUILayout.HelpBox("To be able to display fonts, make sure the necessary styles are in the default style sheet.\n" +
                    "You can use the \"Update TMPro Style Sheet\" section below to add the styles.", MessageType.None);

            }
            EditorGUILayout.EndVertical();


        }

        private void DrawCustomPartStyleSheetUpdate()
        {
            

            GUILayout.Label("Update TMPro Style Sheet", textStyleBold);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            showPart2 = EditorGUILayout.Foldout(showPart2, "Add or update input action asset in default style sheet");
            if (showPart2)
            {

                GUILayout.Label("Check the 'Used Input Action Assets' list above if it contains all desired Input Action Assets.", textStyle);

                if(AllInputActionAssetsNull())
                {
                    { EditorGUILayout.HelpBox("Select an Input Asset before you continue.", MessageType.Warning); }
                }
                else
                {
                    if (serializedInputActionAssets.GetArrayElementAtIndex(0) != null)
                    {
                        GUILayout.Space(10);


                        GUILayout.Label("First prepare the default style sheet.", textStyle);

                        if (GUILayout.Button("Prepare style sheet manually\n(faster, but needs you to update style sheet)"))
                        {
                            InputIconsLogger.Log("Preparing default TMP style sheet for additional entries ...");
                            managerSO.CreateInputStyleData();
                            int c = 0;
                            c += InputIconsManagerSO.PrepareAddingInputStyles(managerSO.inputStyleKeyboardDataList);
                            c += InputIconsManagerSO.PrepareAddingInputStyles(managerSO.inputStyleGamepadDataList);

                            InputIconsLogger.Log("TMP style sheet prepared with " + c + " empty values.");
                            if (c == 0)
                            {
                                InputIconsLogger.LogWarning(c + " empty entries added which is generally not expected. Try the same step again.");
                            }

                        }
                        GUILayout.Label("IMPORTANT: UPDATE THE STYLE SHEET.", textStyleBold);
                        GUILayout.Label("The default style sheet should now be open in the inspector. " +
                            "Make a small change in any field of the style sheet and undo it again. " +
                            "Then continue with the next step.", textStyle);


                        GUILayout.Space(5);

                        DrawUILine(Color.grey);
                        GUILayout.Space(5);

                        GUILayout.Label("Now we can add/update the style sheet with our Input Action Assets.", textStyle);


                        if (!EditorApplication.isCompiling)
                        {
                            if (GUILayout.Button("Add Input Asset styles to default TMP style sheet"))
                            {
                                InputIconsLogger.Log("Adding entries default TMP style sheet for additional entries ...");
                                managerSO.CreateInputStyleData();
                                int c = 0;
                                c += InputIconsManagerSO.AddInputStyles(managerSO.inputStyleKeyboardDataList);
                                c += InputIconsManagerSO.AddInputStyles(managerSO.inputStyleGamepadDataList);

                                InputIconsLogger.Log("TMP style sheet updated with (" + c + ") styles (multiple entries combined to only one)");

                                TMP_InputStyleHack.RemoveEmptyEntriesInStyleSheet();
                            }
                        }
                        else
                        {
                            GUILayout.Label("... waiting for compilation ...", textStyleYellow);
                        }


                        GUILayout.Label("Have a look at the style sheet. You should find entries with the name of your input actions. " +
                            "The opening and closing tags might be empty but they will get filled once you start the game.", textStyle);
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawBottomPart()
        {

            GUILayout.Label("Once you have completed these steps, you can now also use the TMP style tags to display bindings in texts.", textStyle);

            GUILayout.Space(5);

            GUILayout.Label("Type <style=NameOfActionMap/NameOfAction> to display the bindings of the action.\n" +
                   "For example: <style=platformer controls/move> will display the bindings of the move action " +
                   "in the platformer controls action map.\n" +
                   "<style=platformer controls/jump> would display the \'Jump\' action respectively.\n" +
                   "To display a single action of a composite binding, type <style=platformer controls/move/down> for example.\n" +
                   "\n" +
                   "All available bindings are saved in the Input Icons Manager for quick access.\n" +
                   "Open \"Tools -> Input Icons -> Input Icons TMPro Style List Window\". Copy and paste an entry of the\n" +
                   "TMPro Style Tag column into a text field to display the corresponding binding." +
                   "", textStyle);

            GUILayout.Space(5);

            GUILayout.Label("The InputIconsManager provides displaying options for the style tag and more.", textStyle);
            managerSO = (InputIconsManagerSO)EditorGUILayout.ObjectField("", managerSO, typeof(InputIconsManagerSO), true);

            /*
            GUILayout.Label("How to use the Input Icons style tag", textStyleHeader);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            showPart3 = EditorGUILayout.Foldout(showPart3, "Using Input Icons");
            if (showPart3)
            {
                GUILayout.Space(8);
                GUILayout.Label("Displaying Input Icons", textStyleBold);
               

                GUILayout.Space(8);
                GUILayout.Label("Customization", textStyleBold);
            
            }
       
            EditorGUILayout.EndVertical();
            */

        }

        private void DrawAdvanced()
        {
            GUILayout.Label("Advanced", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            showAdvanced = EditorGUILayout.Foldout(showAdvanced, "TMP Style Sheet manipulation");
            if (showAdvanced)
            {
                EditorGUILayout.HelpBox("You can use this button to remove Input Icons style" +
                    " entries from the TMPro style sheet.", MessageType.Warning);

                var style = new GUIStyle(GUI.skin.button);

                if (GUILayout.Button("Remove all Input Icon styles of current Input Action Asset(s) from the TMPro style sheet.", style))
                {
                    InputIconsManagerSO.RemoveAllStyleSheetEntries();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawIconSets()
        {
            InputIconSetConfiguratorSO configurator = InputIconSetConfiguratorSO.Instance;
            if(configurator)
            {
                configurator.keyboardIconSet = (InputIconSetKeyboardSO)EditorGUILayout.ObjectField("Keyboard Icons", configurator.keyboardIconSet, typeof(InputIconSetKeyboardSO), true);
                configurator.ps3IconSet = (InputIconSetGamepadSO)EditorGUILayout.ObjectField("PS3 Icons", configurator.ps3IconSet, typeof(InputIconSetGamepadSO), true);
                configurator.ps4IconSet = (InputIconSetGamepadSO)EditorGUILayout.ObjectField("PS4 Icons", configurator.ps4IconSet, typeof(InputIconSetGamepadSO), true);
                configurator.ps5IconSet = (InputIconSetGamepadSO)EditorGUILayout.ObjectField("PS5 Icons", configurator.ps5IconSet, typeof(InputIconSetGamepadSO), true);
                configurator.switchIconSet = (InputIconSetGamepadSO)EditorGUILayout.ObjectField("Switch Icons", configurator.switchIconSet, typeof(InputIconSetGamepadSO), true);
                configurator.xBoxIconSet = (InputIconSetGamepadSO)EditorGUILayout.ObjectField("XBox Icons", configurator.xBoxIconSet, typeof(InputIconSetGamepadSO), true);

                configurator.fallbackGamepadIconSet = (InputIconSetGamepadSO)EditorGUILayout.ObjectField("Fallback Icons", configurator.fallbackGamepadIconSet, typeof(InputIconSetGamepadSO), true);
                configurator.overwriteIconSet = (InputIconSetGamepadSO)EditorGUILayout.ObjectField("Gamepads Overwrite Icons", configurator.overwriteIconSet, typeof(InputIconSetGamepadSO), true);

            }

           
            //EditorGUI.BeginDisabledGroup(true);
            /*for (int i = 0; i < iconSetSOs.Count; i++)
            {
                if(iconSetSOs[i]!=null)
                    iconSetSOs[i] = (InputIconSetBasicSO)EditorGUILayout.ObjectField(iconSetSOs[i].deviceDisplayName, iconSetSOs[i], typeof(InputIconSetBasicSO), true);
            }*/
            //EditorGUI.EndDisabledGroup();
        }

        public static void DrawUILine(Color color, int thickness = 2, int padding = 5)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            //r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

        public static void DrawUILineVertical(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(padding+thickness), GUILayout.ExpandHeight(true));
            r.width = thickness;
            r.x += padding / 2;
            r.y -= 2;
            r.height += 3;
            EditorGUI.DrawRect(r, color);
        }

        public void DrawCurrentInputActionAssetsList()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedInputActionAssets);

            if(EditorGUI.EndChangeCheck())
            {
                InputIconsManagerSO.Instance.CreateInputStyleData(false);
            }
        }

        void DrawCustomContextList()
        {
            try
            {
                keyboardSchemeNames = new ReorderableList(serializedManager, serializedManager.FindProperty("controlSchemeNames_Keyboard"), true, true, true, true);

                keyboardSchemeNames.drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(new Rect(rect.x + 5, rect.y, 200, EditorGUIUtility.singleLineHeight), "Keyboard control scheme names");
                };


                keyboardSchemeNames.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {

                    var element = keyboardSchemeNames.serializedProperty.GetArrayElementAtIndex(index);

                    rect.y += 2;

                    EditorGUI.PropertyField(new Rect(rect.x + 5, rect.y, rect.width - 10, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
                };
            }
            catch (System.Exception)
            {
                //SerializedObjectNotCreatableException might appear on older Unity Versions. Not critical
            }

            try
            {
                gamepadSchemeNames = new ReorderableList(serializedManager, serializedManager.FindProperty("controlSchemeNames_Gamepad"), true, true, true, true);

                gamepadSchemeNames.drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(new Rect(rect.x + 5, rect.y, 200, EditorGUIUtility.singleLineHeight), "Gamepad control scheme names");
                };


                gamepadSchemeNames.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {

                    var element = gamepadSchemeNames.serializedProperty.GetArrayElementAtIndex(index);

                    rect.y += 2;

                    EditorGUI.PropertyField(new Rect(rect.x + 5, rect.y, rect.width - 10, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
                };
            }
            catch (System.Exception)
            {
                //SerializedObjectNotCreatableException might appear on older Unity Versions. Not critical
            }
        }

    }
}