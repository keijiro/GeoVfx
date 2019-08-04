using UnityEngine;
using Unity.Mathematics;
using Klak.Math;
using static Unity.Mathematics.math;
using static Unity.Mathematics.quaternion;

namespace GeoVfx
{
    sealed class GlobeController : MonoBehaviour
    {
        #region Editable attributes

        [SerializeField] Camera _mainCamera = null;

        #endregion

        #region Local variables

        float3? _dragFrom, _dragTo;
        quaternion _rotateFrom;

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

        void Update()
        {
            // Update the drag-to point while pressing the left button down.
            if (Input.GetMouseButton(0)) _dragTo = MouseRayCast() ?? _dragTo;

            // Update the drag-from point on a left button down.
            if (Input.GetMouseButtonDown(0))
            {
                _dragFrom = _dragTo;
                _rotateFrom = transform.localRotation;
            }

            // Globe pivot rotation
            if (_dragFrom != null && _dragTo != null)
            {
                var delta = RotationBetween((float3)_dragFrom, (float3)_dragTo);
                var target = mul(delta, _rotateFrom);
                transform.localRotation = ExpTween.Step(transform.localRotation, target, 12);
            }
        }

        #endregion
    }
}
