using MelonLoader;
using HarmonyLib;
using UnityEngine;
using Game;
using UI;

namespace HoboToughLifeMods
{
    public class HoboUtility : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Hello HoboUtility");
            MelonEvents.OnGUI.Subscribe(DrawMenu, 100);
        }

        private void DrawMenu()
        {
            GUI.Box(new Rect(0, 0, 300, 500), "My Menu");
        }

        [HarmonyPatch(typeof(GUIMapManager), "Show")]
        static class GUIMapManager_Hook
        {
            static void Postfix(GUIMapManager __instance, bool __0)
            {
                var guiMapObject = GameObject.Find("GUI/GUICanvas/CanvasScaler/GUIMap/transl");
                Debug.Log(guiMapObject.transform);
                foreach (Transform trans in guiMapObject.transform)
                {
                    Debug.Log(trans.gameObject.name);
                }

                var objects = UnityEngine.GameObject.FindObjectsOfType<UnityEngine.GameObject>();
                foreach (var @object in objects)
                {
                    bool isRootObject = @object.transform.parent == null;
                    if (!isRootObject)
                    {
                        continue;
                    }
                    bool isPlayer = @object.GetComponent<Game.PlayerManager>() != null;
                    if (!isPlayer)
                    {
                        continue;
                    }

                    Debug.Log(@object.name);
                }
            }
        }
    }
}