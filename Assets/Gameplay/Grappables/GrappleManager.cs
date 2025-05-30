using System.Collections;
using System.Collections.Generic;
using Gameplay.Player;
using JetBrains.Annotations;
using UnityEngine;

namespace Gameplay.Grappables
{
    public class GrappleManager : MonoBehaviour, IEnumerable
    {
        private readonly List<Grappable> _grappables = new();

        public IEnumerator GetEnumerator()
        {
            return _grappables.GetEnumerator();
        }

        public void AddGrappable(Grappable grappable)
        {
            _grappables.Add(grappable);
        }

        [CanBeNull]
        public Grappable FindClosestGrapplePoint(Vector2 position)
        {
            Grappable best = null;
            var minDist = float.MaxValue;

            foreach (var grappable in _grappables)
            {
                var dist = Vector2.Distance(position, grappable.Position2D);

                if (!(dist < minDist)) continue;

                best = grappable;
                minDist = dist;
            }

            return best;
        }

        [CanBeNull]
        public Grappable FindClosestGrappablePoint(Vector2 position, SwingBody swingBody)
        {
            Grappable best = null;
            var minDist = float.MaxValue;

            foreach (var grappable in _grappables)
            {
                if (!swingBody.CanGrapple(grappable)) continue;

                var dist = Vector2.Distance(position, grappable.Position2D);

                if (!(dist < minDist)) continue;

                best = grappable;
                minDist = dist;
            }

            return best;
        }
    }
}