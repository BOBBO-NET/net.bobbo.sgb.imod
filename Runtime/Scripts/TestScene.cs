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
        public void LoadIntoGame()
        {
            SGBManager.LoadSmileGame(gameResourceLocation);
        }

        public void LoadIntoGame(string subpath)
        {
            SGBManager.LoadSmileGame(subpath);
        }
    }
}