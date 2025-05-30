using System.Collections;
using UnityEngine;

namespace Gameplay.Player
{
	public class RopeAnimation : MonoBehaviour
	{
		private LineRenderer _line;
		private Coroutine _animation;

		public AnimationCurve startShape;
		public AnimationCurve endShape;
		public AnimationCurve strengthOverTime;
		public int resolution;
		public float strengthScale = 5f;
		public float animationTime = 0.3f;
		public float flightTime = 0.2f;

		private void Awake()
		{
			_line = GetComponent<LineRenderer>();
		}

		public void Attach(Transform start, Transform end)
		{
			//line.positionCount = 2;
			//line.SetPosition(0, start.position);
			//line.SetPosition(1, end.position);
			//line.enabled = true;
			if (_animation != null)
				StopCoroutine(_animation);
			_animation = StartCoroutine(Animate(start,end));
		}

		public void Deattach()
		{
			_line.enabled = false;
			if (_animation != null)
				StopCoroutine(_animation);
		}

		private IEnumerator Animate(Transform start, Transform end)
		{
			float t = 0;

			_line.positionCount = 2;
			_line.enabled = true;

			while(t < 1)
			{
				var start2D = new Vector2(start.position.x, start.position.y);
				var end2D = new Vector2(end.position.x, end.position.y);

				var ropeEnd = Vector2.Lerp(start2D, end2D, t);
				_line.SetPosition(0, start2D);
				_line.SetPosition(1, ropeEnd);

				t += Time.deltaTime / flightTime;
				yield return null;
			}

			t = 0;
			_line.positionCount = resolution;
			while (t < 1)
			{
				var start2D = new Vector2(start.position.x, start.position.y);
				var end2D = new Vector2(end.position.x, end.position.y);
				var perp = Vector2.Perpendicular(end2D - start2D).normalized;

				for (var i = 0; i < resolution; i++)
				{
					var x = 1f * i / resolution;
					var startY = startShape.Evaluate(x) * strengthOverTime.Evaluate(t) * strengthScale;
					var endY = endShape.Evaluate(x) * strengthOverTime.Evaluate(t) * strengthScale;
					var y = Mathf.Lerp(startY, endY, t);

					var pos = Vector2.Lerp(start2D, end2D, x);
					pos += perp * y;

					_line.SetPosition(i, pos);
				}

				t += Time.deltaTime / animationTime;
				yield return null;
			}

			_line.positionCount = 2;
			while (true)
			{
				var start2D = new Vector2(start.position.x, start.position.y);
				var end2D = new Vector2(end.position.x, end.position.y);

				_line.SetPosition(0, start2D);
				_line.SetPosition(1, end2D);
				yield return null;
			}
		}
	}
}
