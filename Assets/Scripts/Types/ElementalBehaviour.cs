using UnityEngine;

public enum ElementType
{
    None,
    Ice,
    Lava,
    Acid,
    Water,
    Rock,
    Sand
}

public abstract class ElementalBehaviour : MonoBehaviour
{
    [SerializeField] protected ElementType element;
    
    // Getter / setter for element type
    public ElementType Element
    {
        get => element;
        set => element = value;
    }

    public virtual void ReactTo(ElementType other)
    {
        /*// Shared default behavior
        var reaction = ElementReactions.Get(element, other);
        Debug.Log($"{name} ({element}) reacts to {other}: {reaction}");*/
    }
}