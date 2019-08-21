
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ButtonTest : MonoBehaviour
, IPointerClickHandler////(1)
, IPointerEnterHandler
, IPointerExitHandler
, IPointerDownHandler
, IPointerUpHandler
, IEventSystemHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click");
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Enter");
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Exit");
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Down");
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Up");
    }
}
