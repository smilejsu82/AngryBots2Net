using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using Unity.Mathematics;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject effect;
    public Player Owner { get; private set; } //owner

    void Start()
    {
        
    }

    public void Init(Player owner)
    {
        this.Owner = owner;
        this.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 1000f);
        Destroy(this.gameObject, 3.0f);
    }

    private void OnCollisionEnter(Collision other)
    {
        var contact = other.GetContact(0);
        var obj = Instantiate(this.effect, contact.point, Quaternion.LookRotation(-contact.normal));
        Destroy(obj, 2.0f);
        Destroy(this.gameObject);
    }
}
