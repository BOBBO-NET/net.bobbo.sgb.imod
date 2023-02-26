using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;

namespace SGB_IMod
{
    public class InstallWizardWindow : EditorWindow
    {
        private enum WizardState
        {
            START,
            NAME_GAME,
            CONFIRM,
            IMPORTING,
            COMPLETE
        }

        private enum PathStatus
        {
            OK,
            DOES_NOT_EXIST,
            MISSING_RESOURCES_MAP_FOLDER,
            MISSING_MAP_FOLDER
        }

        private WizardState state = WizardState.START;

        private string pathSGBUnityExport;
        private PathStatus pathStatusSGBUnityExport;
        private string projectName;
        private bool isProjectNameValid;
        private bool wouldProjectNameOverwrite;
        private bool confirmCopying;


        // [MenuItem("SGB_IMod/Install SGB Project")]
        public static void Init()
        {
            // Get existing open window (Or make a new one if there is none)
            InstallWizardWindow window = EditorWindow.GetWindow<InstallWizardWindow>("SGB Import Wizard");
            window.Show();
        }

        private void Update()
        {
            if (state == WizardState.CONFIRM) Repaint();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField($"STAGE: {state}", EditorStyles.boldLabel);
            EditorGUILayout.Separator();

            switch (state)
            {
                case WizardState.START:
                    DrawStateStart();
                    break;
                case WizardState.NAME_GAME:
                    DrawStateNameGame();
                    break;
                case WizardState.CONFIRM:
                    DrawStateConfirm();
                    break;
                case WizardState.IMPORTING:
                    DrawStateImporting();
                    break;
                case WizardState.COMPLETE:
                    DrawStateComplete();
                    break;
            }

            // Add spacing so that the below elements are at the bottom of the window
            GUILayout.FlexibleSpace();

            int stateTransitionDirection = 0;
            if (state != WizardState.COMPLETE)
            {
                // Draw the back & next buttons
                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUI.DisabledScope(!CanStateGoBackward()))
                    {
                        if (GUILayout.Button("Back", EditorStyles.miniButtonLeft))
                        {
                            stateTransitionDirection = -1;
                        }
                    }
                    using (new EditorGUI.DisabledScope(!CanStateGoForward()))
                    {
                        if (GUILayout.Button("Next", EditorStyles.miniButtonRight))
                        {
                            stateTransitionDirection = 1;
                        }
                    }
                }
            }
            // If the user pressed back or next, try to change the state
            if (stateTransitionDirection != 0)
            {
                ChangeState(state + stateTransitionDirection);
            }
        }

        private void DrawStateStart()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Welcome to SGB_IMod, a tool that makes it possible to embed Smile Game Builder Unity-Builds into other Unity projects. ");
            sb.Append("This window is a wizard to assist with importing a Unity project exported by SGB into the currently open Unity Project.\n\n");
            sb.Append("Use the buttons at the bottom of the window to navigate this sequence.");


            GUILayout.Box(sb.ToString(), EditorStyles.wordWrappedLabel);
        }

        private void DrawStateNameGame()
        {
            EditorGUILayout.LabelField("1. SGB EXPORT PATH", EditorStyles.whiteLargeLabel);
            GUILayout.Box("Enter the path to the SGB Unity Export that you want to IMPORT", EditorStyles.wordWrappedLabel);
            string newPath = EditorGUILayout.TextField(pathSGBUnityExport);
            if (newPath != pathSGBUnityExport)
            {
                pathSGBUnityExport = newPath;
                UpdateSGBPathStatus();
            }

            GUILayout.Box($"STATUS: {pathStatusSGBUnityExport}", EditorStyles.helpBox);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Refresh", GUILayout.MaxWidth(100)))
                {
                    GUI.FocusControl(null);
                    UpdateSGBPathStatus();
                }

                if (GUILayout.Button("Browse"))
                {
                    GUI.FocusControl(null);
                    pathSGBUnityExport = EditorUtility.OpenFolderPanel("Select a SGB Unity Export", pathSGBUnityExport, "");
                    UpdateSGBPathStatus();
                }
            }

            EditorGUILayout.Space();

            using (new EditorGUI.DisabledScope(pathStatusSGBUnityExport != PathStatus.OK))
            {
                EditorGUILayout.LabelField("2. NAME PROJECT", EditorStyles.whiteLargeLabel);
                GUILayout.Box("Choose a name for this SGB project. This will be used to name the folders where the project will be stored. No duplicates allowed!", EditorStyles.wordWrappedLabel);
                string newProjectName = EditorGUILayout.TextField(projectName);
                if (newProjectName != projectName)
                {
                    projectName = newProjectName;
                    UpdateProjectNameValidity();
                }

                GUILayout.Box($"STATUS: {(isProjectNameValid ? "OK" : "INVALID NAME")}", EditorStyles.helpBox);

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Refresh", GUILayout.MaxWidth(100)))
                    {
                        GUI.FocusControl(null);
                        UpdateProjectNameValidity();
                    }
                }
            }
        }

        private void DrawStateConfirm()
        {
            EditorGUILayout.LabelField("REVIEW", EditorStyles.whiteLargeLabel);
            GUILayout.Box("This will install the SGB project into the current Unity project, at the following paths:", EditorStyles.wordWrappedLabel);
            GUILayout.Box($"SGB Resources: \t'{RemoveDataPath(ConstructProjectResourcesPathFromName(projectName))}'", EditorStyles.helpBox);
            GUILayout.Box($"SGB Maps: \t'{RemoveDataPath(ConstructProjectMapsPathFromName(projectName))}'", EditorStyles.helpBox);

            EditorGUILayout.Space();

            if (wouldProjectNameOverwrite)
            {
                EditorGUILayout.LabelField("WARNING", new GUIStyle(EditorStyles.whiteLargeLabel)
                {
                    normal = {
                        textColor = Color.HSVToRGB((float)EditorApplication.timeSinceStartup * 0.25f % 1, 1, 1)
                    }
                });
                GUILayout.Box("These paths already exist. This means that they will be DELETED before importing the project.", EditorStyles.helpBox);
            }

            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("CONFIRM:", EditorStyles.wordWrappedLabel);
                confirmCopying = EditorGUILayout.Toggle(confirmCopying);
            }
        }

        private void DrawStateImporting()
        {
            string newResourcePath = ConstructProjectResourcesPathFromName(projectName);
            string newMapPath = ConstructProjectMapsPathFromName(projectName);

            try
            {
                SGBEditorManager.GetSingleton().ImportSGBUnityExport(projectName, pathSGBUnityExport, newResourcePath, newMapPath);
                EditorUtility.DisplayDialog("SGB_IMOD", $"Successfully imported SGB project '{projectName}'!", "yum YUM");
                ChangeState(WizardState.COMPLETE);
            }
            catch (System.Exception exception)
            {
                EditorUtility.DisplayDialog("SGB_IMOD", $"Failed to import SGB project '{projectName}': {exception}", "awww NUTS...");
                ChangeState(WizardState.START);
            }
        }

        private void DrawStateComplete()
        {
            this.Close();
        }

        private bool CanStateGoForward()
        {
            if (state == WizardState.COMPLETE) return false;

            if (state == WizardState.NAME_GAME)
            {
                if (pathStatusSGBUnityExport != PathStatus.OK) return false;
                if (!isProjectNameValid) return false;

                confirmCopying = false;
            }

            if (state == WizardState.CONFIRM)
            {
                if (!confirmCopying) return false;
            }

            return true;
        }

        private bool CanStateGoBackward()
        {
            if (state == WizardState.START) return false;

            return true;
        }

        private void ChangeState(WizardState newState)
        {
            state = newState;
            UpdateSGBPathStatus();
            UpdateProjectNameValidity();
        }

        private PathStatus CheckSGBPathStatus()
        {
            if (!Directory.Exists(pathSGBUnityExport)) return PathStatus.DOES_NOT_EXIST;
            if (!Directory.Exists(Path.Combine(pathSGBUnityExport, "Assets", "map"))) return PathStatus.MISSING_MAP_FOLDER;
            if (!Directory.Exists(Path.Combine(pathSGBUnityExport, "Assets", "Resources", "map"))) return PathStatus.MISSING_RESOURCES_MAP_FOLDER;

            return PathStatus.OK;
        }

        private void UpdateSGBPathStatus()
        {
            pathStatusSGBUnityExport = CheckSGBPathStatus();
        }

        private void UpdateProjectNameValidity()
        {
            isProjectNameValid = CheckProjectNameValidity();
            wouldProjectNameOverwrite = CheckProjectNameWouldOverwrite();
        }

        private bool CheckProjectNameValidity()
        {
            if (projectName == null) return false;
            if (projectName.Length < 1) return false;
            return true;
        }

        private bool CheckProjectNameWouldOverwrite()
        {
            if (Directory.Exists(ConstructProjectResourcesPathFromName(projectName))) return true;
            if (Directory.Exists(ConstructProjectMapsPathFromName(projectName))) return true;
            return false;
        }

        private string ConvertToPathFriendly(string path)
        {
            if (path == null) return "";

            foreach (char c in Path.GetInvalidPathChars())
            {
                path.Replace(c, '_');
            }

            return path.Replace(' ', '_');
        }

        private string ConstructProjectResourcesPathFromName(string projectName)
        {
            return Path.Combine(Application.dataPath, "SGB", "Resources", "SGB", ConvertToPathFriendly(projectName));
        }

        private string ConstructProjectMapsPathFromName(string projectName)
        {
            return Path.Combine(Application.dataPath, "SGB", "Maps", ConvertToPathFriendly(projectName));
        }

        private string RemoveDataPath(string path)
        {
            int dataPathStart = path.IndexOf(Application.dataPath);
            if (dataPathStart < 0) return path;

            return path.Substring(dataPathStart + Application.dataPath.Length);
        }

        private void CopyDirectoryOperation(string sourceDirectory, string destinationDirectory, string progressHeader, string progressDescription, bool clearProgressBar = true)
        {
            DirectoryInfo sourceInfo = new DirectoryInfo(sourceDirectory);
            EditorUtility.DisplayProgressBar(progressHeader, $"{progressDescription} '{sourceInfo.FullName}'...", 1);
            if (!sourceInfo.Exists) throw new DirectoryNotFoundException($"Source directory not found: {sourceInfo.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] sourceSubInfos = sourceInfo.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDirectory);

            // Get the files in the source directory and copy to the destination directory
            FileInfo[] allFiles = sourceInfo.GetFiles();
            for (int i = 0; i < allFiles.Length; i++)
            {
                string targetFilePath = Path.Combine(destinationDirectory, allFiles[i].Name);
                allFiles[i].CopyTo(targetFilePath);
            }

            // Recursively call this method
            foreach (DirectoryInfo subDir in sourceSubInfos)
            {
                string newDestinationDir = Path.Combine(destinationDirectory, subDir.Name);
                CopyDirectoryOperation(subDir.FullName, newDestinationDir, progressHeader, progressDescription, false);
            }

            if (clearProgressBar) EditorUtility.ClearProgressBar();
        }
    }
}
