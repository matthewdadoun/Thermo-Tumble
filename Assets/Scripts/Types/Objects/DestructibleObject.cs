using UnityEngine;

public class DestructibleObject : ElementalBehaviour, IDestructible
{
    public void OnDestructibleOverlap(GameObject destroyer)
    {
        // If destroyer is valid
        if (destroyer)
        {
            // Destroy this game object
            Destroy(gameObject);
        }
    }

    protected void OnTriggerEnter(Collider other)
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
        if (IsOpposingElement(other))
        {
            Destroy(gameObject);
        }
    }
}