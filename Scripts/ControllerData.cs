using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerData : MonoBehaviour
{
    public bool isWii = true;

    // Prevent game object from being destroyed when scene changes
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
