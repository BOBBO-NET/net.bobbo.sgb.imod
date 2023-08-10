using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yukar.Engine;
using UnityEngine.Audio;

namespace BobboNet.SGB.IMod
{
    public static class SGBAudioSettings
    {
        private const int MinVolume = 0;
        private const int MaxVolume = 100;
        private const int DefaultVolume = 50;

        private static int volumeBGM = DefaultVolume; // The current background music volume (0 - 100)
        private static int volumeSFX = DefaultVolume; // The current sound effect volume (0 - 100)

        private static AudioMixerGroup mixerGroupBGM = null; // (optional) The current mixer group to route BGM through
        private static AudioMixerGroup mixerGroupSFX = null; // (optional) The current mixer group to route SFX through

        private static string mixerVolumeHandleNameBGM = null; // (optional) The name of the BGM's volume handle in it's mixer
        private static string mixerVolumeHandleNameSFX = null; // (optional) The name of the SFX's volume handle in it's mixer 

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
        //  Mixer Methods
        //

        /// <summary>
        /// Update what audio mixer group all SGB background music should be routed through.
        /// </summary>
        /// <param name="mixerGroup">The group to route all BGM through. If null, unroutes all BGM.</param>
        /// <param name="volumeHandleName">The name of the mixer handle that controls this group's volume level. If null, volume level is ignored.</param>
        public static void SetMixerGroupBGM(AudioMixerGroup mixerGroup, string volumeHandleName = null)
        {
            mixerGroupBGM = mixerGroup;                     // Update the internal mixer group...
            mixerVolumeHandleNameBGM = volumeHandleName;    // ...cache a reference to it's volume handle...
            Audio.SetMixerGroupBGM(mixerGroup);             // ...and apply it to SGB
        }

        /// <summary>
        /// Get the active audio mixer group that all SGB background music is being routed through.
        /// </summary>
        /// <returns>The current BGM mixer group. Is null if unrouted.</returns>
        public static AudioMixerGroup GetMixerGroupBGM() => mixerGroupBGM;

        /// <summary>
        /// Update what audio mixer group all SGB sound effects should be routed through.
        /// </summary>
        /// <param name="mixerGroup">The group to route all SFX through. If null, unroutes all SFX.</param>
        /// <param name="volumeHandleName">The name of the mixer handle that controls this group's volume level. If null, volume level is ignored.</param>
        public static void SetMixerGroupSFX(AudioMixerGroup mixerGroup, string volumeHandleName = null)
        {
            mixerGroupSFX = mixerGroup;                     // Update the internal mixer group...
            mixerVolumeHandleNameSFX = volumeHandleName;    // ...cache a reference to it's volume handle...
            Audio.SetMixerGroupSFX(mixerGroup);             // ...and apply it to SGB
        }

        /// <summary>
        /// Get the active audio mixer group that all SGB sound effects are being routed through.
        /// </summary>
        /// <returns>The current SFX mixer group. Is null if unrouted.</returns>
        public static AudioMixerGroup GetMixerGroupSFX() => mixerGroupSFX;

        //
        //  Volume Methods
        //

        /// <summary>
        /// Set the RAW volume for SGB's background music.
        /// </summary>
        /// <param name="volume">Volume as an integer, from 0 - 100 (inclusive).</param>
        public static void SetVolumeBGMRaw(int volume)
        {
            volumeBGM = Mathf.Clamp(volume, MinVolume, MaxVolume);

            SetMixerVolumeIfNecessary(mixerGroupBGM, mixerVolumeHandleNameBGM, volumeBGM);
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

            SetMixerVolumeIfNecessary(mixerGroupSFX, mixerVolumeHandleNameSFX, volumeSFX);
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

        private static void SetMixerVolumeIfNecessary(AudioMixerGroup mixerGroup, string handleName, int rawVolume)
        {
            if (mixerGroup == null || handleName == null) return;

            mixerGroup.audioMixer.SetFloat(handleName, VolumeFloatToDb(ConvertVolumeToFloat(rawVolume)));
        }

        private static float VolumeFloatToDb(float volumeFloat)
        {
            if (volumeFloat != 0)
            {
                return 20f * Mathf.Log10(volumeFloat);
            }
            else
            {
                return -144.0f;
            }
        }
    }
}