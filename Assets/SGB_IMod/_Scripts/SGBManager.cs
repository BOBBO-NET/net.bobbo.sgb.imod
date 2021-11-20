using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SGB_IMod
{
    public static class SGBManager
    {
        //
        //  Settings
        //

        // The name of the scene to load into when doing any SGB-related tasks.
        private static readonly string sceneNameWorkspace = "IModWorkspace";



        // Load into a Smile Game Builder game, given the location of it's assets
        public static void LoadSmileGame(string smileResourceLocation)
        {
            EnterWorkspaceScene();
        }



        // Synchronously load into the workspace scene
        private static void EnterWorkspaceScene()
        {
            SceneManager.LoadScene(sceneNameWorkspace);
        }
    }
}