using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct ElementMaterialPair
{
    public ElementType element;
    public Material material;
}

[CreateAssetMenu(fileName = "ElementalMaterialsDB", menuName = "Scriptable Objects/ElementalMaterials")]
public class ElementalMaterials : ScriptableObject
{
    // Dictionary lookup used to find material
    [SerializeField] public ElementMaterialPair[] elementMaterials;


    // Simple singleton-style access with lazy load
    public const string ResourcePath = "ElementalMaterialsDB"; // or "Configs/ElementalMaterialsDB"
    private static ElementalMaterials _instance;

    public static ElementalMaterials Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<ElementalMaterials>(ResourcePath);
            }

            return _instance;
        }
    }
}