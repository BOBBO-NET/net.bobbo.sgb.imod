// Custom overrides description
// Resolution overrides, Margin corrections for the Save window selection options and message
using Microsoft.Xna.Framework;
using Yukar.Common;

namespace Yukar.Engine
{
    internal class SaveFileList : PagedSelectWindow
    {
        string[] saveFileList;
        bool[] saveFileFlag;
        string[] saveDateList;
        internal int availableDataCount;

        internal SaveFileList()
        {
            columnNum = 1;
            itemNumPerPage = 5;
            maxItems = 40;
            itemHeight = 68;
            //pageMarkOffsetY = 22;
            marginRow = 1;
        }

        public override void Initialize(CommonWindow.ParamSet pset, int x, int y, int maxWidth, int maxHeight)
        {

            // Custom overrides
            if (UnityEntry.mOverridesOn == true)
            {
                if (UnityEntry.mResolution == 1)
                {
                    // Do nothing
                }
                else
                {
                    x = 644;
                    y = 540;
                }
            }
            else
            {
                // Do nothing
            }
            // End of custom overrides

            base.Initialize(pset, x, y, maxWidth, maxHeight);

            saveFileList = new string[maxItems];
            saveDateList = new string[maxItems];
            saveFileFlag = new bool[maxItems];

            refreshList();
        }

#if WINDOWS
#else
        internal override void UpdateCallback()
        {
            base.UpdateCallback();
            decideByTouch();
        }
#endif
        internal override void DrawCallback()
        {
            DrawSelect(saveFileList, saveFileFlag);
            DrawReturnBox();
        }

        internal override void DrawMenuItem(int index, Vector2 pos, bool enabled)
        {
            base.DrawMenuItem(index, pos, enabled);

            var strs = saveDateList[index].Split('\n');
            int totalHeight = 0;
            float scale = 0.75f;
            foreach (var str in strs)
            {
                totalHeight += (int)(p.textDrawer.MeasureString(str).Y * scale * scale);
            }

            // Custom code
            // Date
            // pos.Y -= totalHeight / 4;
            pos.Y -= totalHeight / 4 + 5;
            // End of custom code
            foreach (var str in strs)
            {
                var size = new Vector2(innerWidth - TEXT_OFFSET, itemHeight);
                p.textDrawer.DrawString(str, pos, size,
                    TextDrawer.HorizontalAlignment.Right,
                    TextDrawer.VerticalAlignment.Center, Color.White, scale);
                // Custom code
                // pos.Y += totalHeight / 2;
                pos.Y += totalHeight / 2 + 10;
                // End of custom code
            }
        }

        internal void refreshList()
        {
            availableDataCount = 0;
            for (int i = 0; i < maxItems; i++)
            {
                saveFileList[i] = p.gs.glossary.saveData + (i + 1);
                saveDateList[i] = Util.getSaveDate(i, true);
                if (!string.IsNullOrEmpty(saveDateList[i]))
                    availableDataCount++;
                saveFileFlag[i] = true;
            }
        }

        // 右側のwindowをタップすることでもセーブデータの選択を可能に
        internal void decideByTouch(bool isMute = false)
        {
#if WINDOWS
#else
            if (p.owner.parent.owner.nowScene != GameMain.Scenes.TITLE) return;
#if IMOD
            if (UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
#else
            if (UnityEngine.Input.GetMouseButtonDown(0))
#endif
            {
                var touchPos = InputCore.getTouchPos(0);
                var offsetX = (int)windowPos.X - innerWidth / 2;
                var offsetY = (int)windowPos.Y - innerHeight / 2;
                if (offsetX + maxWindowSize.X < touchPos.x 
                    && touchPos.x < Graphics.ScreenWidth - offsetX 
                    && offsetY < touchPos.y 
                    && touchPos.y < maxWindowSize.Y)
                {
                    if (returnSelected == 0) result = selected;
                    else if (returnSelected > 0)
                    {
                        result = Util.RESULT_CANCEL;
                        if (!isMute) Audio.PlaySound(p.owner.parent.owner.se.cancel);
                    }
                }
            }
#endif
        }
    }

    internal class SaveDataDetail : StatusDigest
    {
        private const int STATUS_OFFSET_Y = 80;

        int currentIndex = -1;

        bool exist = false;
        string dataInfo = "";
        GameDataManager currentData;

        internal SaveDataDetail()
        {
        }

        internal override void Show()
        {
            // CommonWindowの処理だけをやる
            if (windowState == WindowState.SHOW_WINDOW || windowState == WindowState.RESIZING_WINDOW)
                return;

            windowState = WindowState.OPENING_WINDOW;
            frame = 0;

            // 1920/960 resolution
            // Custom overrides
            if (UnityEntry.mOverridesOn == true)
            {
                if (UnityEntry.mResolution == 1)
                {
                    // Do nothing
                }
                else
                {
                    windowPos.X = 1128;
                    windowPos.Y = 540;
                }
            }
            else
            {
                // Do nothing
            }

            p = p.clone();  // パーティ情報を書き換えないといけないので、ParamSet のクローンをもらう
            p.partyChars = null;

            setColumnNum(2);
            setRowNum(2, false);
            itemHeight -= STATUS_OFFSET_Y / 2;
        }

        internal override void Hide()
        {
            currentIndex = -1;
            currentData = null;

            base.Hide();
        }

        internal override void UpdateCallback()
        {
        }

        internal override void DrawCallback()
        {
            var pos = new Vector2(TEXT_OFFSET, TEXT_OFFSET);
            var size = new Vector2(innerWidth - TEXT_OFFSET * 2, innerHeight - TEXT_OFFSET * 2);

            if (!exist)
            {
                // セーブ地点と日付を書く
                p.textDrawer.DrawString(dataInfo, pos, size,
                    TextDrawer.HorizontalAlignment.Center,
                    TextDrawer.VerticalAlignment.Center, Color.White);
            }
            else
            {
                // セーブ地点と日付を書く
                p.textDrawer.DrawString(dataInfo, pos, size,
                    TextDrawer.HorizontalAlignment.Left,
                    TextDrawer.VerticalAlignment.Top, Color.White);

                // パーティ情報を書く
                var area = new Vector2(innerWidth / columnNum, itemHeight);
                for (int i = 0; i < currentData.party.members.Count; i++)
                {
                    // 座標算出
                    pos = Vector2.Zero;
                    pos.Y += STATUS_OFFSET_Y;
                    pos.X += (i % 2) * area.X;
                    pos.Y += (i / 2) * area.Y;
                    var origArea = area;

                    // 少しオフセットする
                    pos.X += DIGEST_AREA_OFFSET;
                    pos.Y += DIGEST_AREA_OFFSET;
                    origArea.X -= DIGEST_AREA_OFFSET * 2;
                    origArea.Y -= DIGEST_AREA_OFFSET * 2;

                    DrawDigest(pos, origArea, currentData.party, i);
                }
            }
        }

        public int CurrentIndex
        {
            get { return currentIndex; }
            set
            {
                if (value != currentIndex)
                    refreshInfo(value);

                currentIndex = value;
            }
        }

        private void refreshInfo(int index)
        {
            if (index == -1)
            {
                exist = false;
                dataInfo = "";
                currentData = null;
                return;
            }

            var date = Util.getSaveDate(index);
            if (string.IsNullOrEmpty(date))
            {
                exist = false;
                dataInfo = "-----";//p.gs.glossary.save + (index + 1) + " : " + p.gs.glossary.nothing;
                currentData = null;
            }
            else
            {
                exist = true;
                var catalog = p.owner.parent.owner.catalog;
                currentData = GameDataManager.Load(catalog, index);

                var mapName = "";
                var mapRom = catalog.getItemFromGuid(currentData.start.map) as Common.Rom.Map;
                if (mapRom != null)
                {
                    mapName = mapRom.name;
                }

                dataInfo = date + "\n" + mapName;

                p.partyChars = p.owner.RefreshPartyChr(currentData.party, p.partyChars);
            }
        }

        internal void DoSave()
        {
            p.owner.parent.owner.DoSave(currentIndex);
            // Custom code
            if (UnityEntry.mOverridesOn == true)
            {
                if (UnityEntry.mMargin == true)
                {
                    p.owner.parent.ShowToast("     " + p.gs.glossary.saved + "     ");
                }
                else
                {
                    p.owner.parent.ShowToast(p.gs.glossary.saved);
                }
            }
            else
            {
                p.owner.parent.ShowToast(p.gs.glossary.saved);
            }
            // End of custom code
            refreshInfo(currentIndex);
        }

        internal bool isExisted()
        {
            return exist;
        }
    }
}
