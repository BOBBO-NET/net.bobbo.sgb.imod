﻿// Custom overrides description
// Resolution overrides, Margin corrections for the Config window selection options, customized flat Draw Gauge 
using Microsoft.Xna.Framework;
using KeyStates = Yukar.Engine.Input.KeyStates;
using StateType = Yukar.Engine.Input.StateType;

namespace Yukar.Engine
{
    class ConfigWindow : PagedSelectWindow
    {
        string[] strs;
        bool[] flags;
        int[] settings;
        int[] maxIndexes;

        //public int cursorImgId;
        public int selectedImgId;

        string[] messageSpeedTexts;

        string[] menuTypeTexts;

        /*
        string[] controlTypeTexts = 
        {
            "キーボード・コントローラ",
            "マウス",
        };
        */

        string[][] settingTextsArray;
        public const int RESTORE_INDEX = 4;

        internal ConfigWindow()
        {
            cursorImgId = Graphics.LoadImage("./res/system/arrow.png");
            selectedImgId = Graphics.LoadImage("./res/system/arrow_selected.png");
            disableLeftAndRight = true;
        }

        internal override void Show()
        {
            CreateMenu();

            Reset();

            maxItems = 5;
            setColumnNum(1);
            setRowNum(8, true);
            itemOffset = 16;
            // Custom overrides
            if (UnityEntry.mOverridesOn == true)
            {
                if (UnityEntry.mResolution == 2)
                {
                    windowPos.X = 960;
                    windowPos.Y = 540;
                }
            }
            // End of custom overrides
            base.Show();
        }

        private void CreateMenu()
        {
            messageSpeedTexts = new string[]{
                p.gs.glossary.message_moment,
                p.gs.glossary.message_fast,
                p.gs.glossary.message_normal,
                p.gs.glossary.message_slow,
            };

            menuTypeTexts = new string[]{
                p.gs.glossary.config_memory,
                p.gs.glossary.config_not_memory,
            };

            settingTextsArray = new string[][]{
                null,
                null,
                messageSpeedTexts,
                menuTypeTexts,
                //controlTypeTexts,
            };

            strs = new string[5];
            strs[0] = p.gs.glossary.bgm;
            strs[1] = p.gs.glossary.se;
            strs[2] = p.gs.glossary.message_speed;
            strs[3] = p.gs.glossary.cursor_position;
            //strs[4] = "操作方法";
            strs[4] = p.gs.glossary.restore_defaults;
            // Custom overrides
            if (UnityEntry.mOverridesOn == true)
            {
                if (UnityEntry.mMargin == true)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        strs[i] = " " + strs[i]; // Adding 1 space at left
                    }
                }
            }
            // End of custom overrides
            flags = new bool[5];
            flags[0] = flags[1] = flags[2] = flags[3] = flags[4] = true;

            settings = new int[4];
            maxIndexes = new int[4];
        }

        internal override void UpdateCallback()
        {
            base.UpdateCallback();

            if (selected < 0 || selected > 3 || returnSelected != 0)
                return;

            if (Input.KeyTest(StateType.REPEAT, KeyStates.LEFT))
            {
                if (settings[selected] == 0)
                    return;

                settings[selected]--;

                Audio.setMasterVolume((float)settings[0] / 100, (float)settings[1] / 100);
                Audio.PlaySound(p.owner.parent.owner.se.select);
            }
            else if (Input.KeyTest(StateType.REPEAT, KeyStates.RIGHT))
            {
                if (settings[selected] == maxIndexes[selected])
                    return;

                settings[selected]++;

                Audio.setMasterVolume((float)settings[0] / 100, (float)settings[1] / 100);
                Audio.PlaySound(p.owner.parent.owner.se.select);
            }

            editValuesByTouch();
        }

        // コンフィグタッチ選択
        // 押しっぱなしにも対応すべき
        internal void editValuesByTouch()
        {
#if !WINDOWS
            const int SEPARATE_X = 320;
            const int CONTENT_OFFSET_X = 20;    // 正しい？
            var offsetX = (int)windowPos.X - innerWidth / 2;
            int imgWidth = Graphics.GetImageWidth(cursorImgId);
            int imgHeight = Graphics.GetImageHeight(cursorImgId);
#if IMOD
            if (UnityEngine.InputSystem.Mouse.current.leftButton.isPressed)
#else
            if (UnityEngine.Input.GetMouseButton(0))
#endif
            {
                var touchPos = InputCore.getTouchPos(0);
                SharpKmyIO.Controller.repeatTouchLeft = false;
                SharpKmyIO.Controller.repeatTouchRight = false;
                for (int i = 0; i < maxItems - 1; i++)
                {
                    int cursorY = i * (itemHeight + itemOffset) + itemHeight / 2;
                    var x0 = SEPARATE_X + offsetX; // 仮
                    var y0 = cursorY;
                    var x1 = x0 + imgWidth;
                    var y1 = y0 + imgHeight;
                    if (touchPos.x < x1)
                    {
                        if (x0 < touchPos.x && touchPos.x < x1 && y0 < touchPos.y && touchPos.y < y1)
                        {
                            SharpKmyIO.Controller.repeatTouchRight = false;
                            SharpKmyIO.Controller.repeatTouchLeft = true;
                            break;
                        }
                    }
                    else if (touchPos.x > innerWidth - offsetX)
                    {
                        x0 = innerWidth - offsetX;
                        x1 = x0 + imgWidth;
                        if (x0 < touchPos.x && touchPos.x < x1 && y0 < touchPos.y && touchPos.y < y1)
                        {
                            SharpKmyIO.Controller.repeatTouchLeft = false;
                            SharpKmyIO.Controller.repeatTouchRight = true;
                            break;
                        }
                    }
                    else
                    {
                        // ゲージ直接操作
                        // yは今後使う可能性を考慮して残しておく
                        var pos = new Vector2(SEPARATE_X + imgWidth + CONTENT_OFFSET_X + offsetX, i * (itemHeight + itemOffset));
                        var size = new Vector2(innerWidth - pos.X - imgWidth - CONTENT_OFFSET_X + offsetX, itemHeight);
                        if (y0 < touchPos.y && touchPos.y < y1 && pos.X < touchPos.x && touchPos.x < pos.X + size.X)
                        {
                            selected = i;
                            if (selected > 1) break;
                            settings[selected] = (int)((touchPos.x - pos.X) / size.X * 101);
                            break;
                        }
                    }
                }
            }
#if IMOD
            if (UnityEngine.InputSystem.Mouse.current.leftButton.wasReleasedThisFrame)
#else
            if (UnityEngine.Input.GetMouseButtonUp(0))
#endif
            {
                SharpKmyIO.Controller.repeatTouchLeft = false;
                SharpKmyIO.Controller.repeatTouchRight = false;
                Audio.setMasterVolume((float)settings[0] / 100, (float)settings[1] / 100);

                // 効果音のよりも上を触っていたら音を出す
                if (selected <= 1)
                {
                    Audio.PlaySound(p.owner.parent.owner.se.select);
                }
            }
#endif // #! WINDOWS
        }

        internal override void DrawCallback()
        {
            const int SEPARATE_X = 320;
            const int CONTENT_OFFSET_X = 20;

            // 右側を空ける
            var oldWidth = innerWidth;
            innerWidth = SEPARATE_X;
            DrawSelect(strs, flags);
            DrawReturnBox(true);
            innerWidth = oldWidth;

            int imgWidth = Graphics.GetImageWidth(cursorImgId);
            int imgHeight = Graphics.GetImageHeight(cursorImgId);

            if (selected >= 0 && selected <= 3 && returnSelected == 0)
            {
                // 設定変更ボタンを書く
                int cursorY = selected * (itemHeight + itemOffset) + (itemHeight - imgHeight) / 2;
                var srcRect = new Rectangle(innerWidth, cursorY, -imgWidth, imgHeight);
                var destRect = new Rectangle(0, 0, imgWidth, imgHeight);

                if (settings[selected] > 0)
                    Graphics.DrawImage(cursorImgId, SEPARATE_X, cursorY);
                if (settings[selected] < maxIndexes[selected])
                    Graphics.DrawImage(cursorImgId, srcRect, destRect);
            }

            // 各設定を書く
            for (int i = 0; i < 4; i++)
            {
                var pos = new Vector2(SEPARATE_X + imgWidth + CONTENT_OFFSET_X, i * (itemHeight + itemOffset));
                var size = new Vector2(innerWidth - pos.X - imgWidth - CONTENT_OFFSET_X, itemHeight);

                if (i < 2)
                    DrawGauge(i, pos, size);
                else
                    DrawString(i, pos, size);

                DrawSeparater(i, pos, size);
            }

        }

        private void DrawSeparater(int i, Vector2 pos, Vector2 size)
        {
            Graphics.DrawFillRect(0, (int)(pos.Y + size.Y + itemOffset / 2),
                innerWidth, 1, 64, 64, 64, 0);
        }

        private void DrawGauge(int i, Vector2 pos, Vector2 size)
        {
            // 数字を書く
            p.textDrawer.DrawString("" + settings[i], pos, size,
                TextDrawer.HorizontalAlignment.Center,
                TextDrawer.VerticalAlignment.Top, Color.White, 0.75f);

            // Custom overrides
            #region <Custom overrides>
            // Border
            if (UnityEntry.mOverridesOn == true)
            {
                if (UnityEntry.mFlatGauge == true)
                {
                    // Gauge height = 24
                    pos.Y += itemHeight - 24;
                    size.Y = 24;
                    // Change 128, 128, 128, 255 to your RGBA values
                    Graphics.DrawFillRect((int)pos.X, (int)(pos.Y), (int)size.X, (int)size.Y, 128, 128, 128, 255);
                }
                else
                {
                    // Gauge height = 16 (default)
                    pos.Y += itemHeight - 24;
                    size.Y = 16;
                    Graphics.DrawFillRect((int)pos.X, (int)(pos.Y), (int)size.X, (int)size.Y, 128, 128, 128, 128);
                }
            }
            else
            {
                // Gauge height = 16 (default)
                pos.Y += itemHeight - 24;
                size.Y = 16;
                Graphics.DrawFillRect((int)pos.X, (int)(pos.Y), (int)size.X, (int)size.Y, 128, 128, 128, 128);
            }

            if (UnityEntry.mOverridesOn == true)
            {
                if (UnityEntry.mFlatGauge == true)
                {
                    // Border width = 1
                    pos.X += 1;
                    pos.Y += 1;
                    size.X -= 2;
                    size.Y -= 2;
                }
                else
                {
                    // Border width = 2 (default)
                    pos.X += 2;
                    pos.Y += 2;
                    size.X -= 4;
                    size.Y -= 4;
                }
            }
            else
            {
                // Border width = 2 (default)
                pos.X += 2;
                pos.Y += 2;
                size.X -= 4;
                size.Y -= 4;
            }

            // Background
            if (UnityEntry.mOverridesOn == true)
            {
                if (UnityEntry.mFlatGauge == true)
                {
                    // Change 0, 0, 0, 224 to your RGBA values
                    Graphics.DrawFillRect((int)pos.X, (int)(pos.Y), (int)size.X, (int)size.Y, 0, 0, 0, 224);
                }
                else
                {
                    Graphics.DrawFillRect((int)pos.X, (int)(pos.Y), (int)size.X, (int)size.Y, 0, 0, 0, 128);
                }
            }
            else
            {
                Graphics.DrawFillRect((int)pos.X, (int)(pos.Y), (int)size.X, (int)size.Y, 0, 0, 0, 128);
            }

            // Top part
            size.X = size.X * settings[i] / 100;


            if (UnityEntry.mOverridesOn == true)
            {
                if (UnityEntry.mFlatGauge == true)
                {
                    // Change 236, 30, 80, 255 to your RGBA values
                    Graphics.DrawFillRect((int)pos.X + 4, (int)(pos.Y) + 4, (int)size.X - 8, (int)size.Y - 8, 236, 30, 80, 255);
                }
                else
                {
                    Graphics.DrawFillRect((int)pos.X, (int)(pos.Y), (int)size.X, (int)size.Y, 64, 48, 192, 255);
                }
            }
            else
            {
                Graphics.DrawFillRect((int)pos.X, (int)(pos.Y), (int)size.X, (int)size.Y, 64, 48, 192, 255);
            }


            // Middle part
            if (UnityEntry.mOverridesOn == true)
            {
                if (UnityEntry.mFlatGauge == true)
                {
                    // Do nothing because we need only Top part for a flat style
                }
                else
                {
                    size.Y /= 2;
                    pos.Y += size.Y / 2;
                    Graphics.DrawFillRect((int)pos.X, (int)(pos.Y), (int)size.X, (int)size.Y, 32, 16, 96, 255);
                }
            }
            else
            {
                size.Y /= 2;
                pos.Y += size.Y / 2;
                Graphics.DrawFillRect((int)pos.X, (int)(pos.Y), (int)size.X, (int)size.Y, 32, 16, 96, 255);
            }

            // Bottom part
            if (UnityEntry.mOverridesOn == true)
            {
                if (UnityEntry.mFlatGauge == true)
                {
                    // Do nothing because we need only Top part for a flat style
                }
                else
                {
                    pos.Y += size.Y / 2;
                    Graphics.DrawFillRect((int)pos.X, (int)(pos.Y), (int)size.X, (int)size.Y, 16, 8, 64, 255);
                }
            }
            else
            {
                pos.Y += size.Y / 2;
                Graphics.DrawFillRect((int)pos.X, (int)(pos.Y), (int)size.X, (int)size.Y, 16, 8, 64, 255);
            }
            #endregion
            // End of custom overrides
        }

        private void DrawString(int i, Vector2 pos, Vector2 size)
        {
            p.textDrawer.DrawString(settingTextsArray[i][settings[i]], pos, size,
                TextDrawer.HorizontalAlignment.Center,
                TextDrawer.VerticalAlignment.Center, Color.White);
        }

        internal void Apply()
        {
#if !IMOD
            p.owner.parent.owner.data.system.bgmVolume = settings[0];
            p.owner.parent.owner.data.system.seVolume = settings[1];
#else
            BobboNet.SGB.IMod.SGBAudioSettings.SetVolumeBGMRaw(settings[0]);
            BobboNet.SGB.IMod.SGBAudioSettings.SetVolumeSFXRaw(settings[1]);
#endif
            p.owner.parent.owner.data.system.messageSpeed = (Common.GameData.SystemData.MessageSpeed)settings[2];
            p.owner.parent.owner.data.system.cursorPosition = (Common.GameData.SystemData.CursorPosition)settings[3];
            //p.owner.parent.owner.data.system.controlType = (Common.GameData.SystemData.ControlType)settings[4];
        }

        internal void Reset()
        {
#if !IMOD
            settings[0] = p.owner.parent.owner.data.system.bgmVolume;
#else
            settings[0] = BobboNet.SGB.IMod.SGBAudioSettings.GetVolumeBGMRaw();
#endif
            maxIndexes[0] = 100;
#if !IMOD
            settings[1] = p.owner.parent.owner.data.system.seVolume;
#else
            settings[1] = BobboNet.SGB.IMod.SGBAudioSettings.GetVolumeSFXRaw();
#endif
            maxIndexes[1] = 100;
            settings[2] = (int)p.owner.parent.owner.data.system.messageSpeed;
            maxIndexes[2] = (int)Common.GameData.SystemData.MessageSpeed.SLOW;
            settings[3] = (int)p.owner.parent.owner.data.system.cursorPosition;
            maxIndexes[3] = (int)Common.GameData.SystemData.CursorPosition.NOT_KEEP;
            //settings[4] = (int)p.owner.parent.owner.data.system.controlType;
            //maxIndexes[4] = (int)Common.GameData.SystemData.ControlType.MOUSE_TOUCH;
        }

        internal void RestoreDefaults()
        {
            p.owner.parent.owner.data.system.restoreDefaults();

#if !IMOD
            settings[0] = p.owner.parent.owner.data.system.bgmVolume;
            settings[1] = p.owner.parent.owner.data.system.seVolume;
#else
            settings[0] = BobboNet.SGB.IMod.SGBAudioSettings.GetVolumeBGMRaw();
            settings[1] = BobboNet.SGB.IMod.SGBAudioSettings.GetVolumeSFXRaw();
#endif
            settings[2] = (int)p.owner.parent.owner.data.system.messageSpeed;
            settings[3] = (int)p.owner.parent.owner.data.system.cursorPosition;
            //settings[4] = (int)p.owner.parent.owner.data.system.controlType;
            Audio.setMasterVolume((float)settings[0] / 100, (float)settings[1] / 100);
        }
    }
}
