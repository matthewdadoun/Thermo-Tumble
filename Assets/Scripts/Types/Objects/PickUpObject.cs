using UnityEngine; /**/
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PickUpObject : MonoBehaviour, IGrabbable
{
    // The rigid body of this object
    private Rigidbody _rb;
    private Collider _col;

    // The force amount to apply when throwing the object
    [SerializeField] private float throwVerticalForce = 100f;
    [SerializeField] private float throwHorizontalForce = 100f;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();
    }

    /*private void OnTriggerEnter(Collider other)
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
    }*/

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
        // Disable physics upon pickup
        EnablePhysics(false);

        // Set transform parent to be the holder
        transform.SetParent(holder, worldPositionStays: false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        /*gameObject.layer = LayerMask.NameToLayer($"HeldItem");*/
    }

    public void OnPropel(Transform holder)
    {
        EnablePhysics(true);
        
        // Set position to be holder's position
        transform.position = holder.position;
        
        transform.SetParent(null, true);
    }

    private void EnablePhysics(bool bPhysics)
    {
        // Set whether rigid body is kinematic and gravity enabled
        _rb.isKinematic = !bPhysics;
        _rb.useGravity = bPhysics;

        // Collision is only enabled if physics are enabled
        _col.enabled = bPhysics;
    }

    public void OnThrow(Vector3 throwDirection)
    {
        // Enable physics upon throwing
        EnablePhysics(true);
        
        transform.SetParent(null, true);
        _rb.AddForce((throwDirection * throwHorizontalForce) + (Vector3.up * throwVerticalForce));
    }
}