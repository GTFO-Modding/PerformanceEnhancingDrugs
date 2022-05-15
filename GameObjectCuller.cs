using CullingSystem;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PerformanceEnhancingDrugs
{
    public class GameObjectCuller
    {
        public GameObjectCuller(GameObject gameObject, LG_Area area)
        {
            if (area == null || gameObject == null)
            {
                Patch.s_GameObjectCullers.Remove(this);
                return;
            }

            m_GameObject = gameObject;
            m_Area = area;

            Patch.a_Cull += Cull;
        }
        public void Cull(LG_Area area, bool status) 
        {
            if (area != m_Area) return;

            m_GameObject.SetActive(status);
        }

        public GameObject m_GameObject;
        public LG_Area m_Area;
    }
}
