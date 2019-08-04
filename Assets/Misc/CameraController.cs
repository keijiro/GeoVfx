using UnityEngine;
using Unity.Mathematics;
using Klak.Math;
using static Unity.Mathematics.math;
using static Unity.Mathematics.quaternion;

namespace GeoVfx
{
    sealed class CameraController : MonoBehaviour
    {
        #region Local variables

        float2 _prevMousePos;
        float3 _cameraAngles;
        float _cameraDistance;

        #endregion

        #region MonoBehaviour implementation

        void Start()
        {
            // Initial mouse position
            _prevMousePos = float3(Input.mousePosition).xy;

            // Initial camera angels
            _cameraAngles = radians(transform.parent.localRotation.eulerAngles);
            _cameraAngles = (_cameraAngles + PI) % (2 * PI) - PI;

            // Initial camera distance
            _cameraDistance = -transform.localPosition.z;
        }

        void Update()
        {
            // Camera angle adjustment by right mouse drag
            var mp = float3(Input.mousePosition).xy;
            var d_mp = (float2)(mp - _prevMousePos);

            if (Input.GetMouseButton(1))
            {
                var d_angle = d_mp.yx * float2(-1, 1) * 0.002f;
                _cameraAngles.xy = clamp(_cameraAngles.xy + d_angle, -1, 1);
            }

            _prevMousePos = mp;

            // Camera pivot rotation
            var pivot = transform.parent;
            var target = EulerZXY(_cameraAngles);
            pivot.localRotation = ExpTween.Step(pivot.localRotation, target, 12);

            // Mouse wheel (camera distance)
            var d_dist = Input.mouseScrollDelta.y * -0.8f;
            _cameraDistance = Mathf.Max(1, _cameraDistance + d_dist);

            var z = transform.localPosition.z;
            z = ExpTween.Step(z, -_cameraDistance, 12);

            transform.localPosition = new Vector3(0, 0, z);
        }

        #endregion
    }
}
