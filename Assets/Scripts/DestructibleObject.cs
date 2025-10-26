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

    private void OnTriggerEnter(Collider other)
    {
        var elementalBehaviour = other.gameObject.GetComponent<ElementalBehaviour>();
        if (elementalBehaviour == null)
        {
            return;
        }
        
        // Have both objects react to each other
        elementalBehaviour.ReactTo(element);
        ReactTo(elementalBehaviour.Element);
    }

    public override void ReactTo(ElementType other)
    {
        switch (element)
        {
            // put base logic here
            case ElementType.Ice when other == ElementType.Lava:
            case ElementType.Lava when other == ElementType.Ice:
            {
                Destroy(gameObject);
                break;
            }
        }
    }
}