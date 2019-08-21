using Wrld.Common.Maths;
using UnityEngine;
using Wrld.Space;
using System;

namespace Wrld.MapCamera
{
    class InterestPointProvider
    {
        private const double MaximumInterestPointAltitude = EarthConstants.Radius + 9000.0;
        private const double MaximumInterestPointAltitudeSquared = MaximumInterestPointAltitude * MaximumInterestPointAltitude;

        private DoubleVector3 m_interestPointECEF;
        private bool m_hasInterestPointFromNativeController;
        
        public void UpdateFromNative(DoubleVector3 interestPointECEF)
        {
            m_interestPointECEF = interestPointECEF;
            m_hasInterestPointFromNativeController = true;
        }

        public DoubleVector3 CalculateInterestPoint(Camera cameraECEF, DoubleVector3 cameraOriginECEF)
        {
            if (m_hasInterestPointFromNativeController)
            {
                m_hasInterestPointFromNativeController = false;
                return m_interestPointECEF;
            }

            return CalculateEstimatedInterestPoint(cameraECEF, cameraOriginECEF);
        }

        private DoubleVector3 CalculateEstimatedInterestPoint(Camera cameraECEF, DoubleVector3 cameraOriginECEF)
        {
            DoubleVector3 finalCameraPositionECEF = cameraOriginECEF;// + cameraECEF.transform.position;
            DoubleVector3 estimatedInterestPointECEF = finalCameraPositionECEF + cameraECEF.transform.forward * (cameraECEF.nearClipPlane + cameraECEF.farClipPlane) * 0.5f;
            ClampInterestPointToValidRangeIfRequired(ref estimatedInterestPointECEF);

            return estimatedInterestPointECEF;
        }

        public static bool ClampInterestPointToValidRangeIfRequired(ref DoubleVector3 interestPointEcef)
        {
            double magnitudeSquared = interestPointEcef.sqrMagnitude;

            if (magnitudeSquared > MaximumInterestPointAltitudeSquared)
            {
                interestPointEcef *= MaximumInterestPointAltitude / Math.Sqrt(magnitudeSquared);

                return true;
            }

            return false;
        }
    }
}