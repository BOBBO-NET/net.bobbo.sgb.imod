using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yukar.Engine;
using UnityEngine.Audio;

namespace BobboNet.SGB.IMod
{
    public static class VolumeUtils
    {
        public static float VolumeFloatToDb(float volumeFloat)
        {
            // Ty Liandur for the implementation: 
            // (https://discussions.unity.com/t/how-to-convert-decibel-db-number-to-audio-source-volume-number-0to1/46543)
            if (volumeFloat != 0)
            {
                return 20f * Mathf.Log10(volumeFloat);
            }
            else
            {
                return -144.0f;
            }
        }

        public static float DbToVolumeFloat(float volumeDb)
        {
            // Ty Liandur for the implementation: 
            // (https://discussions.unity.com/t/how-to-convert-decibel-db-number-to-audio-source-volume-number-0to1/46543)
            return Mathf.Pow(10.0f, volumeDb / 20.0f);
        }
    }
}