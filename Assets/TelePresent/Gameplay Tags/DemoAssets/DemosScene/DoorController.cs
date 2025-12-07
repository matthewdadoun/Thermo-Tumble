using UnityEngine;
using TelePresent.GameplayTags;

namespace TelePresent.GameplayTags
{
    public class DoorController : MonoBehaviour
    {
        [Tooltip("Reference to the GameplayTag that indicates whether the door is locked.")]
        public GameplayTag doorLockedTag;

        [Tooltip("The angle (in degrees) when the door is open.")]
        public float openAngle = 90f;

        [Tooltip("The angle (in degrees) when the door is closed.")]
        public float closedAngle = 0f;

        [Tooltip("Speed at which the door rotates.")]
        public float rotationSpeed = 2f;

        // Tracks whether the door is currently open.
        private bool isOpen = false;

        // Call this method (e.g., via a button press) to toggle the door.
        public void ToggleDoor()
        {
            // Only toggle if doorLockedTag is not active.
            if (doorLockedTag != null && doorLockedTag.IsActive())
            {
                Debug.Log("Door is locked and cannot be opened.");
                return;
            }

            isOpen = !isOpen;
        }

        private void Update()
        {
            // Determine target angle based on isOpen state.
            float targetAngle = isOpen ? openAngle : closedAngle;
            // Get the current Y rotation.
            float currentAngle = transform.localEulerAngles.y;
            // Convert angles to a value between -180 and 180 for smooth interpolation.
            currentAngle = NormalizeAngle(currentAngle);
            targetAngle = NormalizeAngle(targetAngle);

            // Smoothly interpolate the door's rotation toward the target angle.
            float newAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * rotationSpeed);
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, newAngle, transform.localEulerAngles.z);

            //toggle door when I press space
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ToggleDoor();
            }
        }

        // Helper method to convert an angle (in degrees) to a range of -180 to 180.
        private float NormalizeAngle(float angle)
        {
            angle %= 360f;
            if (angle > 180f) angle -= 360f;
            if (angle < -180f) angle += 360f;
            return angle;
        }
    }
}