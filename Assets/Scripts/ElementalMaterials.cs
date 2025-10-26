using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct ElementMaterialPair
{
    public ElementType element;
    public Material material;
}

[CreateAssetMenu(fileName = "ElementalMaterials", menuName = "Scriptable Objects/ElementalMaterials")]
public class ElementalMaterials : ScriptableObject
{
    // Dictionary lookup used to find material
    [SerializeField] public ElementMaterialPair[] elementMaterials;
}
