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

    private SkinnedMeshRenderer _mesh;

    private void Awake()
    {
        // Store mesh renderer
        _mesh = gameObject.GetComponent<SkinnedMeshRenderer>();

        if (_elementMats == null)
        {
            // retrieve elemental materials singleton
            _elementMats = ElementalMaterials.Instance;
        }
    }

    // Getter / setter for element type
    public ElementType Element
    {
        get => element;
        set
        {
            element = value;

            if (!_elementMats)
            {
                return;
            }

            Material mat = null;
            foreach (var elementMat in _elementMats.elementMaterials)
            {
                // if we found the matching element
                if (elementMat.element == element)
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
            List<Material> materials = new List<Material>(_mesh.materials);
            for (int i = 0; i < materials.Count; i++)
            {
                materials[i] = mat;
            }

            // set new materials
            _mesh.SetMaterials(materials);
        }
    }

    public virtual void ReactTo(ElementType other)
    {
        /*// Shared default behavior
        var reaction = ElementReactions.Get(element, other);
        Debug.Log($"{name} ({element}) reacts to {other}: {reaction}");*/
    }
}