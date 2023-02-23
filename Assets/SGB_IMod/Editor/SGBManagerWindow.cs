using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SGB_IMod
{
    public class SGBManagerWindow : EditorWindow
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
            SGBManagerWindow window = EditorWindow.GetWindow<SGBManagerWindow>("SGB Manager");
            window.Show();
        }

        private void OnGUI()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                DrawSGBGame("Ba");
                DrawSGBGame("Be");

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    DrawImportButton();
                }
            }
        }

        private void DrawSGBGame(string name)
        {
            using (new EditorGUILayout.VerticalScope(StyleSGBGameBox))
            {
                EditorGUILayout.LabelField(name, EditorStyles.largeLabel);

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
