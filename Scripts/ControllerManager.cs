using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerManager : MonoBehaviour
{
    public Toggle toggle;
    public Color gray = new Color(0.35686274509f, 0.31764705882f, 0.29411764705f);
    public ControllerData controllerData;

    // Start is called before the first frame update
    void Start()
    {
        // Assign listener to detect when toggle is selected/unselected
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
        OnToggleValueChanged(toggle.isOn);
    }

    void OnToggleValueChanged(bool isOn)
    {
        // Change background color of checkmark when toggle is selected/unselected
        if (toggle.targetGraphic != null) // targetGraphic is the background image of the toggle
        {
            toggle.targetGraphic.color = isOn ? gray : Color.white;
        }

        // Update isWii variable (part of empty game object that won't destroy when scene changes)
        controllerData = GameObject.Find("ChosenController").GetComponent<ControllerData>();
        if (controllerData != null && gameObject.name == "WiiToggle")
        {
            controllerData.isWii = toggle.isOn;
            Debug.Log("isWii: " + controllerData.isWii);
        }
    }
}
