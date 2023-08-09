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
            SetBackgroundMusicVolumeRaw(ConvertVolumeToInt(volume));
        }

        public static void SetBackgroundMusicVolumeRaw(int volume)
        {
            Debug.LogWarning($"Set max bgm to {volume}");
            currentVolumeBackgroundMusic = Mathf.Clamp(volume, MinVolume, MaxVolume);
        }

        public static float GetBackgroundMusicVolume() => ConvertVolumeToFloat(currentVolumeBackgroundMusic);
        public static int GetBackgroundMusicVolumeRaw() => currentVolumeBackgroundMusic;

        public static void SetSoundEffectsVolume(float volume)
        {
            SetSoundEffectsVolumeRaw(ConvertVolumeToInt(volume));
        }

        public static void SetSoundEffectsVolumeRaw(int volume)
        {
            currentVolumeSoundEffects = Mathf.Clamp(volume, MinVolume, MaxVolume);
        }

        public static float GetSoundEffectsVolume() => ConvertVolumeToFloat(currentVolumeSoundEffects);
        public static int GetSoundEffectsVolumeRaw() => currentVolumeSoundEffects;


        //
        //  Private Methods
        //

        private static int ConvertVolumeToInt(float volumeAsFloat) => Mathf.Clamp(Mathf.RoundToInt(volumeAsFloat * 100), MinVolume, MaxVolume);
        private static float ConvertVolumeToFloat(int volumeAsInt) => Mathf.Clamp(volumeAsInt / 100.0f, MinVolume, MaxVolume);
    }
}