using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Reset))]
public class Flag : MonoBehaviour, IResetable
{
    private BoxCollider2D boxCollider;
	private new SpriteRenderer renderer;

	public void Reset()
	{
		renderer.color = Color.white;
	}

	private void Awake()
	{
		renderer = GetComponent<SpriteRenderer>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.GetComponent<CharacterController>() != null)
		{
			renderer.color = Color.green;
		}
	}
}
