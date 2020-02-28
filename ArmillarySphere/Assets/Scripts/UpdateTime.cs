using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UpdateTime : MonoBehaviour
{
    public int current_hour;

    public bool increment; // true = increment, false = decrement 

    public GameObject time_display;

    public GameObject sky;

    public GameObject simulation_button;

    public void Start()
    {
        SetTimes();
    }

    private void SetTimes()
    {
        // Initialising the clock
        TextMesh time_text = time_display.GetComponent<TextMesh>();
        time_text.text = current_hour.ToString();

        // Updating sky simulator
        UltimateSky.UltimateSkyCalendar ultimate_sky_script = sky.GetComponent<UltimateSky.UltimateSkyCalendar>();
        ultimate_sky_script.hourMinSec = new Vector3(current_hour, ultimate_sky_script.hourMinSec[1], ultimate_sky_script.hourMinSec[2]);
    }

    public void ButtonPressed()
    {
        // This method allows other classes to reuse the UpdateCurrentTime method

        IterateThroughDay simulation_button_script = simulation_button.GetComponent<IterateThroughDay>();

        if (!simulation_button_script.run_simulation) // No updates allowed whilst the simulation is running
        {
            UpdateCurrentTime();
        }
    }
    public void UpdateCurrentTime()
    {

            if (increment)
            {
                current_hour = (current_hour + 1) % 24;
            }
            else
            {
                if (current_hour == 0)
                {
                    current_hour = 23;
                }   
                else
                {
                    current_hour--;
                } 

            }

            SetTimes();
        
    }

}
