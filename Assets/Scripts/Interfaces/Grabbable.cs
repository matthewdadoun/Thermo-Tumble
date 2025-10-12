using UnityEngine;

public interface IGrabbable
{
    void OnGrab(Transform holder);
    void OnThrow(Vector3 direction);
}
