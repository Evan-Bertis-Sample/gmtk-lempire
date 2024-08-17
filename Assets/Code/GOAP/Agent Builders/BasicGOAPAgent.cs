using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Curly.GOAP;
using System;

namespace Curly.GOAP.AgentBuilders
{
    [GOAPAgentBuilder("Basic")]
    public class BasicGOAPAgent : IGOAPAgentBuilder
    {
        public List<Type> RequiredComponents => new List<Type>()
        {
            typeof(Rigidbody2D),
            typeof(Collider2D),
            typeof(SpriteRenderer)
        };

        public GOAPAgent BuildAgent()
        {
            GOAPAgent agent = new GOAPAgent();

            CreateBeliefs(agent);
            CreateGoals(agent);
            CreateActions(agent);

            return agent;
        }

        private void CreateBeliefs(GOAPAgent agent)
        {

        }

        private void CreateGoals(GOAPAgent agent)
        {

        }

        private void CreateActions(GOAPAgent agent)
        {

        }
    }
}
