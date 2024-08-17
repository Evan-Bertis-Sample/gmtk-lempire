using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Curly.GOAP
{
    public class GOAPBelief
    {
        public string Name { get; }
        public bool Evaluate() => _condition();

        private Func<bool> _condition;

        private GOAPBelief(string name)
        {
            Name = name;
        }

        // override the hash function so that the name is the only thing that matters
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public class Builder {
            private GOAPBelief _belief;
            private List<Func<bool>> _conditions = new List<Func<bool>>();

            public Builder(string beliefName)
            {
                _belief = new GOAPBelief(beliefName);
            }

            public void WithCondition(Func<bool> condition)
            {
                _conditions.Add(condition);
            }

            public GOAPBelief Build()
            {
                _belief._condition = () => {
                    foreach (var condition in _conditions)
                    {
                        if (!condition())
                        {
                            return false;
                        }
                    }
                    return true;
                };
                return _belief;
            }
        }

    }
}
