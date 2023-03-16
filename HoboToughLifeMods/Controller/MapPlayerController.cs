using HoboToughLifeMods.Behaviour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TMPro;
using UnityEngine;

namespace HoboToughLifeMods.Controller
{
    internal class MapPlayerController 
    {
        private List<MapPlayerBehaviour> MapPlayers = new List<MapPlayerBehaviour>();

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

            var model = CreateMapPlayerObject(playerObject);
            if(model != null) { MapPlayers.Add(model); }

            return true;
        }

        /// <summary>
        /// プレイヤーアイコンを管理するGameObjectの生成
        /// </summary>
        /// <param name="playerObject">紐づけるプレイヤー情報</param>
        private MapPlayerBehaviour CreateMapPlayerObject(GameObject playerObject)
        {
            // 既存のアイコンを流用
            var originMapObject = GameObject.Find("GUI/GUICanvas/CanvasScaler/GUIMap/transl/MapPlayer");
            var copiedMapObject = CloneObject(originMapObject, playerObject.name);
            if (originMapObject == null)
            {
                Debug.LogError("マップアイコンの作成に失敗");
                return null;
            }

            AddNameObject(copiedMapObject, playerObject.name);
            
            var controller  = copiedMapObject.AddComponent<MapPlayerBehaviour>();
            controller.Init(playerObject);

            return controller;
        }

        /// <summary>
        /// GameObjectをコピー
        /// </summary>
        /// <param name="origin">元オブジェクト</param>
        /// <param name="copyName">コピーオブジェクト名</param>
        private GameObject CloneObject(GameObject origin, string copyName)
        {
            if (origin == null || copyName == null || copyName == "") { return null; }

            var copy = GameObject.Instantiate(origin);
            var originTrans = origin.transform;
            var copyTrans = copy.transform;

            copy.name = copyName;
            copyTrans.SetParent(originTrans.parent, false);
            copyTrans.localPosition = Vector3.zero;
            copyTrans.localScale = originTrans.localScale;

            return copy;
        }

        /// <summary>
        /// 名前をテキスト表示
        /// </summary>
        /// <remarks>ImageコンポーネントとTextコンポーネントは共存できないのでサブオブジェクトを追加してTextを表示する</remarks>
        /// <param name="object"></param>
        private void AddNameObject(GameObject parent, string name)
        {
            var textObject = new GameObject();
            textObject.name = "PlayerNameText";
            var trans = textObject.transform;
            trans.SetParent(parent.transform, false);

            var text = textObject.AddComponent<TextMeshProUGUI>();
            text.text = name;

            text.m_width = 100;
            text.alignment = TextAlignmentOptions.Top;
            text.fontSize = 20;
            text.fontStyle = FontStyles.Bold;
            text.color = Color.black;
            text.enableWordWrapping = false;
        }



        /// <summary>
        /// アイコン位置を更新して表示
        /// </summary>
        public void UpdateMapPlayer()
        {
            MapPlayers.RemoveAll(p => !p.Exists);
        }
    }
}
