using Wrld.Common.Maths;
using UnityEngine;

namespace Wrld.Space
{
    public interface ITransformUpdateStrategy
    {
        void UpdateTransform(Transform objectTransform, DoubleVector3 objectOriginECEF, Vector3 translationOffsetECEF, Quaternion orientationECEF, float heightOffset, bool applyFlattening);
    }
}
