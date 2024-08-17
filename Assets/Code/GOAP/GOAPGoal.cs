using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Curly.GOAP
{
    public class GOAPGoal
    {
        public string Name { get; }
        private GOAPEvaluator _priorityEvaluator;
        public float PriorityValue => (_priorityEvaluator != null) ? _priorityEvaluator.Evaluate() : 0;
        public HashSet<GOAPBelief> DesiredEffects { get; } = new HashSet<GOAPBelief>();

        private GOAPGoal(string name, GOAPEvaluator priority = null)
        {
            Name = name;
            _priorityEvaluator = priority;
        }

        // override the hash function so that the name is the only thing that matters
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public class Builder {
            private GOAPGoal _goal;

            public Builder(string goalName)
            {
                _goal = new GOAPGoal(goalName);
            }

            public Builder WithPriority(GOAPEvaluator priority)
            {
                _goal._priorityEvaluator = priority;
                return this;
            }

            public Builder WithDesiredEffect(GOAPBelief desiredBelief)
            {
                _goal.DesiredEffects.Add(desiredBelief);
                return this;
            }

            public GOAPGoal Build()
            {
                return _goal;
            }
        }


    }
}
