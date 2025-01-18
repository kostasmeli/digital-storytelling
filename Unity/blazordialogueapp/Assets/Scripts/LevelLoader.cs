
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;
    // public Button ClickButton;
    //FMOD.ChannelGroup mcg;

    public void Start()
    {
        //StartCoroutine(LoadSoundAsync()); 
    }
    public void LoadNextLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            StartCoroutine(LoadLevel(0));
        }
        else
        {
            StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
        }
        
    }

    IEnumerator LoadLevel(int levelIndex)
    {



        //Play Animation
        transition.SetTrigger("Start");

        //Wait
        yield return new WaitForSeconds(transitionTime);

        //LoadScene
        SceneManager.LoadScene(levelIndex);

    }
    /*
    IEnumerator LoadSoundAsync()
    {
        FMODUnity.RuntimeManager.LoadBank("Master", true);
        while (!FMODUnity.RuntimeManager.HaveAllBanksLoaded)
        {
            yield return null;
        }
        while (FMODUnity.RuntimeManager.AnySampleDataLoading())
        {
            yield return null;
        }
    }
    
    */
}
