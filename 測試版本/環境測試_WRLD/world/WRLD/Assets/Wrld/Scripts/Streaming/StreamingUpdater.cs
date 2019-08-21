using Wrld.Common.Maths;
using Wrld.MapCamera;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Wrld
{
    public class StreamingUpdater
    {
        private int m_screenWidth = 0;
        private int m_screenHeight = 0;

        [DllImport(NativePluginRunner.DLL)]
        private static extern void SetCustomStreamingCameraState(IntPtr ptr, CameraState state);

        [DllImport(NativePluginRunner.DLL)]
        private static extern void ClearCustomStreamingCamera(IntPtr ptr);

        [DllImport(NativePluginRunner.DLL)]
        private static extern void NotifyScreenSizeChanged(IntPtr ptr, float screenWidth, float screenHeight);

        public void Update(UnityEngine.Camera zeroBasedCameraECEF, DoubleVector3 cameraOriginECEF, DoubleVector3 interestPointECEF)
        {
            if (InterestPointProvider.ClampInterestPointToValidRangeIfRequired(ref interestPointECEF))
            {
                Debug.LogWarning("Interest point had too high an altitude, clamping to valid range.");
            }

            UpdateScreenSize();

            if (IsValidStreamingCamera(zeroBasedCameraECEF))
            {
                var cameraState = new CameraState(
                    cameraOriginECEF,
                    interestPointECEF,
                    zeroBasedCameraECEF.worldToCameraMatrix,
                    zeroBasedCameraECEF.projectionMatrix
                    );

                SetCustomStreamingCameraState(NativePluginRunner.API, cameraState);
            }
        }

        public void UpdateForBuiltInCamera()
        {
            UpdateScreenSize();
            ClearCustomStreamingCamera(NativePluginRunner.API);
        }

        private void UpdateScreenSize()
        {
            if (Screen.width != m_screenWidth || Screen.height != m_screenHeight)
            {
                m_screenWidth = Screen.width;
                m_screenHeight = Screen.height;
                NotifyScreenSizeChanged(NativePluginRunner.API, Screen.width, Screen.height);
            }
        }

        private bool IsValidStreamingCamera(UnityEngine.Camera zeroBasedCameraECEF)
        {
            if (zeroBasedCameraECEF == null)
            {
                return false;
            }

            var isZeroBased = zeroBasedCameraECEF.transform.position.sqrMagnitude < 0.000001f;
            var hasNonzeroSize = zeroBasedCameraECEF.orthographicSize > 0;

            if (!isZeroBased)
            {
                Debug.LogError("Expected a camera with zero translation (position should be represented in cameraOriginECEF).");
            }

            if (!hasNonzeroSize)
            {
                Debug.LogError("Camera Orthographic Size must be greater than 0 for correct frustum calculation");
            }

            bool isValid = (isZeroBased && hasNonzeroSize);

            return isValid;
        }
    }
}

