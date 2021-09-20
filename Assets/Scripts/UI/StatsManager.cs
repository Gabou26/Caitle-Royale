using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UILifeRoyale;

public class StatsManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playerCount, topPick;
    Transform topPickTrans;

    private void Start()
    {
        playerCount.text = "";
    }

    public void ClearStats()
    {
        topPick.text = "Top Pick : Calculating...";
    }

    public void UpdateStats(List<PlayerRanking> players)
    {
        StartCoroutine(WaitUpdate(players));
    }

    public Transform GetTopPickTrans()
    {
        return topPickTrans;
    }

    string GetTopPick(List<PlayerRanking> players)
    {
        PlayerRanking best = new PlayerRanking();
        float valueBest = 0;
        float value;
        int count = 0;
        foreach (PlayerRanking player in players)
        {
            if (player.player == null)
                continue;
            if (!player.player.activeInHierarchy)
                continue;

            //Calcul Valeur
            value = 0;
            value += (player.score.damageScore * player.player.GetComponent<HealSystem>().GetHealthRatio());
            value += (player.score.killScore * 50);
            count++;

            if (value > valueBest)
            {
                valueBest = value;
                best = player;
                topPickTrans = player.player.transform;
            }
        }

        if (best.player == null)
        {
            topPickTrans = GetRandomAlive(players);
            return null;
        }        
        if (count > 1)
            return best.score.name;
        else if (count == 1)
            return null;
        else
            return "None";
    }

    Transform GetRandomAlive(List<PlayerRanking> players)
    {
        List<PlayerRanking> randoms = new List<PlayerRanking>();
        foreach (PlayerRanking player in players)
        {
            if (player.player == null)
                continue;
            if (!player.player.activeInHierarchy)
                continue;

            randoms.Add(player);
        }
        if (randoms.Count == 0) //Obt le meilleur joueur
        {
            return transform;
        }
        return randoms[Random.Range(0, randoms.Count)].player.transform;
    }

    void UpdateTopPick(List<PlayerRanking> players)
    {
        string top = GetTopPick(players);
        if (top != null)
            topPick.text = "Top Pick : " + top;
    }


    public int GetAliveCount(List<PlayerRanking> players)
    {
        int count = 0;
        foreach (PlayerRanking player in players)
        {
            if (player.player == null)
                continue;
            if (!player.player.activeInHierarchy && player.score.isPlayer)
                continue;
            count++;
        }
        return count;
    }

    void UpdateCount(List<PlayerRanking> players)
    {
        int count = GetAliveCount(players);
        playerCount.text = count + " Left";
    }

    IEnumerator WaitUpdate(List<PlayerRanking> players)
    {
        yield return new WaitForEndOfFrame();
        UpdateCount(players);
        UpdateTopPick(players);
    }
}
