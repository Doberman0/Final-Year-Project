using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateArmillarySphere : MonoBehaviour
{
    [SerializeField] GameObject skylight_object;

    [SerializeField] GameObject arm_sphere;

    // Update is called once per frame
    void Update()
    {
        UltimateSky.UltimateSkyCalendar ultimate_sky_script = skylight_object.GetComponent<UltimateSky.UltimateSkyCalendar>();
        int hour = (int)ultimate_sky_script.hourMinSec[0];
        int minute = (int)ultimate_sky_script.hourMinSec[1];
        int second = (int)ultimate_sky_script.hourMinSec[2];

        int seconds = ToSeconds(hour, minute, second);

        int total_seconds = 24 * 60 * 60;

        float rotation_amount = ((float)seconds)/ ((float)total_seconds); 

        // Rotating the armillary sphere
        arm_sphere.transform.localEulerAngles = new Vector3(
            arm_sphere.transform.localEulerAngles[0],
            arm_sphere.transform.localEulerAngles[1],
            -rotation_amount
            );

    }

    private int ToSeconds(int hour, int minutes, int seconds)
    {
        // Converts the time (hr:min:secs) to just the number of seconds. Makes code neater
        return hour * 60 * 60 + minutes * 60 + seconds;
    }
}
