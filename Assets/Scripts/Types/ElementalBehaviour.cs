using System;
using System.Collections.Generic;
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
    private ElementalMaterials _elementMats;

    private Renderer _renderer;

    private void OnValidate()
    {
        Cache();

        SetElement(element);
    }

    void Cache()
    {
        // Store mesh renderer
        _renderer = gameObject.GetComponent<Renderer>();

        if (_elementMats == null)
        {
            // retrieve elemental materials singleton
            _elementMats = ElementalMaterials.Instance;
        }
    }

    private void Awake()
    {
        // Cache objects
        Cache();

        // Set element's material
        SetElement(element);
    }

    // Getter / setter for element type
    public ElementType Element
    {
        get => element;
    }

    public void SetElement(ElementType elementType)
    {
        // Update element type

        element = elementType;
        if (!_elementMats)
        {
            return;
        }

        Material mat = null;
        foreach (var elementMat in _elementMats.elementMaterials)
        {
            // if we found the matching element
            if (elementMat.element == elementType)
            {
                // store element material
                mat = elementMat.material;
            }
        }

        // if material is null
        if (!mat)
        {
            return;
        }

        // Set the material to ice
        List<Material> materials = null;

        // use "Shared" materials if in editor
        if (Application.isEditor)
        {
            materials = new List<Material>(_renderer.sharedMaterials);
        }

        // otherwise, use main materials
        else
        {
            materials = new List<Material>(_renderer.materials);
        }

        // Update new material
        for (int i = 0; i < materials.Count; i++)
        {
            materials[i] = mat;
        }

        // Update materials / shared materials
        if (Application.isEditor)
        {
            _renderer.SetSharedMaterials(materials);
        }
        else
        {
            _renderer.SetMaterials(materials);
        }

        // set new materials
        _renderer.SetMaterials(materials);
    }

    public virtual void ReactTo(ElementType other)
    {
        // Shared default behavior
        /*var reaction = ElementReactions.Get(element, other);
        Debug.Log($"{name} ({element}) reacts to {other}: {reaction}");*/
    }
}