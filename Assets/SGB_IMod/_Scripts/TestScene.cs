using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGB_IMod
{
    public class TestScene : MonoBehaviour
    {
        [Header("Settings")]
        public string gameResourceLocation = "none lmao";

        public void LoadIntoGame()
        {
            SGBManager.LoadSmileGame(gameResourceLocation);
        }
    }
}