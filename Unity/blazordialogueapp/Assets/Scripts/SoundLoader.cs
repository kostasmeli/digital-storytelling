using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;
using UnityEngine.UI;

public class SoundLoader : MonoBehaviour
{
    /*
    public GameObject StartButton;
    int SoundEnabled;
    public void OnEnable()
    {
         SoundEnabled = 1; // Must Init The PlayerPrefs to 1 on load Scene
    }
    public void userToggle(bool tgl)
    {
        
        if (tgl)
        {
            //set sound variable on and start game with sound
            SoundEnabled = 1;
            PlayerPrefs.SetInt("sound", SoundEnabled);
            StartButton.SetActive(false);
            FMODUnity.RuntimeManager.LoadBank("Master");
            if (FMODUnity.RuntimeManager.HasBankLoaded("Master"))
            {
                StartButton.SetActive(true);
            }
        }
        else
        {
            //sound variable is off , start game without sound
            SoundEnabled = 0;
            PlayerPrefs.SetInt("sound", SoundEnabled);
        }
    }
    */
    public Toggle tgl;
    int SoundEnabled;
    public void submitVolume()
    {
        if(tgl.isOn)
        {
            //FMODUnity.RuntimeManager.LoadBank("Master");
            SoundEnabled = 1;
            PlayerPrefs.SetInt("sound", SoundEnabled);
        }
        else
        {
            SoundEnabled = 0;
            PlayerPrefs.SetInt("sound", SoundEnabled);
        }
    }
}   
