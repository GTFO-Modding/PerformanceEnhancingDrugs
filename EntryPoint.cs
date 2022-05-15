using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using LevelGeneration;
using System;

namespace PerformanceEnhancingDrugs
{
    [BepInPlugin("gov.mccad00.PerformanceEnhancingDrugs", "PerformanceEnhancingDrugs", "1.0.0")]
    public class EntryPoint : BasePlugin
    {
        // The method that gets called when BepInEx tries to load our plugin
        public override void Load()
        {
            m_Harmony = new Harmony("gov.mccad00.PerformanceEnhancingDrugs");
            m_Harmony.PatchAll();
            LG_Factory.add_OnFactoryBuildDone((Action)Patch_Culling.OnFactoryBuildDone);
        }

        private Harmony m_Harmony;
    }
}
