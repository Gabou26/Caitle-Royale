using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasPause : MonoBehaviour
{
    [HideInInspector] public static CanvasPause instance;


    void Awake()
    {
        CanvasPause[] pauseMenu = FindObjectsOfType<CanvasPause>();

        if (pauseMenu.Length > 1)
        {

            Destroy(this.gameObject);
            instance.GetComponentInChildren<TransitionUI>().GetComponent<Image>().enabled = false;
            return;
        }
        
        DontDestroyOnLoad(this.gameObject);
        instance = this;
    }

}
