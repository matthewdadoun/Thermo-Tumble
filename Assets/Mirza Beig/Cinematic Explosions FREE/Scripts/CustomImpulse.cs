using UnityEngine;
using Unity.Cinemachine;

namespace MirzaBeig.CinematicExplosionsFree
{
    public class CustomImpulse : MonoBehaviour
    {
        private CinemachineImpulseSource _source;

        void OnEnable()
        {
            if (!_source)
            {
                _source = GetComponent<CinemachineImpulseSource>();
            }

            _source.GenerateImpulse();
        }
    }
}
