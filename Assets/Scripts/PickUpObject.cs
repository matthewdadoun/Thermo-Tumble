using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PickUpObject : MonoBehaviour, IGrabbable
{
    // The rigid body of this object
    private Rigidbody _rb;

    // The force amount to apply when throwing the object
    [SerializeField] private float throwForce = 100f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnGrab(Transform holder)
    {
        _rb.isKinematic = true;
        holder.SetParent(holder, worldPositionStays: false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        gameObject.layer = LayerMask.NameToLayer($"HeldItem");
    }

    public void OnThrow(Vector3 throwDirection)
    {
        transform.SetParent(null, true);
        _rb.AddForce(throwDirection * throwForce);
    }
}