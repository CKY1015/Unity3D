using Wrld.Common.Maths;
using UnityEngine;

namespace Wrld.Space
{
    public class ECEFTransformUpdateStrategy : ITransformUpdateStrategy
    {
        private DoubleVector3 m_cameraPositionECEF;
        private Vector3 m_up;
        private float m_flattenScale;

        public ECEFTransformUpdateStrategy(DoubleVector3 cameraPositionECEF, Vector3 up, float scale)
        {
            m_cameraPositionECEF = cameraPositionECEF;
            m_up = up;
            m_flattenScale = scale;
        }

        public void UpdateTransform(Transform objectTransform, DoubleVector3 objectOriginECEF, Vector3 translationOffsetECEF, Quaternion orientationECEF, float heightOffset, bool applyFlattening)
        {
            var cameraRelativePosition = (objectOriginECEF - m_cameraPositionECEF).ToSingleVector();
            cameraRelativePosition +=  m_up * heightOffset + translationOffsetECEF;
            
            if (applyFlattening && m_flattenScale != 1.0f)
            {
                var scaleVec = new Vector3(1, m_flattenScale, 1);
                var upECEF = objectOriginECEF.normalized.ToSingleVector();
                var localToECEF = Quaternion.FromToRotation(Vector3.up, m_up);
                var ecefToLocal = Quaternion.FromToRotation(m_up, Vector3.up);
                var innerRotation = ecefToLocal * orientationECEF;

                TransformHelper.ApplyTransform(objectTransform, cameraRelativePosition, scaleVec, localToECEF, innerRotation);
            }
            else
            {
                TransformHelper.ApplyTransform(objectTransform, cameraRelativePosition, Vector3.one, orientationECEF, Quaternion.identity);
            }
        }
    }
}

