using UnityEngine;/**/
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PickUpObject : MonoBehaviour, IGrabbable
{
    // The rigid body of this object
    private Rigidbody _rb;
    private Collider _col;

    // The force amount to apply when throwing the object
    [SerializeField] private float throwForce = 100f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Retrieve destructible component
        var destructible = other.gameObject.GetComponent<IDestructible>();
        
        // Perform on destroy interface function
        destructible?.OnDestructibleOverlap(gameObject);

        // If the destructible
        if (destructible != null)
        {
            Destroy(gameObject);
        }
    }

    /*// Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }*/

    public void OnGrab(Transform holder)
    {
        // Set rigid body to be kinematic and disable gravity
        _rb.isKinematic = true;
        _rb.useGravity = false;

        // set collision enabled to be false
        _col.enabled = false;

        // Set transform parent to be the holder
        transform.SetParent(holder, worldPositionStays: false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        /*gameObject.layer = LayerMask.NameToLayer($"HeldItem");*/
    }

    public void OnThrow(Vector3 throwDirection)
    {
        // Set rigid body to be kinematic and disable gravity
        _rb.isKinematic = false;
        _rb.useGravity = true;

        // set collision enabled to be false
        _col.enabled = true;

        transform.SetParent(null, true);
        _rb.AddForce((throwDirection + Vector3.up) * throwForce);
    }
}