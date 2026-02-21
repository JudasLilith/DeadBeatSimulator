using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerManager : MonoBehaviour
{
    public Toggle toggle;
    public Color gray = new Color(0.35686274509f, 0.31764705882f, 0.29411764705f);

    // Start is called before the first frame update
    void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
        OnToggleValueChanged(toggle.isOn);
    }

    void OnToggleValueChanged(bool isOn)
    {
        // targetGraphic is the background image of the toggle
        if (toggle.targetGraphic != null)
        {
            toggle.targetGraphic.color = isOn ? gray : Color.white;
        }
    }
}
