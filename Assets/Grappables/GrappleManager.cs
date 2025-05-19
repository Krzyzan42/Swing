using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleManager : MonoBehaviour, IEnumerable
{
    private List<Grappable> grappables = new List<Grappable>();

    public void AddGrappable(Grappable grappable)
	{
		grappables.Add(grappable);
	}

	public IEnumerator GetEnumerator()
	{
		return grappables.GetEnumerator();
	}

	public Grappable FindClosestGrapplePoint(Vector2 position)
	{
		Grappable best = null;
		float minDist = float.MaxValue;

		foreach (var grapable in grappables)
		{
			float dist = Vector2.Distance(position, grapable.position2D);
			if(dist < minDist)
			{
				best = grapable;
				minDist = dist;
			}
		}

		return best;
	}
}
