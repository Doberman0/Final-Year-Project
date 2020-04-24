using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepRelativePosition : MonoBehaviour
{
    [SerializeField] GameObject game_object_to_keep_relative_to;

    [SerializeField] GameObject armillary_sphere;

    [SerializeField] GameObject my_game_object;

    [SerializeField] float x_displacement = 0; 

    [SerializeField] float y_displacement = 0;

    [SerializeField] float z_displacement = 0;

    // Update is called once per frame
    void Update()
    {
        BoxCollider bounding_box = game_object_to_keep_relative_to.GetComponent<BoxCollider>();
        if (bounding_box) 
        {

            // Altering the button's location
            my_game_object.transform.position = new Vector3( 
                bounding_box.transform.position.x + x_displacement,
                bounding_box.transform.position.y + y_displacement,
                armillary_sphere.transform.position.z); // Setting z to bounding box is pointless since it just moves it back smh

            // Altering the button's orientation to follow the boxes
            my_game_object.transform.rotation = Quaternion.Euler(
                bounding_box.transform.rotation.eulerAngles.x,
                bounding_box.transform.rotation.eulerAngles.y,
                bounding_box.transform.rotation.eulerAngles.z); 
        }
    }
}
