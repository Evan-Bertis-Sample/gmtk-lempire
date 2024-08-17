using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Curly.Grid
{
    public abstract class GridEntityMovementAnimation : ScriptableObject
    {
        protected abstract IGridEntityMovementAnimation GetAnimationStrategy();

        public void Play(Transform transform, Vector3 from, Vector3 to, float duration)
        {
            GetAnimationStrategy().PlayMovementAnimation(transform, from, to, duration);
        }
    }

    public interface IGridEntityMovementAnimation
    {
        void PlayMovementAnimation(Transform transform, Vector3 from, Vector3 to, float duration);
        void StopMovementAnimation();
    }
}