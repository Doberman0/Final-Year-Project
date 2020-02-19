using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetIncrementDecrementHour : MonoBehaviour
{
    [SerializeField] GameObject time_controller;

    private UpdateTime GetScript()
    {
        // Gets the script needed to increment/decrement the clock

        return time_controller.GetComponent<UpdateTime>();
    }

    public void SetToDecrement()
    {
        UpdateTime time_controller_script = GetScript();
        time_controller_script.increment = false;
    }

    public void SetToIncrement()
    {
        UpdateTime time_controller_script = GetScript();
        time_controller_script.increment = true;
    }
}
