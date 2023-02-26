using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System;

public class SGBEditorManager : ScriptableObject
{
    [System.Serializable]
    public class ImportedGame
    {
        public string projectName;

        public string pathSourceResources;
        public string pathSourceMaps;
        public string pathImportedResources;
        public string pathImportedMaps;

        public List<string> scenes;
    }

    public List<ImportedGame> importedGames;

    public static SGBEditorManager GetSingleton()
    {
        string assetPath = Path.Combine("Assets", "SGB_IMod", "Editor", "SGB_IMod_Manager.asset");
        SGBEditorManager result = AssetDatabase.LoadAssetAtPath<SGBEditorManager>(assetPath);

        if (result == null)
        {
            AssetDatabase.CreateAsset(SGBEditorManager.CreateInstance<SGBEditorManager>(), assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            result = AssetDatabase.LoadAssetAtPath<SGBEditorManager>(assetPath);
        }

        return result;
    }

    //
    //  Import Command
    //

    public void ImportSGBUnityExport(string projectName, string pathToSGBUnityExport, string pathToResourceDestination, string pathToMapDestination)
    {
        ImportedGame newGame = new ImportedGame();
        newGame.projectName = projectName;
        newGame.pathSourceMaps = Path.Combine(pathToSGBUnityExport, "Assets", "map");
        newGame.pathSourceResources = Path.Combine(pathToSGBUnityExport, "Assets", "Resources");
        newGame.pathImportedMaps = pathToMapDestination;
        newGame.pathImportedResources = pathToResourceDestination;

        CopySGBFilesIntoProject(newGame);       // Copy all of the correct files over
        StoreSGBScenes(newGame);                // Scan through the map files & keep track of the new scenes
        RemoveSGBScenesFromSceneList(newGame);  // Remove any scenes that already exist from this SGB project in the scene list (to prevent dupes)
        AddSGBScenesToSceneList(newGame);       // Add new scenes from this SGB project

        // Add this game to the manager object, so we can display it in the editor.
        importedGames.Add(newGame);
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

    //
    //  Delete Game
    //

    public void DeleteImportedSGBGame(ImportedGame game)
    {
        RemoveSGBScenesFromSceneList(game); // Remove all scenes relating to this SGB project from the scene list
        RemoveSGBFilesFromProject(game);    // Remove all imported files relating to this SGB project & refresh the asset database

        // Remove this game from the manager object
        importedGames.Remove(game);
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

    public void ReimportSGBGame(ImportedGame game)
    {
        CopySGBFilesIntoProject(game);      // Copy all of the correct files over
        StoreSGBScenes(game);               // Scan through the map files & keep track of the new scenes
        RemoveSGBScenesFromSceneList(game); // Remove any scenes that already exist from this SGB project in the scene list (to prevent dupes)
        AddSGBScenesToSceneList(game);      // Add new scenes from this SGB project

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }


    //
    //  Utilities
    //

    private static void CopySGBFilesIntoProject(ImportedGame game)
    {
        // Copy the files to the correct location
        try
        {
            if (Directory.Exists(game.pathImportedResources))
            {
                EditorUtility.DisplayProgressBar("SGB_IMOD Import [Resources]", "Removing old resources...", 0);
                Directory.Delete(game.pathImportedResources, true);
            }
            CopyDirectoryOperation(game.pathSourceResources, game.pathImportedResources, "SGB_IMOD Import [Resources]", "Copying resources");

            if (Directory.Exists(game.pathImportedMaps))
            {
                EditorUtility.DisplayProgressBar("SGB_IMOD Import [Maps]", "Removing old maps...", 0);
                Directory.Delete(game.pathImportedMaps, true);
            }
            CopyDirectoryOperation(game.pathSourceMaps, game.pathImportedMaps, "SGB_IMOD Import [Maps]", "Copying maps");

            // Refresh the asset database now that we have new files
            AssetDatabase.Refresh();

            EditorUtility.ClearProgressBar();
        }
        catch (System.Exception exception)
        {
            EditorUtility.DisplayDialog("SGB_IMOD Import [ERROR]", $"Failed to copy files: {exception}", "aw...");
            EditorUtility.DisplayProgressBar("SGB_IMOD Import [ERROR]", "Cleaning up...", 0);

            try
            {
                Directory.Delete(game.pathImportedResources, true);
            }
            catch (System.Exception) { /* swallow any errors */ }
            try
            {
                Directory.Delete(game.pathImportedMaps, true);
            }
            catch (System.Exception) { /* swallow any errors */ }

            EditorUtility.ClearProgressBar();
            throw exception;
        }
    }

    private static void RemoveSGBFilesFromProject(ImportedGame game)
    {
        try
        {
            if (Directory.Exists(game.pathImportedResources))
            {
                EditorUtility.DisplayProgressBar("SGB_IMOD [Resources]", "Removing resources...", 0);
                Directory.Delete(game.pathImportedResources, true);
            }

            if (Directory.Exists(game.pathImportedMaps))
            {
                EditorUtility.DisplayProgressBar("SGB_IMOD [Maps]", "Removing maps...", 0);
                Directory.Delete(game.pathImportedMaps, true);
            }

            // Refresh the asset database now that we have less files
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
        }
        catch (System.Exception exception)
        {
            EditorUtility.DisplayDialog("SGB_IMOD [ERROR]", $"Failed to remove files: {exception}", "aw...");
            EditorUtility.ClearProgressBar();
            throw exception;
        }
    }

    private static void StoreSGBScenes(ImportedGame game)
    {
        List<string> scenePaths = new List<string>();
        foreach (string mapPath in Directory.GetFiles(game.pathImportedMaps, "*.unity"))
        {
            scenePaths.Add("Assets" + mapPath.Substring(Application.dataPath.Length).Replace('\\', '/'));
        }
        game.scenes = scenePaths;
    }

    private static void RemoveSGBScenesFromSceneList(ImportedGame game)
    {
        EditorUtility.DisplayProgressBar("SGB_IMOD", $"Removing {game.scenes.Count} scenes...", 1);

        // Go through the editor scene list and remove all scenes relating to this SGB game
        EditorBuildSettings.scenes = EditorBuildSettings.scenes.Where(scene => !game.scenes.Contains(scene.path)).ToArray();
        EditorBuildSettings.scenes = EditorBuildSettings.scenes.Where(scene => !String.IsNullOrWhiteSpace(scene.path)).ToArray();
        EditorUtility.ClearProgressBar();
    }

    private static void AddSGBScenesToSceneList(ImportedGame game)
    {
        EditorUtility.DisplayProgressBar("SGB_IMOD", $"Adding {game.scenes.Count} scenes...", 1);

        // Add the scenes to the list
        List<EditorBuildSettingsScene> editorSceneList = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        foreach (string scenePath in game.scenes)
        {
            editorSceneList.Add(new EditorBuildSettingsScene(scenePath, true));
        }

        EditorUtility.ClearProgressBar();
        EditorBuildSettings.scenes = editorSceneList.ToArray();
    }

    private static void CopyDirectoryOperation(string sourceDirectory, string destinationDirectory, string progressHeader, string progressDescription, bool clearProgressBar = true)
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