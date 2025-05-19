using UnityEngine;

namespace Grappables
{
	[RequireComponent(typeof(Rigidbody2D))]
	public abstract class Grappable : MonoBehaviour
	{
		private new Rigidbody2D _rigidbody;
		private GrappleManager _grappleManager;

		public Vector2 Position2D => new Vector2(transform.position.x, transform.position.y);

		protected virtual void Awake()
		{
			_rigidbody = GetComponent<Rigidbody2D>();
		}

		protected virtual void Start()
		{
			_grappleManager = FindObjectOfType<GrappleManager>();
			_grappleManager.AddGrappable(this);
		}

		public void Grab(Joint2D joint)
		{
			joint.connectedBody = _rigidbody;
			joint.enabled = true;

			OnGrab();
		}

		public void Release(Joint2D joint)
		{
			joint.enabled = false;

			OnRelease();
		}

		protected virtual void OnGrab() { }

		protected virtual void OnRelease() { }
	}
}
