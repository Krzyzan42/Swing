using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeAnimation : MonoBehaviour
{
	private LineRenderer line;
	private Coroutine animation;

	public AnimationCurve startShape;
	public AnimationCurve endShape;
	public AnimationCurve strengthOverTime;
	public int resolution;
	public float strengthScale = 5f;
	public float animationTime = 0.3f;
	public float flightTime = 0.2f;

	private void Awake()
	{
		line = GetComponent<LineRenderer>();
	}

	public void Attatch(Transform start, Transform end)
	{
		//line.positionCount = 2;
		//line.SetPosition(0, start.position);
		//line.SetPosition(1, end.position);
		//line.enabled = true;
		if (animation != null)
			StopCoroutine(animation);
		animation = StartCoroutine(ropeAnimation(start,end));
	}

	public void Deattach()
	{
		line.enabled = false;
		if (animation != null)
			StopCoroutine(animation);
	}

	IEnumerator ropeAnimation(Transform start, Transform end)
	{
		float t = 0;

		line.positionCount = 2;
		line.enabled = true;

		while(t < 1)
		{
			Vector2 start2D = new Vector2(start.position.x, start.position.y);
			Vector2 end2D = new Vector2(end.position.x, end.position.y);

			Vector2 ropeEnd = Vector2.Lerp(start2D, end2D, t);
			line.SetPosition(0, start2D);
			line.SetPosition(1, ropeEnd);

			t += Time.deltaTime / flightTime;
			yield return null;
		}

		t = 0;
		line.positionCount = resolution;
		while (t < 1)
		{
			Vector2 start2D = new Vector2(start.position.x, start.position.y);
			Vector2 end2D = new Vector2(end.position.x, end.position.y);
			Vector2 perp = Vector2.Perpendicular(end2D - start2D).normalized;

			for (int i = 0; i < resolution; i++)
			{
				float x = 1f * i / resolution;
				float startY = startShape.Evaluate(x) * strengthOverTime.Evaluate(t) * strengthScale;
				float endY = endShape.Evaluate(x) * strengthOverTime.Evaluate(t) * strengthScale;
				float y = Mathf.Lerp(startY, endY, t);

				Vector2 pos = Vector2.Lerp(start2D, end2D, x);
				pos += perp * y;

				line.SetPosition(i, pos);

			}

			t += Time.deltaTime / animationTime;
			yield return null;
		}

		line.positionCount = 2;
		while (true)
		{
			Vector2 start2D = new Vector2(start.position.x, start.position.y);
			Vector2 end2D = new Vector2(end.position.x, end.position.y);

			line.SetPosition(0, start2D);
			line.SetPosition(1, end2D);
			yield return null;
		}
	}
}
