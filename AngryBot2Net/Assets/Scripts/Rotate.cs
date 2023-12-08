using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float speed = 10f;
    void Update()
    {
        this.transform.Rotate(Vector3.forward * this.speed * Time.deltaTime);
    }
}
