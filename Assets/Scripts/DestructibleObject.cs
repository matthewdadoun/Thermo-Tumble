using UnityEngine;

public class DestructibleObject : ElementalBehaviour, IDestructible
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

    public override void ReactTo(ElementType other)
    {
        // put base logic here
        throw new System.NotImplementedException();
    }
}