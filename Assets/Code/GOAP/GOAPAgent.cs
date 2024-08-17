using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Curly.GOAP
{
    public class GOAPAgent
    {
        public Dictionary<string, GOAPBelief> Beliefs { get; } = new Dictionary<string, GOAPBelief>();
        public HashSet<GOAPGoal> Goals { get; } = new HashSet<GOAPGoal>();
        public HashSet<GOAPAction> Actions { get; } = new HashSet<GOAPAction>();
    }

    public class GOAPAgentBuilderAttribute : System.Attribute
    {
        public string AgentType { get; }

        public GOAPAgentBuilderAttribute(string agentType)
        {
            AgentType = agentType;
        }
    }

    public class GOAPAgentBuilderPathAttribute : PropertyAttribute
    {
        public GOAPAgentBuilderPathAttribute()
        {
        }
    }

    public interface IGOAPAgentBuilder
    {
        GOAPAgent BuildAgent();
        List<Type> RequiredComponents { get; }
    }

    public class GOAPAgentBuilderRegistry
    {
        private static Dictionary<string, IGOAPAgentBuilder> _builders = new Dictionary<string, IGOAPAgentBuilder>();

        static GOAPAgentBuilderRegistry()
        {
            FindAgentBuilders();
        }

        private static void FindAgentBuilders()
        {
            _builders.Clear();
            // get all types in the project, and find the ones with the GOAPAgentBuilder attribute
            System.Type[] types = System.AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .ToArray();
            
            foreach (System.Type type in types)
            {
                if (type.GetCustomAttributes(typeof(GOAPAgentBuilderAttribute), true).Length > 0)
                {
                    GOAPAgentBuilderAttribute attribute = (GOAPAgentBuilderAttribute)type.GetCustomAttributes(typeof(GOAPAgentBuilderAttribute), true)[0];

                    Debug.Log($"Found agent builder {attribute.AgentType} at {type}");
                    _builders[attribute.AgentType] = (IGOAPAgentBuilder)System.Activator.CreateInstance(type);
                }
            }
        }

        public static IGOAPAgentBuilder GetBuilder(string agentName)
        {
            return _builders[agentName];
        }

        public static List<string> GetAgentBuilderNames()
        {
            if (_builders == null)
            {
                FindAgentBuilders();
            }

            return new List<string>(_builders.Keys);
        }

        public static bool IsValidBuilder(string agentName)
        {
            if (agentName == null)
            {
                return false;
            }

            if (_builders == null)
            {
                FindAgentBuilders();
            }
            
            return _builders.ContainsKey(agentName);
        }
    }
}
