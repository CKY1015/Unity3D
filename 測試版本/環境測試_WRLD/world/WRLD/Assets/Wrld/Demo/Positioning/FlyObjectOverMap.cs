using System.Collections;
using UnityEngine;
using Wrld;
using Wrld.Space;
using Wrld.Space.Positioners;

public class FlyObjectOverMap: MonoBehaviour
{
    static LatLong startPosition = LatLong.FromDegrees(37.783372, -122.400834);
    static float movementSpeed = 400.0f;

    public GeographicTransform coordinateFrame;
    public Transform box;
    public Positioner positioner;
    public LatLong targetPosition;

    void OnEnable()
    {
        Api.Instance.GeographicApi.RegisterGeographicTransform(coordinateFrame);
        targetPosition = startPosition;
        positioner = Api.Instance.PositionerApi.CreatePositioner(new PositionerOptions()
                                                                            .LatitudeDegrees(targetPosition.GetLatitude())
                                                                            .LongitudeDegrees(targetPosition.GetLongitude())
                                                                            .ElevationAboveGround(100));



        Api.Instance.CameraApi.MoveTo(targetPosition, distanceFromInterest: 1700, headingDegrees: 0, tiltDegrees: 45);
        coordinateFrame.SetPosition(targetPosition);
        
        box.SetParent(coordinateFrame.transform);
        box.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        box.localRotation = Quaternion.identity;
        positioner.SetLocation(targetPosition.GetLatitude(), targetPosition.GetLongitude());
    }

    void Update()
    {
        // Update target position from input
        float latitudeDelta = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
        float longitudeDelta = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;

        targetPosition.SetLatitude(targetPosition.GetLatitude() + (latitudeDelta * 0.00006f));
        targetPosition.SetLongitude(targetPosition.GetLongitude() + (longitudeDelta * 0.00006f));

        // Command positioner to move using lat-long
        positioner.SetLocation(targetPosition.GetLatitude(), targetPosition.GetLongitude());

        // Update physical location via GeometryTransform, with corrected ECEF fetched from positioner
        coordinateFrame.SetPosition(positioner.GetECEFLocation());
    }

    void OnDisable()
    {
        Api.Instance.GeographicApi.UnregisterGeographicTransform(coordinateFrame);
    }
}

