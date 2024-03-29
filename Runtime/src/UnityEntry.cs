﻿// Custom overrides description
// Set overrides setting below
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Yukar.Common;
using Yukar.Engine;

public class UnityEntry : MonoBehaviour
{
    // Icy Override Start

    // When loading Catalog content, this flag will be used to determine where to load it from.
    public static string gameSubpathName = "ExampleGame";

    // Icy Override End

#if IMOD
    public static bool overrideInit = false;
#endif


    // Custom overrides
    static public bool mOverridesOn = false;             // Set false to skip all custom overrides and true to apply overrides below
    static public int mResolution = 2;                 // Set 1 for 960 x 544 (540) resolution, set 2 for 1920 x 1080 resolution
    static public bool mMargin = true;                  // Set false to skip margin correction and true to apply margin correction
    static public bool mRemoveFontBorders = true;       // Set true to remove font borders and false to default font borders
    static public bool mFlatGauge = true;               // Set true to apply flat gauge styles in the Config windows and false to default
    static public bool mHeroesNamesDecoration = true;   // Set true to apply custom color and font size to the heroes name labels in Menu
                                                        // If mHeroesNamesDecoration is true you must set both parameters below
                                                        // Set custom scale of hero name label font (default not set, used internal font scale)
    static public float mHeroesNamesDecorationFontScale = 1.2f;
    // Set custom color of hero name label (default Microsoft.Xna.Framework.Color.White)
    static public Microsoft.Xna.Framework.Color mHeroesNamesDecorationFontColor = Microsoft.Xna.Framework.Color.DeepPink;
    // End of custom overrides

    public static GameMain game;
    public static UnityEntry self;

    public bool didInit = false;
    public GameMain didInitWithGame = null;
    public UnityEntry didInitWithSelf = null;

    private static Texture2D sFrameBuffer;
#if UNITY_IOS || UNITY_ANDROID
    int mUpdateSkipCount = 2;//解像度変更のため２フレーム待つ
#endif


    public static void InitializeDir()
    {
        // Icy Override Start
#if UNITY_SWITCH && !UNITY_EDITOR
        Catalog.sInResourceDir = Path.Combine("SGB", gameSubpathName).Replace("\\", "/");
        string assetPath = Path.Combine(Application.dataPath, "Resources", Catalog.sInResourceDir);

        // Converted below syntax to use path.combine for better cross platform support
        Catalog.sCommonResourceDir = (Path.Combine(assetPath, "samples", "common") + Path.DirectorySeparatorChar).Replace("\\", "/");
        Catalog.sResourceDir = (assetPath + Path.DirectorySeparatorChar).Replace("\\", "/");
        Catalog.sDlcDir = (Path.Combine(assetPath, "samples") + Path.DirectorySeparatorChar).Replace("\\", "/");
#else
        Catalog.sInResourceDir = Path.Combine("SGB", gameSubpathName).Replace("\\", "/");
        string assetPath = Path.Combine("Assets", "Resources", Catalog.sInResourceDir);	// 実際の所なんでもいい (It doesn't matter what the actual place is)

        // Converted below syntax to use path.combine for better cross platform support
        Catalog.sCommonResourceDir = (Path.Combine(assetPath, "samples", "common") + Path.DirectorySeparatorChar).Replace("\\", "/");
        Catalog.sResourceDir = (assetPath + Path.DirectorySeparatorChar).Replace("\\", "/");
        Catalog.sDlcDir = (Path.Combine(assetPath, "samples") + Path.DirectorySeparatorChar).Replace("\\", "/");
#endif
        // Icy Override End
    }

    // Use this for initialization
    void Start()
    {
        if (overrideInit || didInit) return;

        if (game != null) return;
        SharpKmyGfx.Render.InitializeRender();
        UnityUtil.Initialize();
        self = this;
#if !UNITY_EDITOR
        Debug.unityLogger.logEnabled = false;
#endif
#if UNITY_IOS || UNITY_ANDROID
        UnityResolution.Start();
#endif //UNITY_IOS || UNITY_ANDROID

        Yukar.Common.FileUtil.language = Application.systemLanguage.ToString();

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        InitializeDir();

        FileUtil.initialize();

        Catalog.createDlcList(false);
        Catalog catalog = new Catalog();
        catalog.load(false);

#if !IMOD
        Yukar.Common.GameData.SystemData.sDefaultBgmVolume = catalog.getGameSettings().defaultBgmVolume;
        Yukar.Common.GameData.SystemData.sDefaultSeVolume = catalog.getGameSettings().defaultSeVolume;
#endif

        game = new GameMain();
        game.initialize();

        UnityAdsManager.Initialize(game);

        didInit = true;
        didInitWithGame = game;
        didInitWithSelf = this;
    }

    // Icy Override Start
    private void OnDestroy()
    {
        if (!didInit) return;

        // Deinit everything we can
        // FOR THE FUTURE - fix a case here where we create a deinit for the UnityResolution class
        UnityUtil.DeInit();

        if (game == didInitWithGame)
        {
            game.finalize();
            game = null;
        }

        if (self == didInitWithSelf)
        {
            self = null;
        }
    }
    // Icy Override End


    // Update is called once per frame
    void Update()
    {
#if IMOD
        if (Yukar.Engine.Input.IModInput.Window.ToggleFullscreen.WasPressedThisFrame())
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
#elif UNITY_STANDALONE_WIN
        if (UnityEngine.Input.GetKeyDown(KeyCode.F4))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.LeftAlt) && UnityEngine.Input.GetKey(KeyCode.Return) || UnityEngine.Input.GetKeyDown(KeyCode.RightAlt) && UnityEngine.Input.GetKey(KeyCode.Return))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
#elif UNITY_IOS || UNITY_ANDROID
        if (0 < this.mUpdateSkipCount){
            this.mUpdateSkipCount--;
            return;
        }
        UnityResolution.Update();
#endif // UNITY_STANDALONE_WIN
        game.update(Time.deltaTime);
        GameMain.setElapsedTime(Time.deltaTime);
    }

    void OnGUI()
    {
        if (Event.current.type == EventType.Repaint)
        {
            SharpKmyGfx.SpriteBatch.DrawAll();
        }
    }

    void OnApplicationQuit()
    {
        self = null;
    }

    public static bool IsImportMapScene()
    {
#if UNITY_EDITOR
        if (CustomImporter.IsImportMapScene) return true;
#endif
        return false;
    }

    public static bool IsReimportMapScene()
    {
#if UNITY_EDITOR
        if (CustomImporter.IsReimportMapScene) return true;
#endif
        return false;
    }

    public static bool IsDivideMapScene()
    {
        return Yukar.Common.Rom.GameSettings.IsDivideMapScene;
    }

    public static void capture()
    {
        var mainCamera = GameObject.Find("Main Camera");

        if (mainCamera == null) return;

        var camera = mainCamera.GetComponent<Camera>();
        var rendertexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.Default, RenderTextureReadWrite.Default);

        camera.targetTexture = rendertexture;

        RenderTexture.active = rendertexture;
        camera.Render();

        sFrameBuffer = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        sFrameBuffer.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        sFrameBuffer.Apply();

        RenderTexture.active = null;
        camera.targetTexture = null;
    }

    public static void reserveClearFB()
    {
        self.StartCoroutine(clearFB(false));
    }

    public static IEnumerator clearFB(bool immediate)
    {
        if (!immediate)
            yield return new WaitForEndOfFrame();

        if (sFrameBuffer == null)
            yield break;

        Destroy(sFrameBuffer);
        sFrameBuffer = null;
    }

    public static bool isFBCaptured()
    {
        return sFrameBuffer != null;
    }

    internal static void blackout()
    {
        self.StartCoroutine(clearFB(true));

        sFrameBuffer = new Texture2D(1, 1, TextureFormat.RGB24, false);
        sFrameBuffer.SetPixel(0, 0, Color.black);
        sFrameBuffer.Apply();
    }

    public static void drawCapturedFB()
    {
        if (sFrameBuffer == null)
            return;

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), sFrameBuffer);
    }

    internal static void startCoroutine(IEnumerator routine)
    {
        self.StartCoroutine(routine);
    }
}


#if UNITY_IOS || UNITY_ANDROID
class UnityResolution
{
    const int ScreenSizeW = 960;
    //const int ScreenSizeH = 540;
    const int FrameRate = 60;
    
    protected static List<int> sScreenSizeList = new List<int>();
    protected static float sScreenSizeAspect = 0;
    protected static int sCurrentScreenSizeIndex = 0;
    protected static List<float> sFrameRateList = new List<float>();

    public static void Start()
    {
        //初期化
        ClearFrameRateList();
        Application.targetFrameRate = FrameRate;

        //可能とする解像度を追加
        sScreenSizeAspect = (float)Screen.height / (float)Screen.width;
#if true
        if (Screen.width <= ScreenSizeW
        || Screen.width / 2 <=  ScreenSizeW)
        {
            sScreenSizeList.Add(Screen.width);
        }
        else
        {
            if(Screen.width / 2 < ScreenSizeW * 2) {
                sScreenSizeList.Add(Screen.width / 2);
            }
            else
            {
                sScreenSizeList.Add(ScreenSizeW * 2);
            }
        }
#else
        sScreenSizeList.Add(Screen.width);
        if (ScreenSizeW < Screen.width)
        {
            for (int i1 = 1; true; ++i1)
            {
                int w = ScreenSizeW * i1;
                if (Screen.width <= w) break;
                sScreenSizeList.Add(w);
            }
        }
        for (int i1 = 2; true; ++i1)
        {
            int w = Screen.width / i1;
            if (w <= ScreenSizeW) break;
            sScreenSizeList.Add(w);
        }
        sScreenSizeList.Sort();

        //解像度の大きさに制限
        for(int i1 = sScreenSizeList.Count -1; 0 <= i1; --i1)
        {
            if (sScreenSizeList[i1] <= ScreenSizeW * 2) continue;
            sScreenSizeList.RemoveAt(i1);
        }
#endif

        //解像度の初期設定
        sCurrentScreenSizeIndex = -1;
        SetResolution(sScreenSizeList.Count - 1);
    }


    public static void Update()
    {
        //フレームリストに追加
        float fps = 1f / Time.deltaTime;
        if (fps < 3) return;//明らかに低い場合は無視する
        AddFrameRateList(fps);

        //フレームが出ているか確認
        float ave = GetFrameRateAve();
        if (45 < ave) return;

        //解像度を下げる
        Down();
    }

    public static void Up()
    {
        SetResolution(sCurrentScreenSizeIndex + 1);
        ClearFrameRateList();
    }

    public static void Down()
    {
        SetResolution(sCurrentScreenSizeIndex - 1);
        ClearFrameRateList();
    }

    static void ClearFrameRateList()
    {
        sFrameRateList.Clear();
        for (var i1 = 0; i1 < FrameRate * 0.1; ++i1) sFrameRateList.Add(FrameRate);
    }

    static void AddFrameRateList(float inFps)
    {
        sFrameRateList.RemoveAt(0);
        sFrameRateList.Add(inFps);
    }

    static float GetFrameRateAve()
    {
        float ave = 0;
        foreach (var fps in sFrameRateList) ave += fps;
        ave /= sFrameRateList.Count;
        return ave;
    }

    static void SetResolution(int inIndex)
    {
        if (inIndex < 0) inIndex = 0;
        if (sScreenSizeList.Count <= inIndex) inIndex = sScreenSizeList.Count - 1;
        if (inIndex == sCurrentScreenSizeIndex) return;
        sCurrentScreenSizeIndex = inIndex;

        int width = sScreenSizeList[inIndex];
        int height = (int)((float)width * sScreenSizeAspect);
        if (Screen.width == width) return;

        Screen.SetResolution(width, height, true, FrameRate);
        Debug.Log("Screen.SetResolution (" + width.ToString() + "," + height.ToString() + ")");
    }
}
#endif //UNITY_IOS || UNITY_ANDROID
