using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BasePause : MonoBehaviour
{
    [SerializeField] private Button defaultButton;

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

        EventSystem.current.SetSelectedGameObject(defaultButton.gameObject);
        //defaultButton.Select();
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
        Time.timeScale = 1f;
        GetComponentInParent<NewPause>().ChangeScene("MainMenu");
    }
}
