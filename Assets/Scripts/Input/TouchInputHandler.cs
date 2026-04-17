using UnityEngine;
using UnityEngine.InputSystem;

public class TouchInputHandler : MonoBehaviour
{
    // Canvas used for input
    [SerializeField] private GameObject touchHUD;

    private void Awake()
    {
        var hasTouch = Touchscreen.current != null;
        touchHUD.SetActive(hasTouch);
    }
}
