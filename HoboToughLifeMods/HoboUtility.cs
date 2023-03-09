using MelonLoader;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UI;
using Game;
using System;
using Core.Net;
using Game.ArcadeMachine;
using Core;
using UnhollowerRuntimeLib;

namespace HoboToughLifeMods
{
    public class HoboUtility : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Hello HoboUtility");
        }

        [HarmonyPatch(typeof(GUIMapManager), "Show", new Type[] { typeof(bool) })]
        static class GUIMapManager_Hook
        {
            private static UniversalNetworkPlayer MyNetworkPlayer = null;
            private static GameObject DammyMapPlayer = default;
            private static GUIMapPlayerPoint DammyMapPlayerPoint = default;
            private static GameObject CanvasObject = default;

            static void Postfix(GUIMapManager __instance, bool hasMap)
            {
                SetMyNetworkPlayer();
                CreateDammyMapPlayerObject();
                UpdateDammyMapPlayer();
            }

            /// <summary>
            /// 自キャラのコンポーネントを取得
            /// </summary>
            static void SetMyNetworkPlayer()
            {
                if (MyNetworkPlayer != null) { return; }

                var playerObject = GameObject.FindObjectOfType<UniversalNetworkPlayer>();
                if(playerObject == null) { return; }

                MyNetworkPlayer = playerObject.GetComponent<UniversalNetworkPlayer>();
            }

            /// <summary>
            /// マップにダミーアイコンを作成
            /// </summary>
            static void CreateDammyMapPlayerObject()
            {
                if (MyNetworkPlayer == null) { return; }

                bool hasCreated = DammyMapPlayer != null;
                if (hasCreated) { return; }

                var mapPlayer = GameObject.Find("GUI/GUICanvas/CanvasScaler/GUIMap/transl/MapPlayer");
                var dammyMapPlayer = CloneObject(mapPlayer, "DammyMapPlayer");
                AddNameObject(dammyMapPlayer);
                if (dammyMapPlayer == null)
                {
                    Debug.Log("クローンに失敗");
                    return;
                }

                var playerPoint = dammyMapPlayer.GetComponent<GUIMapPlayerPoint>();
                playerPoint.myPlayer = MyNetworkPlayer;
                DammyMapPlayer = dammyMapPlayer;
                DammyMapPlayerPoint = playerPoint;
            }

            /// <summary>
            /// GameObjectをコピー
            /// </summary>
            /// <param name="origin">元オブジェクト</param>
            /// <param name="copyName">コピーオブジェクト名</param>
            static GameObject CloneObject(GameObject origin, string copyName)
            {
                if(origin == null || copyName == null || copyName == "") { return null; }

                var copy = GameObject.Instantiate(origin);
                var originTrans = origin.transform;
                var copyTrans = copy.transform;
                
                copy.name = copyName;
                copyTrans.SetParent(originTrans.parent);
                copyTrans.localPosition = originTrans.localPosition;
                copyTrans.localScale = originTrans.localScale;

                return copy;
            }

            /// <summary>
            /// 名前をテキスト表示
            /// </summary>
            /// <remarks>ImageコンポーネントとTextコンポーネントは共存できないのでサブオブジェクトを追加してTextを表示する</remarks>
            /// <param name="object"></param>
            static void AddNameObject(GameObject parent)
            {
                var textObject = new GameObject();
                textObject.name = "UserName";
                var trans = textObject.transform;
                trans.SetParent(parent.transform, false);

                var text = textObject.AddComponent<Text>();
                text.text = MyNetworkPlayer.name;

                var font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.font = font;
                text.fontSize = 20;
                text.color = Color.blue;
            }

            /// <summary>
            /// アイコンを表示
            /// </summary>
            /// <remarks>これを実行すると紐づくユーザ情報にしたがってアイコンの位置が自動調整される</remarks>
            static void UpdateDammyMapPlayer()
            {
                if (DammyMapPlayerPoint == null) { return; }

                DammyMapPlayerPoint.OnShow();
            }
        }
    }
}