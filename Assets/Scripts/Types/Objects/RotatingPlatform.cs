using System;
using UnityEngine;
using UnityEngine.Serialization;

public class RotatingPlatform : MonoBehaviour
{
    public bool bRotatePlatform = false;
    public bool rotateOppositeDirection = false;
    public float speed = 50.0f;
    private float _initialZ;

    private void Awake()
    {
        if (!bRotatePlatform)
        {
            enabled = false;
            return;
        }

        _initialZ = transform.eulerAngles.z;
    }

    private void Update()
    {
        var newZ = _initialZ + (Time.time * (rotateOppositeDirection ? -speed : speed));
        transform.rotation = Quaternion.Euler(
            transform.eulerAngles.x,
            transform.eulerAngles.y,
            newZ
        );
    }
}