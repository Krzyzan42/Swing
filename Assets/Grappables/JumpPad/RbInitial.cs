using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Reset))]
[RequireComponent(typeof(Rigidbody2D))]
public class RbInitial : MonoBehaviour, IResetable
{
    public Vector2 initalVelocity;

	public void Reset()
	{
        GetComponent<Rigidbody2D>().linearVelocity = initalVelocity;
	}

	// Start is called before the first frame update
	void Start()
    {
        Reset();
    }
}
