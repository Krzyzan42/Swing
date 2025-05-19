using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBlade : MonoBehaviour
{
    public float speed = 1f;


    void Update()
    {
        float zRot = transform.rotation.eulerAngles.z;
        transform.rotation = Quaternion.Euler(0, 0, zRot + speed * 360 * Time.deltaTime);
    }
}
