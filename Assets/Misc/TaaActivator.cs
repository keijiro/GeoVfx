using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;
using BindingFlags = System.Reflection.BindingFlags;
using AntialiasingMode = UnityEngine.Experimental.Rendering.
    HDPipeline.HDAdditionalCameraData.AntialiasingMode;

namespace GeoVfx
{
    sealed class TaaActivator : MonoBehaviour
    {
        #region Editable attributes

        [SerializeField] Camera _camera = null;
        [SerializeField] Transform _marker = null;

        #endregion

        #region Local objects

        Vector3 _previousPosition;

        #endregion

        #region Cached references

        // Reference to HDAdditionalCameraData
        HDAdditionalCameraData HDCamera { get {
            if (_hdCamera != null) return _hdCamera;
            return (_hdCamera = _camera.GetComponent<HDAdditionalCameraData>());
        } }

        HDAdditionalCameraData _hdCamera;

        // HDRenderPipeline.m_PostProcessSystem via reflection
        PostProcessSystem PostProcess { get {
            if (_postProcess != null) return _postProcess;

            var hdrp = RenderPipelineManager.currentPipeline as HDRenderPipeline;
            if (hdrp == null) return null;

            var ppsField = hdrp.GetType().GetField(
                "m_PostProcessSystem",
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            return (_postProcess = (PostProcessSystem)ppsField.GetValue(hdrp));
        } }

        PostProcessSystem _postProcess;

        #endregion

        #region MonoBehaviour implementation

        void LateUpdate()
        {
            if (HDCamera == null || PostProcess == null) return;

            // Check if the marker is moving.
            var pos = _camera.transform.TransformPoint(_marker.position);
            var moving = (pos - _previousPosition).magnitude > 0.0001f;
            _previousPosition = pos;

            // Disable TAA while the marker is moving.
            if (moving)
            {
                _hdCamera.antialiasing = AntialiasingMode.None;
                return;
            }

            // Enable TAA. We have to reset TAA history before activating it.
            if (_hdCamera.antialiasing == AntialiasingMode.None)
            {
                PostProcess.ResetHistory();
                _hdCamera.antialiasing = AntialiasingMode.TemporalAntialiasing;
            }
        }

        #endregion
    }
}
