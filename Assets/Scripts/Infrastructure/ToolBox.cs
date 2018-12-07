using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    public class Toolbox : Singleton<Toolbox>
    {
        // Used to track any global components added at runtime.
        private Dictionary<string, Component> m_Components = new Dictionary<string, Component>();


        // Prevent constructor use.
        protected Toolbox() { }


        private void Awake()
        {
            // Put initialization code here.
        }


        // Define all required global components here. These are hard-codded components
        // that will always be added. Unlike the optional components added at runtime.
        private float m_delta = 3.0f;
        public float GetConnectionDistance() { return m_delta; }

        private float fin_delta = 0.5f;
        public float GetGoalDistance() { return fin_delta; }

        private float m_angleDelta = 1.0f;
        public float GetConnectionAngle() { return m_angleDelta; }

        // The methods below allow us to add global components at runtime.
        // TODO: Convert from string IDs to component types.
        public Component AddGlobalComponent(string componentID, Type component)
        {
            if (m_Components.ContainsKey(componentID))
            {
                Debug.LogWarning("[Toolbox] Global component ID \""
                    + componentID + "\" already exist! Returning that.");
                return GetGlobalComponent(componentID);
            }

            var newComponent = gameObject.AddComponent(component);
            m_Components.Add(componentID, newComponent);
            return newComponent;
        }


        public void RemoveGlobalComponent(string componentID)
        {
            Component component;

            if (m_Components.TryGetValue(componentID, out component))
            {
                Destroy(component);
                m_Components.Remove(componentID);
            }
            else
            {
                Debug.LogWarning("[Toolbox] Trying to remove nonexistent component ID \""
                    + componentID + "\"! Typo?");
            }
        }


        public Component GetGlobalComponent(string componentID)
        {
            Component component;

            if (m_Components.TryGetValue(componentID, out component))
            {
                return component;
            }

            Debug.LogWarning("[Toolbox] Global component ID \""
        + componentID + "\" doesn't exist! Typo?");
            return null;
        }
    }
}