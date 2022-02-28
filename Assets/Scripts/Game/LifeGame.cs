using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LifeGame : GameManager
{
    public Dictionary<GameObject, int> playersLifes;
    
    public static int numberOfLife;
    [SerializeField] private TMP_Text[] lifeLabels;

    private int playerAlive;
    
    protected override void Awake()
    {
        base.Awake();
        playersLifes = new Dictionary<GameObject, int>();
    }

    public override void StartMatch()
    {
        if(!canStartGame)
            return;
        
        base.StartMatch();
        
        playersLifes.Clear();
        
        foreach (var player in playersScores)
            playersLifes.Add(player.Key, numberOfLife);

        ResetLife();
    }

    public bool LooseLife(GameObject player)
    {
        playersLifes[player]--;
        UpdateUILife();
        var hasLife = playersLifes[player] > 0;
        if (!hasLife)
            playerAlive--;
        return hasLife;
    }

    public void AddLife(GameObject player, int nb)
    {
        playersLifes[player] += nb;
    }

    public void CheckGame()
    {
        if (playerAlive <= 1)
            StopMatch(); // Game ends
    }

    private void UpdateUILife()
    {
        int i = 0;
        string playerName;
        foreach (var playerLife in playersLifes)
        {
            playerName = playersScores[playerLife.Key].name;

            lifeLabels[i].text = playerName + " :";

            for (int j = 0; j < playerLife.Value; j++)
                lifeLabels[i].text += " <sprite=0>";
            i++;
        }
    }

    protected override void StopMatch()
    {
        base.StopMatch();
        foreach (var lifeLabel in lifeLabels)
            lifeLabel.text = "";
    }

    private void ResetLife()
    {
        foreach (var player in playersScores)
            playersLifes[player.Key] = numberOfLife;

        UpdateUILife();
        playerAlive = playersLifes.Count;
    }
}
