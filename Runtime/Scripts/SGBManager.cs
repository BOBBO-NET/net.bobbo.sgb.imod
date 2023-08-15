using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BobboNet.SGB.IMod
{
    public static class SGBManager
    {
        //
        //  Settings
        //

        // The name of the scene to load into when doing any SGB-related tasks.
        private static readonly string sceneNameWorkspace = "IModWorkspace";

        // The resource path of the dev overlay prefab
        private static readonly string resourcePathPrefabDevOverlay = "IMod_DevOverlay";


        //
        //  Variables
        //

        private static readonly GameObject prefabDevOverlay = null;
        private static GameObject instanceDevOverlay = null;


        static SGBManager()
        {
            // Load the dev overlay prefab
            prefabDevOverlay = Resources.Load(resourcePathPrefabDevOverlay) as GameObject;
        }




        // Load into a Smile Game Builder game, given the location of it's assets
        public static async Task LoadSmileGameAsync(string smileGameName, int saveFile = -1)
        {
            // Check to see if there's a smile game by this name
            if (!DoesSGBSubpathExist(smileGameName))
            {
                throw new System.Exception("No Smile Game Builder game with this subpath could be found!");
            }

            // Set the game subpath we want to load
            UnityEntry.gameSubpathName = smileGameName;

            // Override natural init if we are given a save to directly load
            UnityEntry.overrideInit = saveFile != -1;

            // Start async loading the scene
            await EnterWorkspaceSceneAsync();

            // When the scene is done loading, create required objects
            CreateSGBRequiredGameObjects(out UnityEntry createdEntryPoint);
            CreateCustomRequiredGameObjects();

            // If we have a save file to load into, jump directly into the game.
            if (saveFile != -1)
            {
                // Custom initialize SGB, ahead of Start() call, so that we can...
                InitializeSGBEntry(createdEntryPoint);

                // ...reset SGB's state, load the first save file, and boot DIRECTLY into the map scene.
                UnityEntry.game.DoReset(true, false);
                UnityEntry.game.DoLoad(0);
                UnityEntry.game.ChangeScene(Yukar.Engine.GameMain.Scenes.MAP);
            }
        }

        // Unload from current Smile Builder Game, and load into a specific scene
        public static async Task UnloadSmileGameAsync()
        {
            // Start Async re-loading into the empty workspace scene
            await EnterWorkspaceSceneAsync();

            // ...and when the scene is done loading, fix up our required objects.
            DestroyCustomRequiredGameObjects();
        }



        // Asynchronously load into the workspace scene
        private static async Task EnterWorkspaceSceneAsync() => await LoadSceneTrueAsync(sceneNameWorkspace);

        private static async Task LoadSceneTrueAsync(string sceneName)
        {
            // Start loading the workspace scene
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);
            loadOperation.allowSceneActivation = true;

            // Loop forever until the scene is ready to go
            while (!loadOperation.isDone)
            {
                await Task.Yield();
            }
        }

        // Construct the GameObjects that SGB needs to initialize
        private static void CreateSGBRequiredGameObjects(out UnityEntry entry)
        {
            CreateSGBRequiredGameObjectsHelper_MainCamera();
            CreateSGBRequiredGameObjectsHelper_ParentAndDummy("MapScene");
            CreateSGBRequiredGameObjectsHelper_ParentAndDummy("BattleScene");
            CreateSGBRequiredGameObjectsHelper_ParentAndDummy("Sound");
            CreateSGBRequiredGameObjectsHelper_ParentAndDummy("Template");
            CreateSGBRequiredGameObjectsHelper_UnityAds();
            CreateSGBRequiredGameObjectsHelper_UnityEntry(out entry);
        }

        // Helper class for CreateSGBRequiredGameObjects. Creates the Main Camera
        private static void CreateSGBRequiredGameObjectsHelper_MainCamera()
        {
            // Create the Main Camera
            GameObject mainCameraObject = new GameObject(
                "Main Camera",
                typeof(Camera),
                typeof(FlareLayer),
                typeof(AudioListener)
            );

            // Set the Main Camera's Tag and Layer
            mainCameraObject.tag = "MainCamera";
            mainCameraObject.layer = 5;     // (UI Layer)
        }

        // Helper class for CreateSGBRequiredGameObjects. Creates the Entry object
        private static void CreateSGBRequiredGameObjectsHelper_UnityEntry(out UnityEntry entry)
        {
            // Create the Entry Object
            GameObject unityEntryObject = new GameObject(
                "Entry",
                typeof(UnityEntry)
            );

            entry = unityEntryObject.GetComponent<UnityEntry>();
        }

        // Helper class for CreateSGBRequiredGameObjects. Creates an empty GameObject with a dummy child object,
        // given the parent object's name.
        private static void CreateSGBRequiredGameObjectsHelper_ParentAndDummy(string parentObjectName)
        {
            // Create the parent object, and a dummy child object (why is this needed? idk)
            GameObject parentObject = new GameObject(parentObjectName);
            GameObject childDummyObject = new GameObject("Dummy");
            childDummyObject.transform.SetParent(parentObject.transform);
        }

        // Helper class for CreateSGBRequiredGameObjects. Creates the Unity Ads Manager
        private static void CreateSGBRequiredGameObjectsHelper_UnityAds()
        {
            // Create the Unity Ads object
            GameObject unityAdsObject = new GameObject(
                "UnityAds",
                typeof(UnityAdsManager)
            );
        }

        private static void CreateCustomRequiredGameObjects()
        {
            // Create the dev UI
            instanceDevOverlay = GameObject.Instantiate(prefabDevOverlay);
        }




        private static void DestroyCustomRequiredGameObjects()
        {
            // Destroy the dev UI
            GameObject.Destroy(instanceDevOverlay);
        }




        private static bool DoesSGBSubpathExist(string gameSubpath)
        {
            string potentialAssetPath = Path.Combine("SGB", gameSubpath, "assets");

            // Try and load the file
            var assetFile = Resources.Load(potentialAssetPath);

            // If it has been loaded correctly, it shouldn't be null.
            return assetFile != null;
        }

        private static void InitializeSGBEntry(UnityEntry entry)
        {
            UnityEntry.self = entry;

            SharpKmyGfx.Render.InitializeRender();
            Yukar.Common.UnityUtil.Initialize();
#if !UNITY_EDITOR
            Debug.unityLogger.logEnabled = false;
#endif
#if UNITY_IOS || UNITY_ANDROID
            UnityResolution.Start();
#endif //UNITY_IOS || UNITY_ANDROID

            Yukar.Common.FileUtil.language = Application.systemLanguage.ToString();

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            UnityEntry.InitializeDir();

            Yukar.Common.FileUtil.initialize();

            Yukar.Common.Catalog.createDlcList(false);
            Yukar.Common.Catalog catalog = new Yukar.Common.Catalog();
            catalog.load(false);

            UnityEntry.game = new Yukar.Engine.GameMain();
            UnityEntry.game.initialize();

            UnityAdsManager.Initialize(UnityEntry.game);

            entry.didInit = true;
            entry.didInitWithGame = UnityEntry.game;
            entry.didInitWithSelf = entry;
        }
    }
}