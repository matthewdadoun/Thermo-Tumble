using UnityEngine;
using UnityEngine.InputSystem;

public class TouchInputHandler : MonoBehaviour
{
    // Canvas used for input
    [SerializeField] private GameObject touchCanvas;

    private void Awake()
    {
        var hasTouch = Touchscreen.current != null;
        touchCanvas.SetActive(hasTouch);
    }
}
