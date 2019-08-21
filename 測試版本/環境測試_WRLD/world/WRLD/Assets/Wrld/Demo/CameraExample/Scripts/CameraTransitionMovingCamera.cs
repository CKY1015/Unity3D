using System.Collections;
using Wrld;
using Wrld.Space;
using UnityEngine;

public class CameraTransitionMovingCamera : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(Example());
    }

    IEnumerator Example()
    {
        Api.Instance.CameraApi.RegisterShouldConsumeInputDelegate(ShouldConsumeCameraInput);

        var startLocation = LatLong.FromDegrees(37.7858, -122.401);
        Api.Instance.CameraApi.MoveTo(startLocation, distanceFromInterest: 800, headingDegrees: 0, tiltDegrees: 50);

        yield return new WaitForSeconds(4.0f);

        var destLocation = LatLong.FromDegrees(37.7952, -122.4028);
        Api.Instance.CameraApi.MoveTo(destLocation, distanceFromInterest: 500);
    }

    public bool ShouldConsumeCameraInput()
    {
        if(UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return false;
        }

        return true;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
