using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Microsoft.MixedReality.Toolkit.UI;

public class SimulateSunMoon : MonoBehaviour
{
    // Simulation flag
    private bool simulate = false;

    // Variables needed to calculate the trajectory of the sun (and moon)
    // Actually, do we want this to change at all. Why not affix it to Oxford?
    private float lattitude;
    private float longitude;

    // Sliders are needed to calculate the path of the sun (longitude and latitude)
    public GameObject lattitude_slider;
    public GameObject longitude_slider;

    // Store the time for sunrise and sunset
    private static double sunrise_time;
    private static double sunset_time;

    
    //Taken from: http://pointofint.blogspot.com/2014/06/sunrise-and-sunset-in-c.html
    private double JD = 0; //Julian date
    private int zone = 0; // Oxford time zone since we wish to show local time // This depends on longitude, no?
    //static double latitude = 47.6; // Seattle lat
    //static double longitude = -122.32; // Seattle lon //Rotation?
    private static bool dst = true; // Day Light Savings 
  

    void SetSunriseSunset()
    {
        // This methods sets the time for the sunrise and sunset
        // We can thus deduce the height of the sun and moon

        // Getting the current date
        DateTime date = GetDate();
        JD = Util.calcJD(date);  

        // Setting the time for sunrise/sunset
        sunrise_time = Util.calcSunRiseUTC(JD, lattitude, longitude);
        sunset_time  = Util.calcSunSetUTC(JD, lattitude, longitude);
    }

    private DateTime GetDate()
    { 
        int dayOfYear = (int)(longitude*365f/360f);
        int year = DateTime.Now.Year;
        return new DateTime(year, 1, 1).AddDays(dayOfYear - 1);
    }

    // Update is called once per frame
    void Update()
    {
        
        // Debug.Log("Test2");
        if (this.simulate) // Should only simulate one day, right?...
        {
            // Basic idea taken from: https://www.youtube.com/watch?v=DmhSWEJjphQ&t=100s
            // Need to model the motion more accurately using spherical coordinates (it's fine as it's an approximation)
            //  -> First, model the rotation of the celestial bodies
            //  -> Model their paths. i.e. how high up must they reach
            //      -> Use an elliptical model: x^2/b^2 + y^2/a^2 = 1
            //           -> b = sunsetTime - sunriseTime
            transform.RotateAround(Vector3.zero, Vector3.left, 10f * Time.deltaTime);
            transform.LookAt(Vector3.zero); // Ensure that the directional light is directed at me
        }
    }

    public void Print()
    {
        Debug.Log(lattitude.ToString());
    }

    public void UpdateLattitude()
    {
        PinchSlider pinch_slider_script = lattitude_slider.GetComponent<PinchSlider>();
        lattitude = (float)(pinch_slider_script.SliderValue * 360); 
    }

    public void UpdateLongitude()
    {
        PinchSlider pinch_slider_script = longitude_slider.GetComponent<PinchSlider>();
        longitude = (float)(pinch_slider_script.SliderValue * 360); 
    }

    public void StartSimulation()
    {
        simulate = true;
    }
}
