using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BobboNet.SGB.IMod
{
    public class IModOverlay : MonoBehaviour
    {
        //
        //  Static Variables
        //

        private static Dictionary<string, Action> buttonActions = new Dictionary<string, Action>();

        //
        //  Static Methods
        //

        static IModOverlay()
        {
            // Supply a button by default that lets the user print hello world to the console.
            buttonActions.Add("Log 'Hello World!'", delegate()
            {
                Debug.Log("Hello World!");
            });

            // Supply a button by default that lets the user return to the test scene.
            // buttonActions.Add("Return to Test Scene", delegate() 
            // {
            //     SGBManager.UnloadSmileGame("SGB_IMod/TestScene");
            // });
        }


        //
        //  Variables
        //

        [Header("Required References")]
        public Button buttonWindowToggle;
        public RectTransform transformButtonParent;
        public Button buttonReturnToTestScene;
        public CanvasGroup canvasGroupPopoutWindow;

        private bool windowIsOpen = false;

        //
        //  Unity Methods
        //

        private void Awake()
        {
            buttonWindowToggle.onClick.AddListener(delegate ()
            {
                SetWindowOpen(!windowIsOpen);
            });
            
            SetWindowOpen(windowIsOpen);

            // Create a button for every button action!
            foreach(var entry in buttonActions)
            {
                CreateButton(entry.Key, entry.Value);
            }
        }

        //
        //  Private Methods
        //

        private Button CreateButton(string buttonText, Action buttonAction)
        {
            // Create an object for the Button, and populate it with all required components
            GameObject newButtonObject = new GameObject($"Button_{buttonText.ToLower().Replace(" ", "")}");
            newButtonObject.transform.SetParent(transformButtonParent, false);
            newButtonObject.AddComponent<CanvasRenderer>();
            newButtonObject.AddComponent<Image>();
            newButtonObject.AddComponent<LayoutElement>().preferredHeight = 30;
            Button newButton = newButtonObject.AddComponent<Button>();

            RectTransform newButtonRect = newButtonObject.GetComponent<RectTransform>();
            
            // Create an object for the button's text, and populate it with all required components.
            GameObject newTextObject = new GameObject("Text");
            newTextObject.transform.SetParent(newButtonObject.transform, false);
            newTextObject.AddComponent<CanvasRenderer>();
            Text newText = newTextObject.AddComponent<Text>();
            newText.text = buttonText;

            // Make text fill the whole button
            RectTransform newTextRect = newTextObject.GetComponent<RectTransform>();
            newTextRect.anchorMin = new Vector2(0.0f, 0.0f);
            newTextRect.anchorMax = new Vector2(1.0f, 1.0f);
            newTextRect.sizeDelta = new Vector2(0, 0);

            // Set the button's font
            Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            newText.font = ArialFont;
            newText.material = ArialFont.material;
            newText.color = Color.black;
            newText.alignment = TextAnchor.MiddleCenter;
            newText.fontStyle = FontStyle.Italic;

            // Set it so that when this button is clicked, it invokes the given action
            newButton.onClick.AddListener( () => buttonAction.Invoke() );

            return newButton;
        }

        private void SetWindowOpen(bool isOpen)
        {
            windowIsOpen = isOpen;
            canvasGroupPopoutWindow.alpha = isOpen ? 1 : 0;
            canvasGroupPopoutWindow.interactable = isOpen;
            canvasGroupPopoutWindow.blocksRaycasts = isOpen;
        }
    }
}
