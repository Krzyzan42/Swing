using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Reset))]
public class StartingPlatform : MonoBehaviour, IResetable
{


	public void Reset()
	{
		GetComponent<BoxCollider2D>().enabled = true;
		GetComponent<SpriteRenderer>().enabled = true;
	}

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKey(KeyCode.Space))
		{
			GetComponent<BoxCollider2D>().enabled = false;
			GetComponent<SpriteRenderer>().enabled = false;
		}
    }
}
