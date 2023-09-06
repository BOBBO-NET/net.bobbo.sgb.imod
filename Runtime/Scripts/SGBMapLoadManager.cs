using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace BobboNet.SGB.IMod
{
    /// <summary>
    /// A manager that exposes functionality relating to SGB map loading.
    /// </summary>
    public static class SGBMapLoadManager
    {
        //
        //  Classes
        //

        /// <summary>
        /// The arguments passed into a pre-load event.
        /// </summary>
        public class PreLoadEventArgs : EventArgs
        {
            /// <summary>
            /// The GUID of the SGB map being loaded.
            /// </summary>
            public Guid Guid { get; private set; }

            /// <summary>
            /// A reference to the actual map data getting loaded. May be null.
            /// </summary>
            public Yukar.Common.Rom.Map Map { get; private set; }

            public PreLoadEventArgs(Guid guid, Yukar.Common.Rom.Map map)
            {
                Guid = guid;
                Map = map;
            }
        }

        /// <summary>
        /// The result of a pre-load operation. This type allows callbacks to approve or cancel
        /// a map loading operation before it begins.
        /// </summary>
        public enum PreLoadResult
        {
            Error = 0,
            ApproveLoad,
            CancelLoad
        }

        /// <summary>
        /// The structure of a pre-load operation. A method using this can inspect the map being loaded
        /// and decide if it's okay to load it at the given moment.
        /// </summary>
        /// <param name="args">Details on the map that is about to be loaded.</param>
        /// <returns>A result indicating if we should continue loading this map or not.</returns>
        public delegate PreLoadResult PreLoadEventHandler(PreLoadEventArgs args);

        //
        //  Events
        //

        /// <summary>
        /// Invoked just before SGB switches over to a new game map. Methods that subscribe to this can
        /// tell the SGBMapLoadManager that a given map is approved to load, or not approved to load.
        /// </summary>
        public static event PreLoadEventHandler OnPreLoad;

        //
        //  Constructor
        //

        static SGBMapLoadManager()
        {

        }

    }
}