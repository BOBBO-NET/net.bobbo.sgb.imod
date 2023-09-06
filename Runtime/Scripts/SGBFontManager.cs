using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobboNet.SGB.IMod
{
    public static class SGBFontManager
    {
        //
        //  Properties
        //

        /// <summary>
        /// The current font that SGB should use. By default this is whatever the first font with the name "font.ttf"
        /// is found in the resource folder.
        /// </summary>
        public static Font CurrentFont { get; set; }

        //
        //  Init
        //

        static SGBFontManager()
        {
            // Load the default font
            CurrentFont = Resources.Load<Font>("font");
        }
    }
}