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
        public GameObject PlayerObject { get; private set; }
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
        public bool Exists => PlayerObject != null;
        /// <summary>
        /// プレイヤー名
        /// </summary>
        public string Name => Exists ? PlayerObject.name : "";

        /// <summary>
        /// 初期化(アイコン表示に必要な情報を作成)
        /// </summary>
        /// <param name="playerObject">プレイヤー情報</param>
        public MapPlayerModel(GameObject playerObject)
        {
            PlayerObject = playerObject;

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

            MapPlayerObject = copiedMapObject;
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

            var gameTrans = PlayerObject.transform;
            var mapTrans = MapPlayerObject.transform;
            mapTrans.position = ConvertGameToMapPostion(gameTrans.position);
            mapTrans.eulerAngles = ConvertGameToMapAngle(gameTrans.eulerAngles);
        }

        /// <summary>
        /// ゲーム位置からマップ位置に変換
        /// </summary>
        /// <param name="current">ゲーム上の位置</param>
        private Vector3 ConvertGameToMapPostion(Vector3 current)
        {
            // GameObjectのPositionから最小二乗法で近似式算出
            // 入力のx, y, zと出力のx, y, zが異なるので注意
            double x = 0.00000630598925 * (current.z * current.z) + -0.976941344 * current.z + 874.85253;
            double y = -0.000000102722494 * (current.x * current.x) + 0.955787392 * current.x + 1041.58575;
            double z = -69.75;
            Vector3 result = new Vector3((float)x, (float)y, (float)z);
            return result;
        }

        /// <summary>
        /// ゲーム角度からマップ角度に変換
        /// </summary>
        /// <param name="angle">ゲーム上の角度</param>
        private Vector3 ConvertGameToMapAngle(Vector3 angle)
        {
            double converted = (540.0 - angle.y) % 360.0;
            Vector3 result = new Vector3(0, 0, (float)converted);
            return result;
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
