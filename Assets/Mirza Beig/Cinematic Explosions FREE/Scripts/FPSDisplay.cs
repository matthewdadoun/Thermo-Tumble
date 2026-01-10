using UnityEngine;

using TMPro;
using System;

namespace MirzaBeig.CinematicExplosionsFree
{
    [ExecuteAlways]
    public class FPSDisplay : MonoBehaviour
    {
        public float FPS { get; private set; }      // Frames per second (interval average).
        public float FrameMS { get; private set; }  // Milliseconds per frame (interval average).

        private readonly GUIStyle _style = new GUIStyle();

        public int size = 16;

        [Space]

        public Vector2 position = new Vector2(16.0f, 16.0f);

        public enum Alignment { Left, Right }
        public Alignment alignment = Alignment.Left;

        [Space]

        public Color colour = Color.green;

        [Space]

        public float updateInterval = 0.5f;

        float _elapsedIntervalTime;
        int _intervalFrameCount;

        [Space]

        [Tooltip("Optional. Will render using GUI if not assigned.")]
        public TextMeshProUGUI textMesh;

        // Get average FPS and frame delta (ms) for current interval (so far, if called early).

        public float GetIntervalFPS()
        {
            // 1 / time.unscaledDeltaTime for same-frame results.
            // Same as above, but uses accumulated frameCount and deltaTime.

            return _intervalFrameCount / _elapsedIntervalTime;
        }
        public float GetIntervalFrameMS()
        {
            // Calculate average frame delta during interval (time / frames).
            // Same as Time.unscaledDeltaTime * 1000.0f, using accumulation.

            return (_elapsedIntervalTime * 1000.0f) / _intervalFrameCount;
        }

        void Update()
        {
            _intervalFrameCount++;
            _elapsedIntervalTime += Time.unscaledDeltaTime;

            if (_elapsedIntervalTime >= updateInterval)
            {
                FPS = GetIntervalFPS();
                FrameMS = GetIntervalFrameMS();

                FPS = (float)Math.Round(FPS, 2);
                FrameMS = (float)Math.Round(FrameMS, 2);

                _intervalFrameCount = 0;
                _elapsedIntervalTime = 0.0f;
            }

            if (textMesh)
            {
                textMesh.text = GetFPSText();
            }
            else
            {
                _style.fontSize = size;
                _style.fontStyle = FontStyle.Bold;
                _style.normal.textColor = colour;
            }
        }

        string GetFPSText()
        {
            return $"FPS: {FPS:.00} ({FrameMS:.00} ms)";
        }

        void OnGUI()
        {
            string fpsText = GetFPSText();

            if (!textMesh)
            {
                float x = position.x;

                if (alignment == Alignment.Right)
                {
                    x = Screen.width - x - _style.CalcSize(new GUIContent(fpsText)).x;
                }

                GUI.Label(new Rect(x, position.y, 200, 100), fpsText, _style);
            }
        }
    }
}