using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeControlTest : MonoBehaviour
{
    public Transform target;
    public RopeAnimation rope;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
		{
            rope.Attatch(transform, target);
		}
        if(Input.GetKeyUp(KeyCode.Space))
		{
            rope.Deattach();
		}
    }
}
