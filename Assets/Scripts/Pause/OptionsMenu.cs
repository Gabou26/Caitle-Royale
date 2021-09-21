using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Button defaultButton;
    public AudioMixer musicMixer, SFXMixer;
    public Dropdown resDropdown;

    //Resolutions
    int[] baseResX = new int[5] { 640,1280,1920,2560,3840 };
    List<Resolution> resolutions;

    CanvasGroup cGroup;

    private void Start()
    {
        GetResolutions();
        cGroup = GetComponent<CanvasGroup>();
    }

    public void Open()
    {
        cGroup.alpha = 1;
        cGroup.blocksRaycasts = true;
        cGroup.interactable = true;

        EventSystem.current.SetSelectedGameObject(defaultButton.gameObject);
        //defaultButton.Select();
    }

    public void Close()
    {
        cGroup.alpha = 0;
        cGroup.blocksRaycasts = false;
        cGroup.interactable = false;
    }

    public void Return()
    {
        transform.parent.GetComponentInChildren<BasePause>().Open();
        Close();
    }

    public void SetMusicVol(float vol)
    {
        if (vol <= -45)
            vol = -80;
        musicMixer.SetFloat("volume", vol);
    }

    public void SetSFXVolume(float vol)
    {
        if (vol <= -45)
            vol = -80;
        SFXMixer.SetFloat("volume", vol);
    }

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
    }

    public void SetResolution(int resIndex)
    {
        Resolution res = resolutions[resIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    private void GetResolutions()
    {
        resolutions = new List<Resolution>();
        Resolution[] resArr = Screen.resolutions;

        foreach (Resolution res in resArr)
        {
            foreach (int dimX in baseResX)
            {
                if (res.width == dimX)
                {
                    resolutions.Add(res);
                    break;
                }
            }
        }

        Resolution screenRes = Screen.currentResolution;
        bool resPresente = false;
        foreach (Resolution res in resolutions)
        {
            if (res.width == screenRes.width && res.height == screenRes.height)
            {
                resPresente = true;
                break;
            }
        }
        if (!resPresente)
        {
            resolutions.Add(screenRes);
            resDropdown.options.Add(new Dropdown.OptionData(screenRes.width + "x" + screenRes.height));
        }
    }
}
