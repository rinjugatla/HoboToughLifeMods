using Core.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UI;
using UnityEngine;

namespace HoboToughLifeMods.Model
{
    internal class MapPlayerModel : IDisposable
    {
        /// <summary>
        /// プレイヤー情報
        /// </summary>
        public UniversalNetworkPlayer NetworkPlayer { get; private set; }
        /// <summary>
        /// マップ上に表示するアイコン等を保持するオブジェクト
        /// </summary>
        public GameObject MapPlayerObject { get; private set; }
        /// <summary>
        /// マップアイコンを更新するクラス
        /// </summary>
        public GUIMapPlayerPoint MapPlayerPoint { get; private set; }
        /// <summary>
        /// プレイヤーがゲーム内に存在するか
        /// </summary>
        public bool Exists => NetworkPlayer != null;
        /// <summary>
        /// プレイヤー名
        /// </summary>
        public string Name => NetworkPlayer ? NetworkPlayer.name : "";

        /// <summary>
        /// 初期化(アイコン表示に必要な情報を作成)
        /// </summary>
        /// <param name="networkPlayer">プレイヤー情報(UniversalNetworkPlayerを含む)</param>
        public MapPlayerModel(UniversalNetworkPlayer networkPlayer)
        {
            NetworkPlayer = networkPlayer;

            CreateDammyMapPlayerObject();
        }

        /// <summary>
        /// マップに表示するアイコンを作成
        /// </summary>
        private void CreateDammyMapPlayerObject()
        {
            if (!Exists) { return; }
            bool hasCreated = MapPlayerObject != null;
            if (hasCreated) { return; }

            // 既存のアイコンを流用
            var originMapObject = GameObject.Find("GUI/GUICanvas/CanvasScaler/GUIMap/transl/MapPlayer");
            var copiedMapObject = CloneObject(originMapObject, Name);
            if (originMapObject == null)
            {
                Debug.LogError("マップアイコンの作成に失敗");
                return;
            }

            AddNameObject(copiedMapObject);

            var playerPoint = copiedMapObject.GetComponent<GUIMapPlayerPoint>();
            playerPoint.myPlayer = NetworkPlayer;
            MapPlayerObject = copiedMapObject;
            MapPlayerPoint = playerPoint;
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
        private void AddNameObject(GameObject parent)
        {
            var textObject = new GameObject();
            textObject.name = "PlayerNameText";
            var trans = textObject.transform;
            trans.SetParent(parent.transform, false);

            var text = textObject.AddComponent<TextMeshProUGUI>();
            text.text = Name;

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
        /// <remarks>紐づくユーザ情報に従ってアイコンの位置や方向が自動調整される</remarks>
        public void UpdateMapPlayer()
        {
            if (!Exists) { return; }

            MapPlayerPoint.OnShow();
        }

        /// <summary>
        /// 関連オブジェクトの破棄
        /// </summary>
        public void Dispose()
        {
            GameObject.Destroy(MapPlayerObject);
        }
    }
}
