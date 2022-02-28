using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionManager : MonoBehaviour
{
    [SerializeField] private GameObject[] gameModes;
    
    [SerializeField] private string[] AISceneMode;
    [SerializeField] private string[] PVPSceneMode;

    [SerializeField] private int numberLifeDefault;
    [SerializeField] private int numberMinutesDefault;
    [SerializeField] private int numberSecondsDefault;
    [SerializeField] private int nbCharsDefault;
    [SerializeField] private bool fillWithAI;

    private int currentIndex;
    private CharacterController2D[] players;

    //Transition
    [SerializeField] private TransitionUI transBleu;
    [SerializeField] private TransitionUI transUI;
    bool isStarting = false; //Empeche le double click sur start

    public void Start()
    {
        players = FindObjectsOfType<CharacterController2D>();
        foreach (var player in players)
        {
            player.gameObject.SetActive(false);
        }

        LifeGame.numberOfLife = numberLifeDefault;
        RoyaleGame.numberOfLife = numberLifeDefault;
        RoyaleGame.numberOfCharacters = nbCharsDefault;
        TimeGame.timeMatch = numberMinutesDefault * 60 + numberSecondsDefault;
        StartCoroutine(LoadTrans());
    }

    public void SetGameMode(int index)
    {
        gameModes[currentIndex].SetActive(false);
        currentIndex = index;
        gameModes[index].SetActive(true);
    }

    public void SetFillWithAI(int index)
    {
        fillWithAI = index == 0;
    }

    public void SetNumberPlayers(int index)
    {
        RoyaleGame.numberOfCharacters = GetNbPlayers(index);
    }
    
    public void SetLife(int index)
    {
        LifeGame.numberOfLife = index + 1;
        RoyaleGame.numberOfLife = index + 1;
    }
    
    public void SetTime(int index)
    {
        TimeGame.timeMatch = 30 * (index + 1);
    }

    public void LaunchMatch()
    {
        if (!isStarting)
            StartCoroutine(StartGame());
    }

    int GetNbPlayers(int index)
    {
        if (index == 0)
            return 4;
        if (index == 1)
            return 16;
        if (index == 2)
            return 32;
        if (index == 3)
            return 64;
        if (index == 4)
            return 100;
        if (index == 5)
            return 200;
        if (index == 6)
            return 300;
        if (index == 7)
            return 500;
        return 1000;
    }

    IEnumerator StartGame()
    {
        isStarting = true;
        transUI.Transition(new Vector2(-2000, 0), new Vector2(0,0), false);
        yield return new WaitForSeconds(0.4f);
        foreach (var player in players)
            player.gameObject.SetActive(true);

        SceneManager.LoadScene(fillWithAI ? AISceneMode[currentIndex] : PVPSceneMode[currentIndex]);
    }

    IEnumerator LoadTrans()
    {
        yield return new WaitForSeconds(0.1f);
        transBleu.Transition(new Vector2(400, 0), new Vector2(0, -1200), true);
    }
}
