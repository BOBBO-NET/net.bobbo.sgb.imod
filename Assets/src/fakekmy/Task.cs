using System;
using Yukar.Engine;

namespace SharpKmyBase
{
    public class Task
    {

        protected static void shutdown()
        {
            // Icy Override Start
            UnityEngine.Debug.LogWarning("Heads up... This method isn't implemented, but I don't think it's an issue. ~Icy");
            // throw new NotImplementedException();
            // Icy Override End
        }

        public virtual void update(float elapsed)
        {

        }

        public virtual void initialize()
        {

        }

        public virtual void finalize()
        {

        }

        //メニュー＞ゲームを終了する
        protected static void removeTask(GameMain gameMain)
        {
            // Unityではタイトルに戻るようにする
            gameMain.ChangeScene(GameMain.Scenes.TITLE);
        }
    }
}