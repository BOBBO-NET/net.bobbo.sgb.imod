using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobboNet.SGB.IMod
{
    public class TestScene : MonoBehaviour
    {
        [Header("Settings")]
        public string gameResourceLocation = "ExampleGame";

        [ContextMenu("Debug Load Smile Game")]
        public async void LoadIntoGame()
        {
            await SGBManager.LoadSmileGameAsync(gameResourceLocation);
        }

        public async void LoadIntoGame(string subpath)
        {
            await SGBManager.LoadSmileGameAsync(subpath);
        }
    }
}