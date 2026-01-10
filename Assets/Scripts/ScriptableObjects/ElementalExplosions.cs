using UnityEngine;

public struct ElementExplosionPair
{
    public ElementType Element;
    public GameObject ExplosionObject;
}

// Data type which holds a list of explosion objects related to the "Element" enum
[CreateAssetMenu(fileName = "ElementalExplosionsDB", menuName = "Scriptable Objects/ElementalExplosions")]
public class ElementalExplosions : ScriptableObject
{
    // Dictionary lookup used to find material
    [SerializeField] public ElementExplosionPair[] ElementExplosions;
    
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
}
