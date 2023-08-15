using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Yukar.Engine;

namespace BobboNet.SGB.IMod
{
    public static class SGBSaveManager
    {
        //
        //  Delegates
        //

        /// <summary>
        /// A function that implements saving raw SGB save data somewhere.
        /// </summary>
        /// <param name="saveIndex">The index of the SGB save data to save.</param>
        /// <param name="dataToSave">The raw binary data of the SGB save file</param>
        public delegate void SaveDataDelegate(int saveIndex, Stream dataToSave);

        /// <summary>
        /// A function that implements reading info about an SGB save file.
        /// </summary>
        /// <param name="saveIndex">The index of the SGB save data to get info on.</param>
        /// <returns>The info requested about the given save data.</returns>
        public delegate SGBSaveInfo ReadSaveInfoDelegate(int saveIndex);

        /// <summary>
        /// A function that implements loading raw SGB save data from somewhere.
        /// </summary>
        /// <param name="saveIndex">The index of the SGB save data to load.</param>
        /// <returns>A stream holding the raw binary data of the SGB save file</returns>
        public delegate Stream LoadDataDelegate(int saveIndex);

        //
        //  Properties
        //

        /// <summary>
        /// The function to use when overriding SGB's save data stream functionality.
        /// If this is null, there is no override and SGB will save as normal.
        /// If this is populated, SGB will call this function after formatting
        /// the current save data as a binary stream.
        /// </summary>
        public static SaveDataDelegate SaveDataOverrideFunc { get; set; } = null;

        /// <summary>
        /// The function to use when overriding SGB's save data querying functionality.
        /// If this is null, there is no override and SGB will query details about save data normally.
        /// If this is populated, then whenever SGB needs to get data about a save file, it will call this
        /// function.
        /// </summary>
        public static ReadSaveInfoDelegate ReadSaveInfoOverrideFunc { get; set; } = null;

        /// <summary>
        /// The function to use when overriding SGB's load data stream funcitonality.
        /// If this is null, there is no override and SGB will load as normal.
        /// If this is populated, SGB will call this function to get a stream to read
        /// from when parsing binary data.
        /// </summary>
        public static LoadDataDelegate LoadDataOverrideFunc { get; set; } = null;

        //
        //  Constructor
        //

        static SGBSaveManager()
        {

        }


    }
}