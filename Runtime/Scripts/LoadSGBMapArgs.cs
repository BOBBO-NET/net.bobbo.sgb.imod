using UnityEngine;
using Yukar.Common;

namespace BobboNet.SGB.IMod
{
    /// <summary>
    /// Data to use when entering into an SGB map.
    /// </summary>
    public class LoadSGBMapArgs
    {
        /// The name of the SGB map to load.
        public string MapName { get; set; }

        // Where to place the player once the map has loaded.
        public Vector2Int StartPosition { get; set; }

        // Which way to orient the player once the map has loaded.
        public int StartDirection { get; set; } = -1;

        // Where to place the player vertically once the map has loaded.
        public float StartHeight { get; set; } = -1.0f;

        //
        //  Public Methods
        //

        /// <summary>
        /// Find the SGB map for these entry parameters, given a catalog of SGB data.
        /// </summary>
        /// <param name="catalog">The SGB catalog to search through when looking for this map.</param>
        /// <returns>The SGB Map RomItem that this class refers to, null if not found.</returns>
        public Yukar.Common.Rom.Map GetMap(Catalog catalog)
        {
            var smileMaps = catalog.getFilteredItemList(typeof(Yukar.Common.Rom.Map));

            foreach (var map in smileMaps)
            {
                if (map.name == MapName) return (Yukar.Common.Rom.Map)map;
            }

            return null;
        }

        /// <summary>
        /// Generate SGB's internal change map parameters using these entry parameters and the
        /// map to load itself.
        /// </summary>
        /// <param name="map">The SGB map to load.</param>
        /// <returns>SGB's internal change map parameters.</returns>
        internal Yukar.Engine.MapScene.ChangeMapParams GenerateChangeMapParams(Yukar.Common.Rom.Map map)
        {
            var changeMapParameters = Yukar.Engine.MapScene.ChangeMapParams.defaultParams;
            changeMapParameters.guid = map.guId;
            changeMapParameters.x = StartPosition.x;
            changeMapParameters.y = StartPosition.y;
            changeMapParameters.height = StartHeight;
            changeMapParameters.dir = StartDirection;

            // The following parameters are left alone, but could be implemented in the future. 
            //
            // changeMapParameters.createHero = true;
            // changeMapParameters.eventStates = data.start.events;
            // changeMapParameters.camera = data.start.camera;
            // changeMapParameters.spriteStates = data.start.sprites;
            // changeMapParameters.playerLock = data.start.plLock;
            // changeMapParameters.cameraLockByEvent = data.start.camLockedByEvent;
            // changeMapParameters.cameraLock = data.start.camLock;
            // changeMapParameters.cameraModeLock = data.start.camModeLock;
            // changeMapParameters.bgmStatus = data.start.currentBgm;
            // changeMapParameters.bgsStatus = data.start.currentBgs;

            return changeMapParameters;
        }
    }
}