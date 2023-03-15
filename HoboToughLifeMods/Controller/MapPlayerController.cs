using HoboToughLifeMods.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HoboToughLifeMods.Controller
{
    internal class MapPlayerController 
    {
        private List<MapPlayerModel> MapPlayers = new List<MapPlayerModel>();

        /// <summary>
        /// アイコンを追加
        /// </summary>
        /// <param name="playerObject">プレイヤー情報</param>
        /// <returns>追加したか</returns>
        public bool AddPlayer(GameObject playerObject)
        {
            if (playerObject == null) { throw new ArgumentNullException(); }

            bool hasPlayerObject = MapPlayers.Select(p => p.Name).Contains(playerObject.name);
            if (hasPlayerObject) { return false; }

            var model = new MapPlayerModel(playerObject);
            if(model != null) { MapPlayers.Add(model); }

            return true;
        }

        /// <summary>
        /// アイコン位置を更新して表示
        /// </summary>
        public void UpdateMapPlayer()
        {
            foreach (var player in MapPlayers) {
                if (player.Exists) { player.UpdateMapPlayer(); }
                else { player.Dispose(); }
            }

            MapPlayers.RemoveAll(p => !p.Exists);
        }
    }
}
