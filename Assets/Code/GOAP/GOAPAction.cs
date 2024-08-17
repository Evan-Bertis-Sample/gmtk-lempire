using System.Collections.Generic;
using System.Linq;

namespace Curly.GOAP
{
    public class GOAPAction
    {
        public string Name { get; }
        private GOAPEvaluator _costEvalulator;
        public float CostValue => (_costEvalulator != null) ? _costEvalulator.Evaluate() : 0;

        public HashSet<GOAPBelief> Preconditions { get; } = new HashSet<GOAPBelief>();
        public HashSet<GOAPBelief> Effects { get; } = new HashSet<GOAPBelief>();

        IGOAPActionStrategy _strategy;

        public bool CanPerform => _strategy.CanPerform && Preconditions.All(b => b.Evaluate());
        public bool Complete => _strategy.Complete;

        public void Start() => _strategy.Start();

        // override the hash function so that the name is the only thing that matters
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public void Update(float deltaTime)
        {
            if (CanPerform)
            {
                _strategy.Update(deltaTime);
            }

            if (Complete)
            {
                foreach (var effect in Effects)
                {
                    effect.Evaluate();
                }
            }
        }

        public void Finish() => _strategy.Finish();

        private GOAPAction(string name, GOAPEvaluator cost = null)
        {
            Name = name;
            _costEvalulator = cost;
        }

        public class Builder {
            private GOAPAction _action;

            public Builder(string actionName)
            {
                _action = new GOAPAction(actionName);
            }

            public Builder WithCost(GOAPEvaluator cost)
            {
                _action._costEvalulator = cost;
                return this;
            }

            public Builder WithPrecondition(GOAPBelief precondition)
            {
                _action.Preconditions.Add(precondition);
                return this;
            }

            public Builder WithEffect(GOAPBelief effect)
            {
                _action.Effects.Add(effect);
                return this;
            }

            public Builder WithStrategy(IGOAPActionStrategy strategy)
            {
                _action._strategy = strategy;
                return this;
            }

            public GOAPAction Build()
            {
                return _action;
            }
        }
    }
}