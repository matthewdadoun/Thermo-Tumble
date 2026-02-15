using UnityEngine;

public interface IGrabbable
{
    // What to do when the player grabs this object
    void OnGrab(Transform holder);
    
    // What to do using this grabbable as the "propel"
    void OnPropel(Transform holder);
    
    // What to do when the player throws this object
    void OnThrow(Vector3 direction);
}
