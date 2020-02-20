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

    public void Start()
    {
        // Initialising the clock
        //TextMesh time_text = time_display.GetComponent<TextMesh>();
        //time_text.text = current_hour.ToString();

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
    public void UpdateCurrentTime()
    {

        //IterateThroughDay simulation_button_script = simulation_button.GetComponent<IterateThroughDay>();

        //if (!simulation_button_script.run_simulation) // No updates allowed whilst the simulation is running
        //{
        if (increment)
        {
            current_minute = (current_minute + 1) % 60;
        }
        else
        {
            if (current_minute == 0)
            {
                current_minute = 59;
            }
            else
            {
                current_minute--;
            }
            //current_hour = (current_hour - 1) % 24; // Mod doesn't work the same way as it does in mathematics
        }

        SetTimes();

        // Updating sky simulator
        //UltimateSky.UltimateSkyCalendar ultimate_sky_script = sky.GetComponent<UltimateSky.UltimateSkyCalendar>();
        //ultimate_sky_script.hourMinSec = new Vector3(current_hour, 0f, 0f);

        // Updating text shown
        //TextMesh time_text = time_display.GetComponent<TextMesh>();
        //time_text.text = current_hour.ToString();
        //}

    }

}
