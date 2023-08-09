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

        private static int currentVolumeBackgroundMusic = DefaultVolume;
        private static int currentVolumeSoundEffects = DefaultVolume;

        //
        //  Public Methods
        //

        public static void SetBackgroundMusicVolume(float volume)
        {
            currentVolumeBackgroundMusic = ConvertVolumeToInt(volume);
        }

        public static float GetBackgroundMusicVolume() => ConvertVolumeToFloat(currentVolumeBackgroundMusic);
        public static int GetBackgroundMusicVolumeRaw() => currentVolumeBackgroundMusic;

        public static void SetSoundEffectsVolume(float volume)
        {
            currentVolumeSoundEffects = ConvertVolumeToInt(volume);
        }

        public static float GetSoundEffectsVolume() => ConvertVolumeToFloat(currentVolumeSoundEffects);
        public static float GetSoundEffectsVolumeRaw() => currentVolumeSoundEffects;


        //
        //  Private Methods
        //

        private static int ConvertVolumeToInt(float volumeAsFloat) => Mathf.Clamp(Mathf.RoundToInt(volumeAsFloat * 100), MinVolume, MaxVolume);
        private static float ConvertVolumeToFloat(int volumeAsInt) => Mathf.Clamp(volumeAsInt / 100.0f, MinVolume, MaxVolume);
    }
}