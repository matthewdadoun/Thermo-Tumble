using UnityEngine;

public class DestructibleObject : MonoBehaviour, IDestructible
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnDestructibleOverlap(GameObject destroyer)
    {
        // If destroyer is valid
        if (destroyer)
        {
            // Destroy this game object
            Destroy(gameObject);
        }
    }
}