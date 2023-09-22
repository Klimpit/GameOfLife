using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderClickDetector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool isSliderClicked = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        isSliderClicked = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isSliderClicked = false;
    }
}
