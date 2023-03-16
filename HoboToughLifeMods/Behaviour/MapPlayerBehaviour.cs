using Core.Net;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UI;
using UnityEngine;

namespace HoboToughLifeMods.Behaviour
{
    [RegisterTypeInIl2Cpp]
    internal class MapPlayerBehaviour : MonoBehaviour
    {
        /// <summary>
        /// 初期化が完了したか
        /// </summary>
        private bool IsInitEnd = false;
        /// <summary>
        /// プレイヤー情報
        /// </summary>
        private GameObject PlayerObject { get; set; }
        /// <summary>
        /// プレイヤーがゲーム内に存在するか
        /// </summary>
        public bool Exists => PlayerObject != null;
        /// <summary>
        /// プレイヤー名
        /// </summary>
        public string Name => Exists ? PlayerObject.name : "";

        public MapPlayerBehaviour(IntPtr ptr) : base(ptr) { }

        /// <summary>
        /// 初期化(アイコン表示に必要な情報を作成)
        /// </summary>
        /// <param name="playerObject">プレイヤー情報</param>
        public void Init(GameObject playerObject)
        {
            PlayerObject = playerObject;
            IsInitEnd = true;
        }

        private void Update()
        {
            if (IsInitEnd && !Exists)
            {
                Destroy(gameObject);
                return;
            }

            UpdateMapPlayer();
        }

        /// <summary>
        /// アイコン位置を更新して表示
        /// </summary>
        /// <remarks>紐づくユーザ情報に従ってアイコンの位置や方向が自動調整される</remarks>
        private void UpdateMapPlayer()
        {
            if (!Exists) { return; }

            var gameTrans = PlayerObject.transform;
            var mapTrans = gameObject.transform;
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
            float x = 0.00000630598925f * (current.z * current.z) + -0.976941344f * current.z + 874.85253f;
            float y = -0.000000102722494f * (current.x * current.x) + 0.955787392f * current.x + 1041.58575f;
            float z = -69.75f;
            Vector3 result = new Vector3(x, y, z);
            return result;
        }

        /// <summary>
        /// ゲーム角度からマップ角度に変換
        /// </summary>
        /// <param name="angle">ゲーム上の角度</param>
        private Vector3 ConvertGameToMapAngle(Vector3 angle)
        {
            float converted = (540f - angle.y) % 360f;
            Vector3 result = new Vector3(0, 0, converted);
            return result;
        }
    }
}
