using UnityEngine;
using UnityEngine.Experimental.VFX;

namespace GeoVfx
{
    sealed class VfxParameterBridge : MonoBehaviour
    {
        [SerializeField, Range(0, 1)] float _smoothness = 0;

        public float parameterValue {
            set {
                if (_smoothness == 0)
                {
                    // Direct modification
                    if (_target.enabled) _target.SetFloat(_id, value);
                }
                else
                {
                    // Input to the interpolator
                    _input = value;
                }
            }
        }

        VisualEffect _target;
        int _id;

        float _input;
        float _current;
        float _velocity;

        void Start()
        {
            _target = transform.parent.GetComponent<VisualEffect>();
            _id = Shader.PropertyToID(gameObject.name);
        }

        void Update()
        {
            if (_smoothness == 0) return;

            // Damped spring interpolator
            var dt = Time.deltaTime;
            var sp = 1 / _smoothness;
            var n1 = _velocity - (_current - _input) * sp * sp * dt;
            var n2 = 1 + sp * dt;
            _velocity = n1 / (n2 * n2);
            _current += _velocity * dt;

            if (_target.enabled) _target.SetFloat(_id, _current);
        }
    }
}
