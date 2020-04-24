using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using UltimateSky;

public class UpdateLongitudeLatitude : MonoBehaviour
{
    // What we need to rotate. i.e. the armillary sphere itself
    [SerializeField] GameObject arm_sphere;

    // Rotate around the earth in the armillary sphere
    [SerializeField] GameObject arm_sphere_earth;

    // Skylight 
    public GameObject skylight_object;
    
    // Interaction between the sliders
    public GameObject longitude_slider;
    public GameObject latitude_slider;

    // To show the updated values
    public GameObject latitude_display;
    
    public void UpdateLatitude()
    {
        PinchSlider pinch_slider_script = latitude_slider.GetComponent<PinchSlider>();
        UltimateSky.UltimateSky ultimate_sky_script = skylight_object.GetComponent<UltimateSky.UltimateSky>();
        //ultimate_sky_script.SetLat((float)(pinch_slider_script.SliderValue * 180) - 90f);

        float latitude_val = (float)(pinch_slider_script.SliderValue * 180) - 90f;

        // Rotating the armillary sphere
        arm_sphere.transform.localEulerAngles = new Vector3(
            latitude_val,
            arm_sphere.transform.localEulerAngles[1],
            arm_sphere.transform.localEulerAngles[2]
            );

        // Updating the latitude of the skybox
        ultimate_sky_script.lonLat = new Vector2(ultimate_sky_script.lonLat[0], latitude_val);

        // Updating text
        TextMesh lat_text = latitude_display.GetComponent<TextMesh>();
        lat_text.text = FormatLat(latitude_val);
    }

    private string FormatLat(float latitude_val)
    {
        string s = "";

        if (0 <= latitude_val && latitude_val <= 9)
        {
            s = "  ";
        } else if (10 <= latitude_val || (-9 <= latitude_val && latitude_val <= -1))
        {
            s = " ";
        }
        return "Latitude: " + s + ((int)latitude_val).ToString() + "°";
    }
}
