using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SimulateSunMoon : MonoBehaviour
{

    private bool simulate = false;

    /*
        //Taken from: http://pointofint.blogspot.com/2014/06/sunrise-and-sunset-in-c.html
        static double JD = 0;
        static int zone = -8; // Seattle time Zone
        static double latitude = 47.6; // Seattle lat
        static double longitude = -122.32; // Seattle lon //Rotation?
        static bool dst = true; // Day Light Savings 
        static DateTime date = DateTime.Today;
 
        static void Main(string[] args)
        {
            JD = Util.calcJD(date);  //OR   JD = Util.calcJD(2014, 6, 1);
            double sunRise = Util.calcSunRiseUTC(JD, latitude, longitude);
            double sunSet = Util.calcSunSetUTC(JD, latitude, longitude);
            System.Console.WriteLine(Util.getTimeString(sunRise, zone, JD, dst));
            System.Console.WriteLine(Util.getTimeString(sunSet, zone, JD, dst));
            System.Console.WriteLine("");
            System.Console.WriteLine(Util.getDateTime(sunRise, zone, date, dst).Value.ToString());
            System.Console.WriteLine(Util.getDateTime(sunSet, zone, date, dst).Value.ToString());
        }
         
    */
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
            //      -> 
            transform.RotateAround(Vector3.zero, Vector3.left, 10f * Time.deltaTime);
            transform.LookAt(Vector3.zero);
        }
    }

    public void Print()
    {
        Debug.Log("Test");
    }

    public void StartSimulation()
    {
        simulate = true;
    }
}
