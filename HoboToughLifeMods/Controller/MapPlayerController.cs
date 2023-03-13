using Core.Net;
using HoboToughLifeMods.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UI;
using UnityEngine;

namespace HoboToughLifeMods.Controller
{
    internal class MapPlayerController 
    {
        private List<MapPlayerModel> MapPlayers = new List<MapPlayerModel>();

        /// <summary>
        /// アイコンを追加
        /// </summary>
        /// <param name="networkPlayer">プレイヤー情報(UniversalNetworkPlayerを含む)</param>
        /// <param name="isSelfPlayer">自キャラか</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>追加したか</returns>
        public bool AddPlayer(UniversalNetworkPlayer networkPlayer)
        {
            if (networkPlayer == null) { throw new ArgumentNullException(); }

            bool hasPlayerObject = MapPlayers.Select(p => p.Name).Contains(networkPlayer.name);
            if (hasPlayerObject) { return false; }

            var model = new MapPlayerModel(networkPlayer);
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
