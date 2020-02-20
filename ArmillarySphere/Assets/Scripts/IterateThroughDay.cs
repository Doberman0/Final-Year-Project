using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class IterateThroughDay : MonoBehaviour
{
    // Ultimate sky object
    public GameObject increment_hour_button;

    // State control mechanism whereby you can only run the simulation once
    // A "semaphore" for coroutines, if you will
    public bool run_simulation = false;

    public IEnumerator InvokeMethod(Action method, float interval, int invokeCount)
     {
        // This method actually increments the time itself
        // I have decided to keep this generic in case I need to use this method later
        // on for any other reason.

        run_simulation = true;

        for (int i = 0; i < invokeCount; i++)
        {
             method();
 
             yield return new WaitForSeconds(interval);
        }

        run_simulation = false;
     }

    public void SimulateDay()
    {
        // Get the increment button
        UpdateTime time_text_script = increment_hour_button.GetComponent<UpdateTime>();
        time_text_script.increment = true; // Ensures that you increment a day
        Action update_time = () => time_text_script.UpdateCurrentTime(); // Update function is a function which executes in and of
                                                                         // itself thus you need to convert it into an action 
                                                                         // for the coroutine.

        // Constants I need - makes code clearer
        int hour = 1;
        int hrs_in_a_day = 24;

        // Running the simulation itself
        if (!run_simulation) // Simulation is atomic
        {
            StartCoroutine(InvokeMethod(update_time, hour, hrs_in_a_day));
        }
        
    }

}
