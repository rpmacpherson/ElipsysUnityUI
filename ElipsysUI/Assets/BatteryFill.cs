using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryFill : MonoBehaviour
{
    //Script is attached to the object "Battery Icon- Fill"
    [SerializeField]
    private TobiiViewServerController tobiiViewServerController;
    [SerializeField]
    Image batteryFillLevel;

    public void UpdateFill()
    {
        batteryFillLevel.fillAmount = tobiiViewServerController.batteryLevel;
    }

    //void Update()
    //{
    //    batteryFillLevel.fillAmount = tobiiViewServerController.batteryLevel;
    //}
}