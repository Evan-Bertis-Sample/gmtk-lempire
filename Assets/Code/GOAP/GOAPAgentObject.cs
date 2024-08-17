using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Curly.GOAP
{
    public class GOAPAgentObject : MonoBehaviour
    {
        public enum GOAPAgentStatus
        {
            UNINITIALIZED,
            INITIALIZED,
            ERROR,
            RUNNING,
            PAUSED,
            NUM_STATES
        }

        [SerializeField, GOAPAgentBuilderPath] public string AgentPath;
        public GOAPAgentStatus Status { get; private set; } = GOAPAgentStatus.UNINITIALIZED;
        private GOAPAgent _agent;

        private void Awake()
        {
            Status = GOAPAgentStatus.UNINITIALIZED;
            MeetsRequiredComponents(out List<System.Type> missingComponents);
            if (missingComponents.Count > 0)
            {
                Debug.LogError($"Agent missing required components: {string.Join(", ", missingComponents)}");
                Status = GOAPAgentStatus.ERROR;
                return;
            }

            if (_agent == null)
            {
                Debug.LogError($"Failed to build agent from path {AgentPath}");
                Status = GOAPAgentStatus.ERROR;
            }

            Status = GOAPAgentStatus.INITIALIZED;
        }

        public bool MeetsRequiredComponents(out List<System.Type> missingComponents)
        {
            missingComponents = new List<System.Type>();
            IGOAPAgentBuilder builder = GOAPAgentBuilderRegistry.GetBuilder(AgentPath);
            List<System.Type> requiredComponents = builder.RequiredComponents;
            foreach (System.Type component in requiredComponents)
            {
                // check if we have the required component in the list of components,
                // either in this game object or in any of its children
                if (GetComponent(component) == null && GetComponentsInChildren(component).Length == 0)
                {
                    missingComponents.Add(component);
                }
            }

            return missingComponents.Count == 0;
        }

        private void Update()
        {
            if (Status == GOAPAgentStatus.UNINITIALIZED || Status == GOAPAgentStatus.ERROR)
            {
                Debug.LogError("Agent not initialized or in error state");
                return;
            }

            if (Status == GOAPAgentStatus.PAUSED)
            {
                return;
            }


        }
    }
}