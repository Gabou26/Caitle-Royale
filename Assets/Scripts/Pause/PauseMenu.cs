using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private TransitionUI trans;

    public void ReturnMainMenu()
    {
        Time.timeScale = 1f;
        StartCoroutine(ReturnMenu());
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void DisconnectEveryone()
    {
        foreach (var player in FindObjectsOfType<PlayerInput>())
        {
            Destroy(player.gameObject);
        }
        PlayerManager.instance.players.Clear();
        ReturnMainMenu();
    }

    public void Show()
    {
        Time.timeScale = 0f;
        foreach (var button in buttons)
        {
            button.SetActive(true);
        }
    }

    public void Hide()
    {
        Time.timeScale = 1f;
        foreach (var button in buttons)
        {
            button.SetActive(false);
        }
    }

    IEnumerator ReturnMenu()
    {
        trans.Transition(new Vector2(0, 1200), new Vector2(0, 0), false);
        yield return new WaitForSeconds(0.6f);
        SceneManager.LoadScene("MainMenu");
    }
}
