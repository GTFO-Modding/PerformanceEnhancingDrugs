using CullingSystem;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PerformanceEnhancingDrugs
{
    public class BehaviourCuller
    {
        public BehaviourCuller(MonoBehaviour cullableBehaviour, LG_Area area)
        {
            if (area == null || cullableBehaviour == null)
            {
                Patch.s_BehaviourCullers.Remove(this);
                return;
            }

            m_CullableBehaviour = cullableBehaviour;
            m_Area = area;

            Patch.a_Cull += Cull;
        }
        public void Cull(LG_Area area, bool status) 
        {
            if (area == m_Area) m_CullableBehaviour.enabled = status;
        }

        public MonoBehaviour m_CullableBehaviour;
        public LG_Area m_Area;
    }
}
