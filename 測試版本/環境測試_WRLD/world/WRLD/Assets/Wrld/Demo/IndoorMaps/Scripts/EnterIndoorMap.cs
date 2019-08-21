using Wrld;
using Wrld.Space;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EnterIndoorMap : MonoBehaviour
{
    private LatLong m_indoorMapLocation = LatLong.FromDegrees(56.460143, -2.978325);
    private Slider m_floorSlider;
    private Color m_meetingRoomColor = new Color(1.0f, 0.0f, 0.0f, 0.5f);
    private Color m_smallRoomColor = new Color(1.0f, 1.0f, 0.0f, 0.5f);
    private Color m_clickedColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);

    private void OnEnable()
    {   
        m_floorSlider = FindObjectOfType<Slider>();

        Api.Instance.IndoorMapsApi.OnIndoorMapEntered += IndoorMapsApi_OnIndoorMapEntered;
        Api.Instance.IndoorMapsApi.OnIndoorMapExited += IndoorMapsApi_OnIndoorMapExited;
        Api.Instance.IndoorMapsApi.OnIndoorMapFloorChanged += IndoorMapsApi_OnIndoorMapFloorChanged;
        Api.Instance.IndoorMapsApi.OnIndoorMapEntityClicked += IndoorMapsApi_OnIndoorMapEntityClicked;

        Api.Instance.CameraApi.MoveTo(m_indoorMapLocation, distanceFromInterest: 500, headingDegrees: 0, tiltDegrees: 45); 
    }

    private void OnDisable()
    {
        Api.Instance.IndoorMapsApi.OnIndoorMapFloorChanged -= IndoorMapsApi_OnIndoorMapFloorChanged;
        Api.Instance.IndoorMapsApi.OnIndoorMapExited -= IndoorMapsApi_OnIndoorMapExited;
        Api.Instance.IndoorMapsApi.OnIndoorMapEntered -= IndoorMapsApi_OnIndoorMapEntered;
        Api.Instance.IndoorMapsApi.OnIndoorMapEntityClicked -= IndoorMapsApi_OnIndoorMapEntityClicked;
    }

    public void OnExpand()
    {
        Api.Instance.IndoorMapsApi.ExpandIndoor();
    }

    public void OnCollapse()
    {
        Api.Instance.IndoorMapsApi.CollapseIndoor();
    }

    public void MoveUp()
    {
        Api.Instance.IndoorMapsApi.MoveUpFloor();
    }

    public void MoveDown()
    {
        Api.Instance.IndoorMapsApi.MoveDownFloor();
    }

    public void ExitMap()
    {
        Api.Instance.IndoorMapsApi.ExitIndoorMap();
    }

    public void EnterMap()
    {
        if (Api.Instance.IndoorMapsApi.GetActiveIndoorMap() == null)
        {
            Api.Instance.CameraApi.MoveTo(m_indoorMapLocation, distanceFromInterest: 500);
            Api.Instance.IndoorMapsApi.EnterIndoorMap("westport_house");
        }
    }

    public void OnSliderValueChanged()
    {
        Api.Instance.IndoorMapsApi.SetIndoorFloorInterpolation(m_floorSlider.value);
    }

    public void OnBeginDrag()
    {
        Api.Instance.IndoorMapsApi.ExpandIndoor();
    }

    public void OnEndDrag()
    {
        float sliderValue = m_floorSlider.value;
        int roundedValue = Mathf.RoundToInt(sliderValue);
        var map = Api.Instance.IndoorMapsApi.GetActiveIndoorMap();

        if (map != null)
        {
            int endFloorId = map.FloorIds[roundedValue];
            Api.Instance.IndoorMapsApi.SetCurrentFloorId(endFloorId);
            Api.Instance.IndoorMapsApi.CollapseIndoor();
        }
    }

    private void IndoorMapsApi_OnIndoorMapFloorChanged(int newFloorId)
    {
        Debug.LogFormat("Switched to floor {0}!", newFloorId);

        var map = Api.Instance.IndoorMapsApi.GetActiveIndoorMap();

        if (map != null)
        {
            m_floorSlider.value = Array.IndexOf(map.FloorIds, newFloorId);
        }

        if (map.Id == "westport_house")
        {
            if (newFloorId == 2)
            {
                Api.Instance.IndoorMapsApi.SetIndoorEntityHighlight("Small Meeting Room", m_meetingRoomColor);
                Api.Instance.IndoorMapsApi.SetIndoorEntityHighlight("Meeting Room", m_smallRoomColor);
            }
            else
            {
                Api.Instance.IndoorMapsApi.ClearIndoorEntityHighlights(new[] { "Small Meeting Room", "Meeting Room" });
            }
        }
    }

    private void IndoorMapsApi_OnIndoorMapExited()
    {
        Debug.Log("Exited indoor map!");
    }

    private void IndoorMapsApi_OnIndoorMapEntered()
    {
        Debug.Log("Entered indoor map!");

        var map = Api.Instance.IndoorMapsApi.GetActiveIndoorMap();
        m_floorSlider.minValue = 0.0f;
        m_floorSlider.maxValue = map.FloorCount - 1.0f;
        var currentFloorId = Api.Instance.IndoorMapsApi.GetCurrentFloorId();
        m_floorSlider.value = Array.IndexOf(map.FloorIds, currentFloorId);
    }

    private void IndoorMapsApi_OnIndoorMapEntityClicked(string entityIds) 
    {
        Debug.Log("Clicked on interior object(s)!: " + entityIds);

        if (entityIds != null && entityIds.Length > 0)
        {
            string[] selectedEntities = entityIds.Split('|');

            foreach (string entityId in selectedEntities)
            {
                Api.Instance.IndoorMapsApi.SetIndoorEntityHighlight(entityId, m_clickedColor); 
            }
        }
    }
}
