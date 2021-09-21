using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewPause : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    public TransitionUI trans;

    RectTransform rect;
    CanvasGroup cGroup;
    float vitPauseMouv;

    public bool open = false;
    Vector2 posDepart, posFin;
    float delaiCour = 1;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        cGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        //Transition
        if (delaiCour >= 1)
            return;

        delaiCour += Time.deltaTime * vitPauseMouv;
        if (delaiCour > 1)
        {
            if (open)
                Time.timeScale = 0.03f;
            delaiCour = 1;
        }
            

        rect.position = Vector2.Lerp(posDepart, posFin, delaiCour);
        if (open)
            cGroup.alpha = delaiCour;
        else
            cGroup.alpha = 1 - delaiCour;
    }

    public void ChangeScene(string sceneName)
    {
        Hide();
        StartCoroutine(ChangeCurrentScene(sceneName));
    }

    public void Show()
    {
        if (SceneManager.GetActiveScene().name == "GameOption")
            return;

        open = true;
        cGroup.interactable = true;
        CharacterController2D.paused = true;
        WeaponManager.paused = true;

        vitPauseMouv = 20f;
        posDepart = new Vector2(-300, 0);
        posFin = new Vector2(0, 0);
        delaiCour = 0;
        Time.timeScale = 0.3f;

        GetComponentInChildren<BasePause>().Open();
        GetComponentInChildren<OptionsMenu>().Close();
    }

    public void Hide()
    {
        cGroup.interactable = false;
        CharacterController2D.paused = false;
        WeaponManager.paused = false;
        open = false;

        vitPauseMouv = 8f;
        posFin = new Vector2(-300, 0);
        posDepart = rect.position;
        delaiCour = 0;
        Time.timeScale = 1f;
    }

    public void DisconnectEveryone()
    {
        foreach (var player in FindObjectsOfType<PlayerInput>())
        {
            Destroy(player.gameObject);
        }
        PlayerManager.instance.players.Clear();
    }

    IEnumerator ChangeCurrentScene(string scene)
    {
        if (scene == "MainMenu")
            DisconnectEveryone();
        trans.Transition(new Vector2(0, 1200), new Vector2(0, 0), false);
        yield return new WaitForSeconds(0.6f);
        SceneManager.LoadScene(scene);
    }
}
