using System;
using System.Collections.Generic;
using UnityEngine;

namespace Curly.GOAP
{
    public class GOAPEvaluator
    {
        public static GOAPEvaluator Static(float value)
        {
            return new GOAPEvaluator(() => value);
        }

        public static GOAPEvaluator Distance(Transform transform, Transform target)
        {
            return new GOAPEvaluator(() => Vector3.Distance(transform.position, target.position));
        }


        private GOAPEvaluator(Func<float> evaluationFunction)
        {
            _evaluationFunction = evaluationFunction;
        }

        private Func<float> _evaluationFunction;

        public float Evaluate()
        {
            return _evaluationFunction();
        }

        public class Builder
        {
            private List<GOAPEvaluator> _evaluators = new List<GOAPEvaluator>();

            public Builder Add(GOAPEvaluator evaluator, float weight = 1)
            {
                _evaluators.Add(new GOAPEvaluator(() => evaluator.Evaluate() * weight));
                return this;
            }

            public GOAPEvaluator Build()
            {
                return new GOAPEvaluator(() =>
                {
                    float sum = 0;
                    foreach (var evaluator in _evaluators)
                    {
                        sum += evaluator.Evaluate();
                    }
                    return sum;
                });
            }
        }
    }
}