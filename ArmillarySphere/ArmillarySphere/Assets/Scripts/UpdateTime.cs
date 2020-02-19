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
        // Initialising the clock
        //TextMesh time_text = time_display.GetComponent<TextMesh>();
        //time_text.text = current_hour.ToString();

        SetTimes();
    }

    private void SetTimes()
    {
        // Initialising the clock
        TextMesh time_text = time_display.GetComponent<TextMesh>();
        time_text.text = current_hour.ToString();

        // Updating sky simulator
        UltimateSky.UltimateSkyCalendar ultimate_sky_script = sky.GetComponent<UltimateSky.UltimateSkyCalendar>();
        ultimate_sky_script.hourMinSec = new Vector3(current_hour, 0f, 0f);
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
