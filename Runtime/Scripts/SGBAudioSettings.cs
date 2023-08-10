using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yukar.Engine;

namespace BobboNet.SGB.IMod
{
    public static class SGBAudioSettings
    {
        private const int MinVolume = 0;
        private const int MaxVolume = 100;
        private const int DefaultVolume = 50;

        private static int volumeBGM = DefaultVolume;
        private static int volumeSFX = DefaultVolume;

        //
        //  Constructor
        //

        static SGBAudioSettings()
        {
            // Add debug buttons to overlay that allow BGM and SFX volume adjustment
            IModOverlay.AddButtonAction("SGB BGM Volume +", () => SetVolumeBGMRaw(volumeBGM + 5));
            IModOverlay.AddButtonAction("SGB BGM Volume -", () => SetVolumeBGMRaw(volumeBGM - 5));
            IModOverlay.AddButtonAction("SGB SFX Volume +", () => SetVolumeSFXRaw(volumeSFX + 5));
            IModOverlay.AddButtonAction("SGB SFX Volume -", () => SetVolumeSFXRaw(volumeSFX - 5));
        }

        //
        //  Public Methods
        //

        /// <summary>
        /// Set the RAW volume for SGB's background music.
        /// </summary>
        /// <param name="volume">Volume as an integer, from 0 - 100 (inclusive).</param>
        public static void SetVolumeBGMRaw(int volume)
        {
            volumeBGM = Mathf.Clamp(volume, MinVolume, MaxVolume);
            UpdateVolumeState();
        }

        /// <summary>
        /// Set the volume for SGB's background music.
        /// </summary>
        /// <param name="volume">Volume as a float, from 0.0 - 1.0 (inclusive).</param>
        public static void SetVolumeBGM(float volume) => SetVolumeBGMRaw(ConvertVolumeToInt(volume));

        /// <summary>
        /// Get the volume of SGB's background music.
        /// </summary>
        /// <returns>Volume as a float, from 0.0 - 1.0 (inclusive).</returns>
        public static float GetVolumeBGM() => ConvertVolumeToFloat(volumeBGM);

        /// <summary>
        /// Get the RAW volume of SGB's background music.
        /// </summary>
        /// <returns>Volume as an integer, from 0 - 100 (inclusive).</returns>
        public static int GetVolumeBGMRaw() => volumeBGM;


        /// <summary>
        /// Set the RAW volume for SGB's sound effects.
        /// </summary>
        /// <param name="volume">Volume as an integer, from 0 - 100 (inclusive).</param>
        public static void SetVolumeSFXRaw(int volume)
        {
            volumeSFX = Mathf.Clamp(volume, MinVolume, MaxVolume);
            UpdateVolumeState();
        }

        /// <summary>
        /// Set the volume for SGB's sound effects.
        /// </summary>
        /// <param name="volume">Volume as a float, from 0.0 - 1.0 (inclusive).</param>
        /// 
        public static void SetVolumeSFX(float volume) => SetVolumeSFXRaw(ConvertVolumeToInt(volume));

        /// <summary>
        /// Get the volume of SGB's sound effects.
        /// </summary>
        /// <returns>Volume as a float, from 0.0 - 1.0 (inclusive).</returns>
        public static float GetVolumeSFX() => ConvertVolumeToFloat(volumeSFX);

        /// <summary>
        /// Get the RAW volume of SGB's sound effects.
        /// </summary>
        /// <returns>Volume as an integer, from 0 - 100 (inclusive).</returns>
        public static int GetVolumeSFXRaw() => volumeSFX;


        //
        //  Private Methods
        //

        private static void UpdateVolumeState() => Audio.updateVolume(); // Update the volume in SGB
        private static int ConvertVolumeToInt(float volumeAsFloat) => Mathf.Clamp(Mathf.RoundToInt(volumeAsFloat * 100), MinVolume, MaxVolume);
        private static float ConvertVolumeToFloat(int volumeAsInt) => Mathf.Clamp(volumeAsInt / 100.0f, MinVolume, MaxVolume);
    }
}