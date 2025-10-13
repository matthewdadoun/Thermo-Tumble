using UnityEngine;

public interface IDestructible
{
    // Callback for when this object is destroyed
    void OnDestructibleOverlap(GameObject destroyer);
}