using MelonLoader;
using HarmonyLib;
using UnityEngine;
using UI;
using System;
using HoboToughLifeMods.Controller;

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
            private static MapPlayerController MapPlayerController = new MapPlayerController();
            private static GameObject SelfPlayerObject = null;
            private static GameObject RootObject = null;

            static void Postfix(GUIMapManager __instance, bool hasMap)
            {
                AddSelfPlayer();
                AddMultiplayerObjects();

                UpdateMapPlayer();
            }

            /// <summary>
            /// 自キャラのゲームオブジェクトを取得
            /// </summary>
            private static void AddSelfPlayer()
            {
                if(SelfPlayerObject != null) { return; }

                SelfPlayerObject = GameObject.FindObjectOfType<Game.PlayerManager>().gameObject;
                MapPlayerController.AddPlayer(SelfPlayerObject);
            }

            /// <summary>
            /// マルチプレイヤーのゲームオブジェクトを取得
            /// </summary>
            private static void AddMultiplayerObjects()
            {
                var players = GameObject.FindObjectsOfType<Game.PlayerClientManager>();
                foreach (var player in players) { MapPlayerController.AddPlayer(player.transform.gameObject); }
            }

            /// <summary>
            /// アイコンを表示
            /// </summary>
            /// <remarks>これを実行すると紐づくユーザ情報にしたがってアイコンの位置が自動調整される</remarks>
            static void UpdateMapPlayer()
            {
                MapPlayerController.UpdateMapPlayer();
            }
        }
    }
}