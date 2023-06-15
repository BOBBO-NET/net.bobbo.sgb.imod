using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BobboNet.Editor.SGB.IMod
{
    public class SGBEditorManagerWindow : EditorWindow
    {
        private GUIStyle StyleSGBGameBox
        {
            get
            {
                return EditorStyles.helpBox;
            }
        }


        [MenuItem("SGB_IMod/SGB Manager")]
        private static void Init()
        {
            // Get existing open window (Or make a new one if there is none)
            SGBEditorManagerWindow window = EditorWindow.GetWindow<SGBEditorManagerWindow>("SGB Manager");
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Imported SGB Games", EditorStyles.whiteLargeLabel);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                foreach (var game in SGBEditorManager.GetSingleton().importedGames.ToArray())
                {
                    DrawSGBGame(game);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    DrawImportButton();
                }
            }

            SGBEditorManager.GetSingleton();
        }

        private void DrawSGBGame(SGBEditorManager.ImportedGame game)
        {
            using (new EditorGUILayout.VerticalScope(StyleSGBGameBox))
            {
                EditorGUILayout.LabelField(game.projectName, EditorStyles.largeLabel);
                EditorGUILayout.LabelField($"Maps: '{game.pathImportedMaps}'", EditorStyles.wordWrappedMiniLabel);
                EditorGUILayout.LabelField($"Resources: '{game.pathImportedResources}'", EditorStyles.wordWrappedMiniLabel);
                EditorGUILayout.LabelField($"Scene Count: {game.scenes.Count}");

                using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
                {
                    if (GUILayout.Button("Delete", EditorStyles.toolbarButton, GUILayout.MaxWidth(80)))
                    {
                        SGBEditorManager.GetSingleton().DeleteImportedSGBGame(game);
                    }
                    if (GUILayout.Button("Re-Import", EditorStyles.toolbarButton))
                    {
                        SGBEditorManager.GetSingleton().ReimportSGBGame(game);
                    }
                }
            }
        }

        private void DrawImportButton()
        {
            if (GUILayout.Button("Import", GUILayout.MaxWidth(100)))
            {
                InstallWizardWindow.Init();
            }
        }
    }
}
