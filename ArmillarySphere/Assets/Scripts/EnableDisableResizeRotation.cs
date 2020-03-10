using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
//using Microsoft.MixedReality.Toolkit.Input;

public class EnableDisableResizeRotation : MonoBehaviour
{
    private bool enable = true;

    [SerializeField] GameObject UI;

    [SerializeField] GameObject ButtonsSliders;

    private void SetButtonsOnOff()
    {
        ButtonsSliders.SetActive(!enable);
    }

    private void AddResizeScript()
    {
        UI.AddComponent<BoundingBox>();
        //UI.AddComponent<NearInteractionGrabbable>();
        //UI.AddComponent<ManipulationHandler>();
        SetButtonsOnOff();
    }

    private void RemoveResizeScript() 
    {
        Destroy(UI.GetComponent<BoundingBox>());
        //Destroy(UI.GetComponent<NearInteractionGrabbable>());
        //Destroy(UI.GetComponent<ManipulationHandler>());
        SetButtonsOnOff();
    }

    public void ToggleScript()
    {
        if (enable)
        {
            AddResizeScript();
        } else
        {
            RemoveResizeScript();
        }

        // Debug.Log(enable);

        enable = !enable;
    }
}
