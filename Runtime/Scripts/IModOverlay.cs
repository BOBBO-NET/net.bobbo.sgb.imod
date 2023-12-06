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
        //  Types
        //

        private delegate void ButtonActionAdded(string text, Action action);

        //
        //  Static Variables
        //

        private static Dictionary<string, Action> buttonActions = new Dictionary<string, Action>();
        private static event ButtonActionAdded onButtonActionAdded;

        //
        //  Static Methods
        //

        static IModOverlay()
        {
            // Supply a button by default that lets the user print hello world to the console.
            AddButtonAction("Log 'Hello World!'", delegate ()
            {
                Debug.Log("Hello World!");
            });
        }

        public static void AddButtonAction(string buttonText, Action buttionAction)
        {
            buttonActions.Add(buttonText, buttionAction);
            onButtonActionAdded?.Invoke(buttonText, buttionAction);
        }


        //
        //  Variables
        //

        [Header("Required References")]
        public Button buttonWindowToggle;
        public RectTransform transformButtonParent;
        public CanvasGroup canvasGroupPopoutWindow;

        private bool windowIsOpen = false;

        //
        //  Unity Methods
        //

        private void Awake()
        {
            // If we're not in the editor, DESTROY THIS.
            if (!Application.isEditor)
            {
                Destroy(gameObject);
                return;
            }

            buttonWindowToggle.onClick.AddListener(delegate ()
            {
                SetWindowOpen(!windowIsOpen);
            });

            SetWindowOpen(windowIsOpen);

            // Create a button for every button action!
            foreach (var entry in buttonActions)
            {
                CreateButton(entry.Key, entry.Value);
            }

            onButtonActionAdded += OnNewButtonActionAdded;
        }

        private void OnDestroy()
        {
            onButtonActionAdded -= OnNewButtonActionAdded;
        }

        //
        //  Private Methods
        //

        private void OnNewButtonActionAdded(string text, Action action)
        {
            CreateButton(text, action);
        }

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
#if UNITY_2022_2_OR_NEWER
            Font ArialFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
#else
            Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
#endif
            newText.font = ArialFont;
            newText.material = ArialFont.material;
            newText.color = Color.black;
            newText.alignment = TextAnchor.MiddleCenter;
            newText.fontStyle = FontStyle.Italic;

            // Set it so that when this button is clicked, it invokes the given action
            newButton.onClick.AddListener(() => buttonAction.Invoke());

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
