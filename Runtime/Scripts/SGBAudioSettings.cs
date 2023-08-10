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

        public static void SetVolumeBGMRaw(int volume)
        {
            volumeBGM = Mathf.Clamp(volume, MinVolume, MaxVolume);
            UpdateVolumeState();
        }

        public static void SetVolumeBGM(float volume) => SetVolumeBGMRaw(ConvertVolumeToInt(volume));
        public static float GetVolumeBGM() => ConvertVolumeToFloat(volumeBGM);
        public static int GetVolumeBGMRaw() => volumeBGM;



        public static void SetVolumeSFXRaw(int volume)
        {
            volumeSFX = Mathf.Clamp(volume, MinVolume, MaxVolume);
            UpdateVolumeState();
        }

        public static void SetVolumeSFX(float volume) => SetVolumeSFXRaw(ConvertVolumeToInt(volume));
        public static float GetVolumeSFX() => ConvertVolumeToFloat(volumeSFX);
        public static int GetVolumeSFXRaw() => volumeSFX;


        //
        //  Private Methods
        //

        private static void UpdateVolumeState() => Audio.updateVolume();
        private static int ConvertVolumeToInt(float volumeAsFloat) => Mathf.Clamp(Mathf.RoundToInt(volumeAsFloat * 100), MinVolume, MaxVolume);
        private static float ConvertVolumeToFloat(int volumeAsInt) => Mathf.Clamp(volumeAsInt / 100.0f, MinVolume, MaxVolume);
    }
}