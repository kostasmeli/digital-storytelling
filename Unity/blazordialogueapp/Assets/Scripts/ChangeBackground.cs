using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class ChangeBackground : MonoBehaviour
{
    public GameObject background1;
    public GameObject background2;
    // Start is called before the first frame update
    [YarnCommand("LoadBackground")]
    public void LoadNewBackground(int number)
    {
           if(number==1)
        {
            background2.SetActive(false);
            background1.SetActive(true);

        }
           else
        {
            background1.SetActive(false);
            background2.SetActive(true);
        }
    }
}
