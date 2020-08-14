using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnSliderUp : MonoBehaviour, IPointerUpHandler
{
    Slider slider;      //the slider this script is attached
    float oldValue;     //previous value of the slider

    void Start()
    {
        slider = GetComponent<Slider>();  
        oldValue = slider.value;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (slider.value != oldValue)//if value has changed
        {
            //log change
            DebugLog.Instance.Write(slider.name + " value has been changed to " + slider.value);
            //set new log value
            oldValue = slider.value;
        }
    }
}
