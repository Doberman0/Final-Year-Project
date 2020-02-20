using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Microsoft.MixedReality.Toolkit.UI;

public class ChangeDate : MonoBehaviour
{
    // Skylight 
    public GameObject skylight_object;

    // Interaction between the sliders
    public GameObject date_slider;

    public GameObject date_UI;

    public void UpdateDate() // Set date
    {
        // Extracting value from slider (you can only rotate by 360 degrees)
        PinchSlider pinch_slider_script = date_slider.GetComponent<PinchSlider>();
        float date_slider_val = (float)(365f*pinch_slider_script.SliderValue);

        // Get the date from the slider
        DateTime cur_date = GetDate(date_slider_val);

        // Updating the value on the label to match the day of the year
        TextMesh date_UI_textmesh = date_UI.GetComponent<TextMesh>();
        date_UI_textmesh.text = ((int)date_slider_val).ToString();

        // Setting the actual date for the sky to simulate
        UltimateSky.UltimateSkyCalendar ultimate_sky_script = skylight_object.GetComponent<UltimateSky.UltimateSkyCalendar>();
        ultimate_sky_script.yearMonthDay = new Vector3(cur_date.Year, cur_date.Month, cur_date.Day);

    }

    private DateTime GetDate(float num_of_days)
    {
        int dayOfYear = (int)num_of_days;
        int year = DateTime.Now.Year; // Design decision. Look to see how we can alter the year using a real armillary sphere
        return new DateTime(year, 1, 1).AddDays(dayOfYear - 1);
    }
}
