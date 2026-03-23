using UnityEngine;

[System.Serializable]
public struct ElementExplosionPair
{
    public ElementType element;
    public GameObject explosionObject;
}

// Data type which holds a list of explosion objects related to the "Element" enum
[CreateAssetMenu(fileName = "ElementalExplosionsDB", menuName = "Scriptable Objects/ElementalExplosions")]
public class ElementalExplosions : ScriptableObject
{
    // Dictionary lookup used to find material
    [SerializeField] public ElementExplosionPair[] elementExplosions;

    // Simple singleton-style access with a lazy load
    public const string ResourcePath = "ElementalExplosionsDB"; // or "Configs/ElementalExplosionsDB"
    private static ElementalExplosions _instance;

    public static ElementalExplosions Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<ElementalExplosions>(ResourcePath);
            }

            return _instance;
        }
    }

    public void SpawnInstanceAttached(ElementType inType, GameObject inObject)
    {
        // Check to see which explosion to use
        foreach (var elementExplosion in elementExplosions)
        {
            // Retrieve element explosion type
            if (elementExplosion.element != inType)
            {
                continue;
            }

            // spawn element
            Instantiate(elementExplosion.explosionObject, inObject.transform);
            break;
        }
    }
}