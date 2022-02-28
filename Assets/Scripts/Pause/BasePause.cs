using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BasePause : MonoBehaviour
{
    [SerializeField] private Button defaultButton;
    [SerializeField] private Text returnMenuText;

    CanvasGroup cGroup;

    private void Start()
    {
        cGroup = GetComponent<CanvasGroup>();
    }

    public void Open()
    {
        cGroup.alpha = 1;
        cGroup.blocksRaycasts = true;
        cGroup.interactable = true;

        defaultButton.Select();

        if (SceneManager.GetActiveScene().name == "MainMenu")
            returnMenuText.text = "Quit Game";
        else
            returnMenuText.text = "Return to Menu";
    }

    public void Close()
    {
        cGroup.alpha = 0;
        cGroup.blocksRaycasts = false;
        cGroup.interactable = false;
    }

    public void GoToOptions()
    {
        transform.parent.GetComponentInChildren<OptionsMenu>().Open();
        Close();
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        GetComponentInParent<NewPause>().ChangeScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        //Si bouton est "Quit", fermer le jeu.
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            GetComponentInParent<NewPause>().ChangeScene("Quit");
            return;
        }

        Time.timeScale = 1f;
        GetComponentInParent<NewPause>().ChangeScene("MainMenu");
    }
}
