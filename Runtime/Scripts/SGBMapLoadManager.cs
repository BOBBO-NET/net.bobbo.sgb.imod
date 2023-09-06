using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
            /// A reference to the actual map data getting loaded. May be null.
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
    }
}