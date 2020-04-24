using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Microsoft.MixedReality.Toolkit.UI;

public class ChangeDate : MonoBehaviour
{
    // Object to rotate - the armillary sphere
    [SerializeField] GameObject arm_sphere;

    // Skylight 
    public GameObject skylight_object;

    // Interaction between the sliders
    public GameObject date_slider;

    // To display the day as a number of degrees
    public GameObject date_UI;

    // Do display the date 
    [SerializeField] GameObject date_UI_display_date;

    public void UpdateDate() // Set date
    {
        // Extracting value from slider (you can only rotate by 360 degrees)
        PinchSlider pinch_slider_script = date_slider.GetComponent<PinchSlider>();
        float date_slider_val = (float)(365f*pinch_slider_script.SliderValue);

        // Rotating the armillary sphere
        arm_sphere.transform.localEulerAngles = new Vector3(
            arm_sphere.transform.localEulerAngles[0],
            arm_sphere.transform.localEulerAngles[1],
            -date_slider_val
            );

        // Get the date from the slider
        DateTime cur_date = GetDate(date_slider_val);

        // Updating the value on the label to match the day of the year
        TextMesh date_UI_textmesh = date_UI.GetComponent<TextMesh>();
        date_UI_textmesh.text = ((int)date_slider_val).ToString() + "°";

        // Updating the actual date that you can see
        TextMesh date_UI_textmesh_cur = date_UI_display_date.GetComponent<TextMesh>();
        //date_UI_textmesh_cur.text = ("Date: " + cur_date.ToString()).Substring(0,15);
        date_UI_textmesh_cur.text = "Date: " + cur_date.Day.ToString() + "/" + cur_date.Month.ToString() + "/" + cur_date.Year.ToString();

        // Setting the actual date for the sky to simulate
        UltimateSky.UltimateSkyCalendar ultimate_sky_script = skylight_object.GetComponent<UltimateSky.UltimateSkyCalendar>();
        ultimate_sky_script.yearMonthDay = new Vector3(cur_date.Year, cur_date.Month, cur_date.Day);

    }

    private DateTime GetDate(float num_of_days)
    {
        int dayOfYear = (int)num_of_days;
        int year = DateTime.Now.Year; // Design decision. 
        return new DateTime(year, 1, 1).AddDays(dayOfYear);
    }
}
