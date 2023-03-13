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
using TMPro;
using System.Collections.Generic;
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

            static void Postfix(GUIMapManager __instance, bool hasMap)
            {
                AddPlayers();
                UpdateMapPlayer();
            }

            private static void AddPlayers()
            {
                // すべてのオブジェクトから探索するのは無駄なので追加や削除があったときのみ行うよう変更したほうがいい
                var networkPlayers = GameObject.FindObjectsOfType<UniversalNetworkPlayer>();
                if (networkPlayers == null) { return; }

                foreach (var player in networkPlayers) { MapPlayerController.AddPlayer(player); }
                
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