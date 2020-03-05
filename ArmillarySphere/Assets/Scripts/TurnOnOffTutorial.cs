using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnOffTutorial : MonoBehaviour
{

    // To control whether or not you can see the video
    [SerializeField] bool video_on; 

    // The video gameobject
    [SerializeField] GameObject video_player;
    
    public void ToggleVideo()
    {
        // So the video ready to be destroyed/created as appropriate
        // you press the simulate button
        video_on = !video_on;

        // Make the video active/inactive
        video_player.SetActive(video_on);
    }
}
