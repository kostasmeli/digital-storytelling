using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using Yarn;

public class CharacterAnimation : MonoBehaviour
{
    public Animator TalkAnimationAdeimantos;
    public Animator TalkAnimationSokratis;
    
    

    [YarnCommand("TalkAdeimantos")]
    public void Talk_Adeimantos(int time)
    {
        //Set Both to idle state 
        TalkAnimationSokratis.Play("Idle_Socrates");
        TalkAnimationAdeimantos.Play("Idle_Adeimantos");
        TalkAnimationAdeimantos.SetInteger("Time", time);
        TalkAnimationAdeimantos.SetTrigger("IsTalking");
       
    }
    [YarnCommand("TalkSokratis")]
    public void Talk_Sokratis(int time)
    {
        //Set Both to idle state
        TalkAnimationSokratis.Play("Idle_Socrates");
        TalkAnimationAdeimantos.Play("Idle_Adeimantos");
        TalkAnimationSokratis.SetInteger("Time",time);
        TalkAnimationSokratis.SetTrigger("IsTalking");
    }
}
    