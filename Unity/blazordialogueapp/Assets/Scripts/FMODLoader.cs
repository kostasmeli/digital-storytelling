using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using Yarn;

public class FMODLoader : MonoBehaviour
{
    //private EventInstance soundEvent;

    void Start()
    {
        /*
        FMOD.Debug.Initialize(FMOD.DEBUG_FLAGS.LOG);
        //Create an instance of the event
        soundEvent = FMODUnity.RuntimeManager.CreateInstance("event:/DialogueLines/MainSpeech");
        */
    }

    public void PlaySound(int currentid)
    {
        /*
        //Start playing the event
        soundEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        soundEvent.setParameterByName("Line", currentid);
        soundEvent.start();
        */
    }
}
