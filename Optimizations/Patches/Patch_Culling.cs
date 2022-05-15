using ChainedPuzzles;
using CullingSystem;
using Enemies;
using HarmonyLib;
using IRF;
using LevelGeneration;
using Player;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace PerformanceEnhancingDrugs
{
    [HarmonyPatch]
    public class Patch_Culling
    {

        [HarmonyPostfix]
        [HarmonyPatch(typeof(C_Node), "Hide")]
        public static void Hide(C_Node __instance)
        {
            a_Cull?.Invoke(__instance.m_owner.TryCast<LG_Area>(), false);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(C_Node), "Show")]
        public static void Show(C_Node __instance)
        {
            a_Cull?.Invoke(__instance.m_owner.TryCast<LG_Area>(), true);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GS_AfterLevel), "Enter")]
        public static void LevelCleanup()
        {
            s_BehaviourCullers.Clear();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerAgent), "TryWarpTo")]
        public static void TryWarpTo(eDimensionIndex dimensionIndex)
        {
            LG_LevelBuilder.Current.m_currentFloor.GetDimension(dimensionIndex, out var dimension);
            PreLitVolume.Current.gameObject.GetComponent<AmbientParticles>().enabled = (dimension.DimensionData.DustColor.a != 0);
        }

        public static void OnFactoryBuildDone()
        {
            var levelGO = LG_LevelBuilder.Current.m_currentFloor.gameObject;

            foreach (var irf in levelGO.GetComponentsInChildren<InstancedRenderFeature>()) s_BehaviourCullers.Add(new BehaviourCuller(irf, GetParentArea(irf)));
            foreach (var pingTarget in levelGO.GetComponentsInChildren<PlayerPingTarget>()) s_BehaviourCullers.Add(new BehaviourCuller(pingTarget, GetParentArea(pingTarget)));
            foreach (var mapLookAt in levelGO.GetComponentsInChildren<LG_MapLookatRevealerBase>()) s_BehaviourCullers.Add(new BehaviourCuller(mapLookAt, GetParentArea(mapLookAt)));
            foreach (var soundEmitter in levelGO.GetComponentsInChildren<CellSoundEmitter>()) s_BehaviourCullers.Add(new BehaviourCuller(soundEmitter, GetParentArea(soundEmitter)));
            foreach (var interaction in levelGO.GetComponentsInChildren<Interact_Base>()) s_BehaviourCullers.Add(new BehaviourCuller(interaction, GetParentArea(interaction)));
            foreach (var feedbackPlayer in levelGO.GetComponentsInChildren<LG_EnvironmentFeedbackPlayer>()) s_BehaviourCullers.Add(new BehaviourCuller(feedbackPlayer, GetParentArea(feedbackPlayer)));
            foreach (var blinkingLight in levelGO.GetComponentsInChildren<BlinkingLight>()) s_BehaviourCullers.Add(new BehaviourCuller(blinkingLight, GetParentArea(blinkingLight)));
            foreach (var textMeshPro in levelGO.GetComponentsInChildren<TextMeshPro>()) s_BehaviourCullers.Add(new BehaviourCuller(textMeshPro, GetParentArea(textMeshPro)));

            foreach (var culler in s_BehaviourCullers) culler.m_CullableBehaviour.enabled = false;
            L.Debug($"Added {s_BehaviourCullers.Count} MonoBehaviours to collection for improved culling.");



            foreach (var light in levelGO.GetComponentsInChildren<LG_Light>()) s_GameObjectCullers.Add(new GameObjectCuller(light.gameObject, GetParentArea(light)));
            foreach (var clLight in levelGO.GetComponentsInChildren<CL_Light>()) s_GameObjectCullers.Add(new GameObjectCuller(clLight.gameObject, GetParentArea(clLight)));

            foreach (var culler in s_GameObjectCullers) culler.m_GameObject.SetActive(false);
            L.Debug($"Added {s_GameObjectCullers.Count} GameObjects to collection for improved culling.");



            if (RundownManager.ActiveExpedition.Expedition.DustColor.a == 0)
            {
                L.Debug("Ambient particle alpha is 0. Disabled for to improve performance");
                PreLitVolume.Current.gameObject.GetComponent<AmbientParticles>().enabled = false;
            }
        }

        public static LG_Area GetParentArea(MonoBehaviour behaviour)
        {
            return behaviour.GetComponentInParent<LG_Area>();
        }

        public static List<BehaviourCuller> s_BehaviourCullers = new();
        public static List<GameObjectCuller> s_GameObjectCullers = new();
        public static event Action<LG_Area, bool> a_Cull;
    }
}
