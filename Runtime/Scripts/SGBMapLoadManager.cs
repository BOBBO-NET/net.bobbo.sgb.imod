using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Yukar.Engine;

namespace BobboNet.SGB.IMod
{
    /// <summary>
    /// A manager that exposes functionality relating to SGB map loading.
    /// </summary>
    public static class SGBMapLoadManager
    {
        //
        //  Types
        //

        public struct PreLoadApprovalArgs
        {
            /// <summary>
            /// The GUID of the SGB map being loaded.
            /// </summary>
            public Guid Guid { get; set; }

            /// <summary>
            /// A reference to the actual map data getting loaded. 
            /// </summary>
            public Yukar.Common.Rom.Map Map { get; set; }
        }

        public class PreLoadEvent : ApproveEvent<PreLoadApprovalArgs> { }

        //
        //  Properties
        //

        /// <summary>
        /// Invoked just before SGB switches over to a new game map. Methods that subscribe to this can
        /// tell the SGBMapLoadManager that a given map is approved to load, or not approved to load.
        /// </summary>
        public static PreLoadEvent OnPreLoad { get; private set; } = new PreLoadEvent();

        //
        //  Constructor
        //

        static SGBMapLoadManager()
        {
            OnPreLoad.AddApprovalSource(delegate (PreLoadApprovalArgs args)
            {
                Debug.Log($"Loading SGB Map {args.Guid}{(args.Map != null ? $" - '{args.Map.name}'" : "")}");
                return ApproveEventResult.Approve;
            });
        }

        //
        //  Methods
        //

        /// <summary>
        /// Load directly into an SGB map
        /// </summary>
        /// <param name="loadMapArgs">The details about what map to load into, and where to put the player.</param>
        public static void LoadMap(LoadSGBMapArgs loadMapArgs = null)
        {
            // If we don't have any specific map to load, just load the default map and EXIT EARLY
            if (loadMapArgs == null)
            {
                UnityEntry.game.ChangeScene(GameMain.Scenes.MAP);
                return;
            }

            // OTHERWISE... we DO have a specific map to load, so...
            // Load the map that our args refer to. If we can't find it, issue a warning.
            var mapToLoad = loadMapArgs.GetMap(UnityEntry.game.catalog);
            if (mapToLoad == null)
            {
                Debug.LogWarning($"Failed to find an SGB map with the name '{mapToLoad}'..! Ignoring...");
                UnityEntry.game.ChangeScene(GameMain.Scenes.MAP);
                return;
            }

            // Create a delegate that says to IGNORE loading any maps that are NOT the desired map
            PreLoadEvent.EventDelegate temporaryPreloadEvent = delegate (PreLoadApprovalArgs args)
            {
                return (args.Guid == mapToLoad.guId) ? ApproveEventResult.Approve : ApproveEventResult.Error;
            };

            // 1. Setup the preload event that will STOP SGB from loading any maps EXCEPT the one we want
            OnPreLoad.AddApprovalSource(temporaryPreloadEvent);

            // 2. Tell SGB to internally get ready for the map scene
            UnityEntry.game.ChangeScene(GameMain.Scenes.MAP);

            // 3. Directly load to the desired map
            MapScene.ChangeMapParams changeMapArgs = loadMapArgs.GenerateChangeMapParams(mapToLoad);
            UnityEntry.game.mapScene.ChangeMap(changeMapArgs);

            // 4. Remove the preload event from earlier, so that we can load any maps in the future.
            OnPreLoad.RemoveApprovalSource(temporaryPreloadEvent);
        }
    }
}