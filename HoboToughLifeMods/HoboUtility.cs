using MelonLoader;
using HarmonyLib;
using UnityEngine;
using Game;
using UI;
using System;

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

        [HarmonyPatch(typeof(GUIMapManager), "Show", new Type[] { typeof(bool) })]
        static class GUIMapManager_Hook
        {
            static void Postfix(GUIMapManager __instance, bool hasMap)
            {
                var guiMapObject = GameObject.Find("GUI/GUICanvas/CanvasScaler/GUIMap/transl");
                Debug.Log($"A: {guiMapObject.transform}");
                
                // Good
                foreach (var trans in guiMapObject.transform)
                {
                    var casted = trans.TryCast<Transform>();
                    Debug.Log($"B: {casted.gameObject.name}");
                }

                // Bad
                //[ERROR] Exception in Harmony patch of method void UI.GUIMapManager::Show(bool hasMap):
                //System.InvalidCastException: Specified cast is not valid.
                //  at HoboToughLifeMods.HoboUtility + GUIMapManager_Hook.Postfix(UI.GUIMapManager __instance, System.Boolean __0)[0x0002d] in < 2cecffa515584304a7faeb001594e823 >:0
                //  at(wrapper dynamic - method) UI.GUIMapManager.DMD<UI.GUIMapManager::Show>(UI.GUIMapManager, bool)
                //  at(wrapper dynamic - method) MonoMod.Utils.DynamicMethodDefinition.DMD<UI.GUIMapManager::Show> _il2cpp(intptr, bool)
                //foreach (Transform trans in guiMapObject.transform)
                //{
                //    var casted = trans.TryCast<Transform>();
                //    Debug.Log($"B: {casted.gameObject.name}");
                //}


                var objects = GameObject.FindObjectsOfType<GameObject>();
                foreach (var @object in objects)
                {
                    bool isRootObject = @object.transform.parent == null;
                    if (!isRootObject)
                    {
                        continue;
                    }
                    bool isPlayer = @object.GetComponent<PlayerManager>() != null;
                    if (!isPlayer)
                    {
                        continue;
                    }

                    Debug.Log($"C: {@object.name}");
                }
            }
        }
    }
}