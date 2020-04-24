using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetIncrementDecrementMinute : MonoBehaviour
{
    [SerializeField] GameObject time_controller;

    [SerializeField] GameObject simulate_button;

    //private IterateThroughDay simulate_day_script;

    private UpdateMinute GetScript()
    {
        // Gets the script needed to increment/decrement the clock

        return time_controller.GetComponent<UpdateMinute>();
    }

    public void SetToDecrement()
    {
        if (!simulate_button.GetComponent<IterateThroughDay>().run_simulation)
        {
            UpdateMinute time_controller_script = GetScript();
            time_controller_script.increment = false;
        }
    }

    public void SetToIncrement()
    {
        if (!simulate_button.GetComponent<IterateThroughDay>().run_simulation)
        {
            UpdateMinute time_controller_script = GetScript();
            time_controller_script.increment = true;
        }
    }
}
