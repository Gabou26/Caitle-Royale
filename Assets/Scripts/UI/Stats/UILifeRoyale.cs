using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UILifeRoyale : MonoBehaviour
{
    public struct PlayerRanking  { 
        public GameObject player; 
        public Score score;
        public UIPlayerName UI;
        public PlayerRanking(GameObject player, Score score, UIPlayerName UI)
        {
            this.player = player;
            this.score = score;
            this.UI = UI;
        }
    }

    const float offName = 19f;

    public StatsManager statsManager;
    public GameObject lifeUIPrefab;
    List<PlayerRanking> players = new List<PlayerRanking>();
    int topShown = 8;

    public void AddLifeUI(GameObject obj, Score score)
    {
        UIPlayerName playerUI = Instantiate(lifeUIPrefab, transform).GetComponent<UIPlayerName>();
        playerUI.player = obj;
        players.Add(new PlayerRanking(obj, score, playerUI));

        statsManager.UpdateStats(players);
        UpdatePlacement();
    }

    public void RemoveLifeUI(GameObject obj)
    {
        int id = FindStatID(obj);
        players[id].score.rankingScore = statsManager.GetAliveCount(players);

        statsManager.UpdateStats(players);

        StartCoroutine(WaitUpdate());
    }

    public void ClearLifeUI()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        players = new List<PlayerRanking>();
        statsManager.ClearStats();
        UpdatePlacement();
    }

    public void UpdateUI()
    {
        TMP_Text lblText;
        Score score;
        RoyaleGame manager = GameManager.instance as RoyaleGame;
        for (int i = 0; i < players.Count; i++)
        {
            lblText = players[i].UI.GetComponent<TMP_Text>();
            score = players[i].score;
            if (i < topShown || score.isPlayer)
            {
                lblText.text = "";
                if (score.isPlayer && i >= topShown)
                    lblText.text += "#" + (i+1) + ": ";
                if (score.isPlayer)
                    if (i < topShown)
                        lblText.color = new Color(0.5f, 1, 0.5f, 1);
                    else
                        lblText.color = new Color(1, 1, 1,1);

                lblText.text += score.name;
                lblText.text += " : " + score.damageScore;
                if (players[i].player == null)
                {
                    lblText.text += "-#" + score.rankingScore;
                    lblText.color = new Color(1, 1, 1, 0.6f);
                }
                else if ((score.isPlayer && !players[i].player.activeInHierarchy && manager.playersLifes[players[i].player] <= 0))
                {
                    lblText.text += "-#" + score.rankingScore;
                    lblText.color = new Color(0.4f, 1, 0.6f, 0.6f);
                }
            }
            else
                lblText.text = "";
        }
    }

    void UpdatePlacement()
    {
        int playerCount = 0;
        for (int i = 0; i < players.Count; i++)
        {
            if (i < topShown)
                players[i].UI.UpdatePos(new Vector2(0, i * -offName));
            else if (players[i].score.isPlayer)
            {
                players[i].UI.UpdatePos(new Vector2(0, (topShown + 1 + playerCount) * -offName));
                playerCount++;
            }
            else
                players[i].UI.UpdatePos(new Vector2(0, topShown * -offName));
        }
        UpdateUI();
    }

    int FindStatID(GameObject obj)
    {
        for (int i = 0; i < players.Count; i++)
            if (players[i].player == obj)
                return i;
            
        return -1;
    }

    void TriDMG(int id)
    {
        if (players == null || players.Count <= 0)
            return;

        if (id > 0 && players[id - 1].score.damageScore < players[id].score.damageScore)
        {
            PlayerRanking c = players[id - 1];
            players[id - 1] = players[id];
            players[id] = c;
            TriDMG(id - 1);
        }
    }

    public void UpdateStat(GameObject caster)
    {
        int playerID = FindStatID(caster);
        if (playerID >= 0)
            TriDMG(playerID);
        UpdatePlacement();
    }

    IEnumerator WaitUpdate()
    {
        yield return new WaitForEndOfFrame();
        UpdatePlacement();
    }

    public bool IsAnyPlayerAlive(RoyaleGame royaleManager)
    {
        foreach (PlayerRanking player in players)
        {
            if (player.player == null)
                continue;
            if (!player.player.activeInHierarchy && royaleManager.playersLifes[player.player] <= 0)
                continue;
            if(!player.score.isPlayer)
                continue;
            return true;
        }
        return false;
    }
}
