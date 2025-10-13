using UnityEngine;

public enum ElementType
{
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
    public ElementType Element => element;

    public virtual void ReactTo(ElementType other)
    {
        /*// Shared default behavior
        var reaction = ElementReactions.Get(element, other);
        Debug.Log($"{name} ({element}) reacts to {other}: {reaction}");*/
    }
}