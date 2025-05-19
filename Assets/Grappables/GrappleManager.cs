using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grappables
{
	public class GrappleManager : MonoBehaviour, IEnumerable
	{
		private List<Grappable> _grappables = new List<Grappable>();

		public void AddGrappable(Grappable grappable)
		{
			_grappables.Add(grappable);
		}

		public IEnumerator GetEnumerator()
		{
			return _grappables.GetEnumerator();
		}

		public Grappable FindClosestGrapplePoint(Vector2 position)
		{
			Grappable best = null;
			var minDist = float.MaxValue;

			foreach (var grapable in _grappables)
			{
				var dist = Vector2.Distance(position, grapable.Position2D);
				if(dist < minDist)
				{
					best = grapable;
					minDist = dist;
				}
			}

			return best;
		}
	}
}
