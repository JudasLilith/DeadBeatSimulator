using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    public Slider volumeSlider;

    // Start is called before the first frame update
    void Start()
    {
        // Sets saved volume if it has not been saved before
        if (!PlayerPrefs.HasKey("savedVolume"))
        {
            PlayerPrefs.SetFloat("savedVolume", 1);
        }
        Load();
    }

    // Set volume equal to slide value
    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
        Save();
    }

    // Load player's volume settings from previous game
    void Load()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("savedVolume");
    }

    // Save player's volume settings for each game
    void Save()
    {
        PlayerPrefs.SetFloat("savedVolume", volumeSlider.value);
    }
}
