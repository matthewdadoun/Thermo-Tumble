using UnityEngine;
using UnityEngine.Serialization;

public class AnimatedMat : MonoBehaviour
{
    public Material objectMat;

    public float scrollSpeedX = 1.75f;

    private float _offsetX;

    // Update is called once per frame
    void Update()
    {
        _offsetX += Time.deltaTime * scrollSpeedX;
        objectMat.mainTextureOffset = new Vector2(_offsetX, 0);
    }
}