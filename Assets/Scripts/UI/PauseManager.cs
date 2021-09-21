using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    private void OnPause()
    {
        NewPause pauseMenu = FindObjectOfType<NewPause>();
        if (!pauseMenu.open)
        {
            pauseMenu.Show();
        }
        else
        {
            pauseMenu.Hide();
        }

        /*var pauseMenu = FindObjectOfType<PauseMenu>();
        if (pauseMenu.GetComponent<CanvasGroup>().alpha <= 0f)
        {
            pauseMenu.GetComponent<CanvasGroup>().alpha = 1f;
            pauseMenu.Show();
        }
        else
        {
            pauseMenu.GetComponent<CanvasGroup>().alpha = 0f;
            pauseMenu.Hide();
        }*/
    }
}
