using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class StartGame : MonoBehaviour
{
    bool has_started = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!has_started)
            {
                DialogueRunner dialogueRunner = FindObjectOfType<DialogueRunner>();
                if (!dialogueRunner.IsDialogueRunning)
                {
                    dialogueRunner.StartDialogue("Start");
                    has_started = true;
                }
            }
            else
            {
                return;
            }
        }
    }

}
