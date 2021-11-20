using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IModOverlay : MonoBehaviour
{
    [Header("Required References")]
    public Button buttonWindowToggle;
    public Button buttonReturnToTestScene;
    public CanvasGroup canvasGroupPopoutWindow;

    private bool windowIsOpen = false;


    private void Awake()
    {
        SetupEvents();
        SetWindowOpen(windowIsOpen);
    }

    private void SetupEvents()
    {
        buttonWindowToggle.onClick.AddListener(delegate ()
        {
            SetWindowOpen(!windowIsOpen);
        });

        buttonReturnToTestScene.onClick.AddListener(delegate ()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SGB_IMod/TestScene");
        });
    }


    private void SetWindowOpen(bool isOpen)
    {
        windowIsOpen = isOpen;
        canvasGroupPopoutWindow.alpha = isOpen ? 1 : 0;
        canvasGroupPopoutWindow.interactable = isOpen;
        canvasGroupPopoutWindow.blocksRaycasts = isOpen;
    }
}
