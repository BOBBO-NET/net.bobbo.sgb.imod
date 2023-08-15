using System;

namespace BobboNet.SGB.IMod
{
    /// <summary>
    /// Information about an SGB save file.
    /// </summary>
    [System.Serializable]
    public class SGBSaveInfo
    {
        /// <summary>
        /// The save index that this file belongs to. Must be >= 0 to be considered valid.
        /// </summary>
        public int Index { get; set; } = -1;

        /// <summary>
        /// Does this save file exist? If false, this is treated as an empty slot.
        /// </summary>
        /// 
        public bool Exists { get; set; } = false;

        /// <summary>
        /// The last time this save file was written to.
        /// </summary>
        public DateTime LastSaveDate { get; set; } = DateTime.Now;

        //
        //  Public Static Methods
        //

        /// <summary>
        /// Is the given save file loadable?
        /// In this context, loadable means that the save file exists and is valid.
        /// </summary>
        /// <param name="saveInfo">Information about the SGB save to query.</param>
        /// <returns>true if loadable, false otherwise.</returns>
        public static bool IsLoadable(SGBSaveInfo saveInfo)
        {
            if (saveInfo == null) return false;
            if (saveInfo.Index < 0) return false;
            if (!saveInfo.Exists) return false;

            return true;
        }
    }
}