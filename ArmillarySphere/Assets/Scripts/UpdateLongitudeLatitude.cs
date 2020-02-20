using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using UltimateSky;

public class UpdateLongitudeLatitude : MonoBehaviour
{
    // Skylight 
    public GameObject skylight_object;
    
    // Interaction between the sliders
    public GameObject longitude_slider;
    public GameObject latitude_slider;

    // To show the updated values
    public GameObject latitude_display;
    
    public void UpdateLattitude()
    {
        PinchSlider pinch_slider_script = latitude_slider.GetComponent<PinchSlider>();
        UltimateSky.UltimateSky ultimate_sky_script = skylight_object.GetComponent<UltimateSky.UltimateSky>();
        //ultimate_sky_script.SetLat((float)(pinch_slider_script.SliderValue * 180) - 90f);

        float lattitude_val = (float)(pinch_slider_script.SliderValue * 180) - 90f;
        
        // Updating the longitude of the skybox
        ultimate_sky_script.lonLat = new Vector2(ultimate_sky_script.lonLat[0], lattitude_val);

        // Updating text
        TextMesh lat_text = latitude_display.GetComponent<TextMesh>();
        lat_text.text = ((int)lattitude_val).ToString();
    }

    /*public void UpdateLongitude() // Do we even want this?
                                  // Does this even matter since we're modelling the movement of the sun
    {
        PinchSlider pinch_slider_script = latitude_slider.GetComponent<PinchSlider>();
        UltimateSky.UltimateSky ultimate_sky_script = skylight_object.GetComponent<UltimateSky.UltimateSky>();
        //ultimate_sky_script.SetLong((float)(pinch_slider_script.SliderValue * 180) - 90f);
    }*/
}
