using System;
using System.Collections;
using SystemBase;
using UnityEngine;

namespace Systems.Movement
{
    [RequireComponent(typeof(TwoDeeMovementComponent))]
    public class PaperMarioStyleAnimationComponent : GameComponent
    {
        public float AnimationDelay;
        public IDisposable AnimationDisposable { get; set; }

        public IEnumerator Turn(Vector3 from, Vector3 to)
        {
            for (var ft = -.1f; ft <= 1.1f; ft += 0.1f)
            {
                transform.localScale = Vector3.Lerp(from, to, ft);
                yield return new WaitForSeconds(.1f);
            }
        }
    }
}