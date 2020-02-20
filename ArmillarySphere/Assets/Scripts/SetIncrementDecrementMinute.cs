using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetIncrementDecrementMinute : MonoBehaviour
{
    [SerializeField] GameObject time_controller;

    private UpdateMinute GetScript()
    {
        // Gets the script needed to increment/decrement the clock

        return time_controller.GetComponent<UpdateMinute>();
    }

    public void SetToDecrement()
    {
        UpdateMinute time_controller_script = GetScript();
        time_controller_script.increment = false;
    }

    public void SetToIncrement()
    {
        UpdateMinute time_controller_script = GetScript();
        time_controller_script.increment = true;
    }
}
