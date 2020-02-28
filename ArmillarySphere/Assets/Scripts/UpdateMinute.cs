using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UpdateMinute : MonoBehaviour
{
    public int current_minute;

    public bool increment; // true = increment, false = decrement 

    public GameObject time_display;

    public GameObject sky;

    public GameObject simulation_button;

    [SerializeField] GameObject hour_controller;

    public void Start()
    {
        // Initialising the clock
        SetTimes();
    }

    private void SetTimes()
    {
        // Initialising the clock
        TextMesh time_text = time_display.GetComponent<TextMesh>();
        // Just a spot of formatting (i.e. minutes under 10 should look like 0x. E.g. 1 minute should look like 01)
        if (current_minute < 10)
        {
            time_text.text = "0" + current_minute.ToString();
        }  else
        {
            time_text.text = current_minute.ToString();
        }

        // Updating sky simulator
        UltimateSky.UltimateSkyCalendar ultimate_sky_script = sky.GetComponent<UltimateSky.UltimateSkyCalendar>();
        ultimate_sky_script.hourMinSec = new Vector3(ultimate_sky_script.hourMinSec[0], current_minute, ultimate_sky_script.hourMinSec[2]);
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

    private void UpdateHour(bool increment)
    {
        UpdateTime hour_controller_script = hour_controller.GetComponent<UpdateTime>();
        hour_controller_script.increment = increment;
        hour_controller_script.UpdateCurrentTime();
    }

    public void UpdateCurrentTime()
    {

        if (increment)
        {
            current_minute = (current_minute + 1) % 60;

            if (current_minute == 0)
            {
                bool increment_hr = true;
                UpdateHour(increment_hr);
            }
        }
        else
        {
            if (current_minute == 0)
            {
                bool increment_hr = false;
                UpdateHour(increment_hr);

                current_minute = 59;
            }
            else
            {
                current_minute--;
            }
        }

        SetTimes();

    }

}
