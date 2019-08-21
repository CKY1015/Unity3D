using Wrld;
using Wrld.Space;
using UnityEngine;
using System.Collections;

public class PickingIndoors : MonoBehaviour
{
    private LatLong m_indoorMapLocation = LatLong.FromDegrees(56.459984, -2.978238);

    private Color m_pickColor = new Color(1.0f, 1.0f, 0.0f, 0.5f);

    private void OnEnable()
    {
        Api.Instance.IndoorMapsApi.OnIndoorMapEntityClicked += IndoorMapsApi_OnIndoorMapEntityClicked;
        Api.Instance.IndoorMapsApi.OnIndoorMapEntered += IndoorMapsApi_OnIndoorMapEntered;

        Api.Instance.CameraApi.MoveTo(m_indoorMapLocation, distanceFromInterest: 200, headingDegrees: 0, tiltDegrees: 45);

        StartCoroutine(EnterMap());
    }

    IEnumerator EnterMap()
    {
        yield return new WaitForSeconds(5.0f);

        Api.Instance.CameraApi.MoveTo(m_indoorMapLocation, distanceFromInterest: 500);
        Api.Instance.IndoorMapsApi.EnterIndoorMap("westport_house");
    }

    public void ClearHighlights()
    {
        Api.Instance.IndoorMapsApi.ClearIndoorEntityHighlights();
    }

    private void OnDisable()
    {
        Api.Instance.IndoorMapsApi.OnIndoorMapEntityClicked -= IndoorMapsApi_OnIndoorMapEntityClicked;
        Api.Instance.IndoorMapsApi.OnIndoorMapEntered -= IndoorMapsApi_OnIndoorMapEntered;
    }

    private void IndoorMapsApi_OnIndoorMapEntityClicked(string entityIds)
    {
        Debug.Log("Clicked on interior object(s)!: " + entityIds);

        if (entityIds != null && entityIds.Length > 0)
        {
            string[] selectedEntities = entityIds.Split('|');

            foreach (string entityId in selectedEntities)
            {
                Api.Instance.IndoorMapsApi.SetIndoorEntityHighlight(entityId, m_pickColor);
            }
        }
    }

    private void IndoorMapsApi_OnIndoorMapEntered()
    {
        Api.Instance.IndoorMapsApi.SetCurrentFloorId(2);
    }
}
