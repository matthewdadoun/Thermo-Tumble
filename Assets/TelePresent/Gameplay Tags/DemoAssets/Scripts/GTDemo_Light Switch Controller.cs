using UnityEngine;
using System.Collections;

namespace TelePresent.GameplayTags
{
    public class LightSwitchController : MonoBehaviour
    {
        public GameplayTag wireFixedState;
        public GameplayTag switchedState;
        public GameplayTagManager tagManager;
        public Light lightBulb;
        public GameObject lever;
        public GameObject wirePiece;

        private void OnEnable()
        {
            if (tagManager != null)
            {
                switchedState.OnTagChanged += PullSwitch;
                wireFixedState.OnTagChanged += ToggleWireFixed;

            }
            UpdateLightState();
        }

        private void OnDisable()
        {
            if (tagManager != null)
            {
                switchedState.OnTagChanged -= PullSwitch;
                wireFixedState.OnTagChanged -= ToggleWireFixed;
            }
        }

        private void UpdateLightState()
        {
                if (GameplayTagUtility.IsTagAndParentsActive(tagManager, switchedState))
                    lightBulb.enabled = true;

                else
                    lightBulb.enabled = false;

        }

        public void PullSwitch(bool newLocalState)
        {

            if (GameplayTagUtility.IsTagActive(switchedState))
            {
                StartCoroutine(RotateLever(true));
                if (GameplayTagUtility.IsTagAndParentsActive(tagManager, wireFixedState))
                    lightBulb.enabled = true;
            }
            else
            {
                StartCoroutine(RotateLever(false));
                    lightBulb.enabled = false;
            }
        }

        public void ToggleWireFixed( bool newLocalState)
        {

            if (GameplayTagUtility.IsTagActive(wireFixedState))
            {
                wirePiece.SetActive(true);
                if (GameplayTagUtility.IsTagActive(switchedState))
                    lightBulb.enabled = true;
            }
            else
            {
                wirePiece.SetActive(false);
                    lightBulb.enabled = false;
            }
        }

        private IEnumerator RotateLever(bool rotateForward)
        {
            const float duration = 0.5f;
            float elapsedTime = 0f;

            Vector3 initialAngles = lever.transform.localEulerAngles;
            float startZ = initialAngles.z;
            if (startZ > 180f)
                startZ -= 360f;

            float targetZ = !rotateForward ? 64f : -64f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                float newZ = Mathf.LerpAngle(startZ, targetZ, t);

                lever.transform.localEulerAngles = new Vector3(initialAngles.x, initialAngles.y, newZ);
                yield return null;
            }
            lever.transform.localEulerAngles = new Vector3(initialAngles.x, initialAngles.y, targetZ);
        }
    }
}
