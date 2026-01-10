using UnityEngine;

namespace MirzaBeig.CinematicExplosionsFree
{
    [ExecuteAlways]
    public class FPSTest : MonoBehaviour
    {
        public Vector2 size = new Vector2(128.0f, 64.0f);
        public Vector2 position = new Vector2(16.0f, 64.0f);

        [Space]

        public float spacing = 72;

        [Space]

        public int[] fpsButtons = new int[] { 0, 10, 30, 45, 60, 90, 120 };

        void OnGUI()
        {
            var positionY = position.y;

            foreach (var fps in fpsButtons)
            {
                if (GUI.Button(new Rect(position.x, positionY, size.x, size.y), $"FPS: {fps}"))
                {
                    Application.targetFrameRate = fps;
                }

                positionY += spacing;
            }
        }
    }
}