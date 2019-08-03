using UnityEngine;
using Unity.Mathematics;
using Klak.Math;
using static Unity.Mathematics.math;
using static Unity.Mathematics.quaternion;

namespace GeoVfx
{
    sealed class SceneController : MonoBehaviour
    {
        #region Editable attributes

        [SerializeField] Camera _mainCamera = null;
        [SerializeField] Transform _cameraPivot = null;
        [SerializeField] Transform _cameraArm = null;
        [SerializeField] Transform _globePivot = null;

        #endregion

        #region Local variables

        float2 _prevMousePos;

        float3 _dragFrom, _dragTo;
        quaternion _rotateFrom;

        float3 _cameraAngles;
        float _cameraDistance;

        #endregion

        #region Internal utility methods

        quaternion RotationBetween(float3 v1, float3 v2)
        {
            var c = cross(v1, v2);
            var w = sqrt(dot(v1, v1) * dot(v2, v2)) + dot(v1, v2);
            return normalize(quaternion(float4(c, w)));
        }

        float3? MouseRayCast()
        {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) return hit.point;
            return null;
        }

        #endregion

        #region MonoBehaviour implementation

        void Start()
        {
            // Initial camera angels
            _cameraAngles = radians(_cameraPivot.localRotation.eulerAngles);
            _cameraAngles = (_cameraAngles + PI) % (2 * PI) - PI;

            // Initial camera distance
            _cameraDistance = -_cameraArm.localPosition.z;

            // Initial mouse position
            _prevMousePos = float3(Input.mousePosition).xy;

            // To avoid Nan
            _dragFrom = _dragTo = float3(0, 0, 1);
        }

        void Update()
        {

            // Update the drag-to point while pressing the left button down.
            if (Input.GetMouseButton(0)) _dragTo = MouseRayCast() ?? _dragTo;

            // Update the drag-from point on a left button down.
            if (Input.GetMouseButtonDown(0))
            {
                _dragFrom = _dragTo;
                _rotateFrom = _globePivot.localRotation;
            }

            // Globe pivot rotation
            {
                var delta = RotationBetween(_dragFrom, _dragTo);
                var target = mul(delta, _rotateFrom);
                _globePivot.localRotation =
                    ExpTween.Step(_globePivot.localRotation, target, 15);
            }

            // Camera angle adjustment by right mouse drag
            {
                var mp = float3(Input.mousePosition).xy;
                var dp = (float2)(mp - _prevMousePos);

                if (Input.GetMouseButton(1))
                {
                    var delta = dp.yx * float2(-1, 1) * 0.002f;
                    _cameraAngles.xy = clamp(_cameraAngles.xy + delta, -1, 1);
                }

                _prevMousePos = mp;
            }

            // Camera pivot rotation
            {
                var target = EulerZXY(_cameraAngles);
                _cameraPivot.localRotation =
                    ExpTween.Step(_cameraPivot.localRotation, target, 15);
            }

            // Mouse wheel (camera distance)
            {
                var delta = Input.mouseScrollDelta.y * -0.8f;
                _cameraDistance = Mathf.Max(1, _cameraDistance + delta);

                var z = _cameraArm.localPosition.z;
                z = ExpTween.Step(z, -_cameraDistance, 15);

                _cameraArm.localPosition = new Vector3(0, 0, z);
            }
        }

        #endregion
    }
}
