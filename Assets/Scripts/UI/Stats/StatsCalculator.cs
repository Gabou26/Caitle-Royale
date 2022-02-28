using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StatsCalculator
{
    public enum StatType { MOSTKILLS, MOSTDMG, RANKING, GLOBAL }

    static void SetRank(Score score, StatType statType, int value)
    {
        switch (statType)
        {
            case StatType.MOSTKILLS:
                score.killsRanking = value;
                break;
            case StatType.MOSTDMG:
                score.dmgRanking = value;
                break;
            case StatType.RANKING:
                score.rankingScore = value;
                break;
            case StatType.GLOBAL:
                score.globalRanking = value;
                break;
            default:
                break;
        }
    }

    static int GetScore(Score score, StatType statType, int totalPlayers, int nbLifes)
    {
        switch (statType)
        {
            case StatType.MOSTKILLS:
                return score.killScore;
            case StatType.MOSTDMG:
                return score.damageScore;
            case StatType.RANKING:
                return totalPlayers - score.rankingScore;
            case StatType.GLOBAL:
                return GetGlobalScore(score, totalPlayers, nbLifes);
            default:
                return 0;
        }
    }

    public static Score[] SortRankings(Dictionary<GameObject, Score> playersScores, int count, int nbLifes, StatType typeData)
    {
        int totalPlayers = playersScores.Count;
        List<Score> playernotAIs = new List<Score>();
        Score[] sortScores = new Score[totalPlayers];
        for (int i = 0; i < totalPlayers; i++)
            sortScores[i] = new Score("None");


        Score temp;
        int scorePlayer; //Score comparaison selon type data
        int score2; //Score comparaison 2 selon type data
        foreach (Score playerScore in playersScores.Values)
        {
            if (playerScore.isPlayer) //If is real player
                playernotAIs.Add(playerScore);

            scorePlayer = GetScore(playerScore, typeData, totalPlayers, nbLifes);
            score2 = GetScore(sortScores[totalPlayers - 1], typeData, totalPlayers, nbLifes);
            if (scorePlayer > score2 || sortScores[totalPlayers - 1].name == "None") //Si entre dans classement
            {
                sortScores[totalPlayers - 1] = playerScore;
                for (int i = totalPlayers - 1; i > 0; i--)
                {
                    score2 = GetScore(sortScores[i - 1], typeData, totalPlayers, nbLifes);
                    if (scorePlayer > score2 || sortScores[i - 1].name == "None")
                    {
                        temp = sortScores[i]; //Swap/Sort elements of array
                        sortScores[i] = sortScores[i - 1];
                        sortScores[i - 1] = temp;
                    }
                    else //Stop sorting when value not good enough
                        break;
                }
            }
        }
        //Updating Ranks
        for (int i = 0; i < sortScores.Length; i++) 
            SetRank(sortScores[i], typeData, i + 1);

        //Fetch Top Unranked Player
        Score[] topScores;
        Score bestUnrankedPlayer = null;
        if (totalPlayers > count) //Add player rank if nbPlayers bigger than shown list
        {
            //Get Top player that isn't in top X
            bool playerInTop = false;
            foreach (Score player in playernotAIs)
            {
                for (int i = 0; i < totalPlayers; i++) //Pour optimiser : lighten up if type is ranking
                {
                    if (sortScores[i] == player && i < count) //Si dans top 10 
                    {
                        playerInTop = true;
                        break;
                    }
                        
                }
                //If player not in top X
                if (!playerInTop)
                {
                    if (bestUnrankedPlayer == null)
                        bestUnrankedPlayer = player;
                    else
                    {
                        scorePlayer = GetScore(player, typeData, totalPlayers, nbLifes);
                        score2 = GetScore(bestUnrankedPlayer, typeData, totalPlayers, nbLifes);
                        if (scorePlayer > score2)
                            bestUnrankedPlayer = player;
                    }
                }
            }
        }

        //Set Unranked if present
        if (bestUnrankedPlayer != null)
        {
            topScores = new Score[count + 1];
            topScores[count] = bestUnrankedPlayer;
        }
        else
            topScores = new Score[count];

        //Fetch Top X scores only (Add player if unranked)
        for (int i = 0; i < count; i++)
            topScores[i] = sortScores[i];
        if (bestUnrankedPlayer != null)
            topScores[count] = bestUnrankedPlayer;

        return topScores;
    }

    public static Score[] RatioTaken(Dictionary<GameObject, Score> playersScores, int count)
    {
        Score[] topScores = new Score[count];
        for (int i = 0; i < count; i++)
            topScores[i] = new Score("None");


        Score temp;
        foreach (var playerScore in playersScores)
        {
            if (playerScore.Value.GetRatioTaken() < topScores[count - 1].GetRatioTaken() || topScores[count - 1].name == "None") //Si entre dans classement
            {
                topScores[count - 1] = playerScore.Value;
                for (int i = count - 1; i > 0; i--)
                {
                    if (topScores[i].GetRatioTaken() < topScores[i - 1].GetRatioTaken() || topScores[i - 1].name == "None")
                    {
                        temp = topScores[i]; //Swap/Sort elements of array
                        topScores[i] = topScores[i - 1];
                        topScores[i - 1] = temp;
                    }
                }
            }
        }
        return topScores;
    }

    static int GetGlobalScore(Score player, int totalPlayers, int nbLifes)
    {
        //Set Global Score
        float facteurDMG = 10;
        float global = 0;
        global += player.damagehitsScore * 0.7f * facteurDMG;
        global -= player.takenhitsScore * 0.5f * facteurDMG;
        global += (player.killScore * 90);

        //Empeche val negative
        float minVal = player.damagehitsScore * 0.1f * facteurDMG;
        if (global < minVal) global = minVal; //0 si pas de tir :)

        global += player.GetRankingPoints(totalPlayers, nbLifes);

        player.globalScore = (int)(global * GetRatioGlobal(player)); //Ajout facteurs globaux
        return player.globalScore; 
    }

    static float GetRatioGlobal(Score player)
    {
        float globalRatio = 0.65f;

        //Ajout ratio Tir recu/donne
        float ratioTaken = player.GetRatioTaken();
        if (ratioTaken > 1.25f)
            globalRatio += 0.2f;
        else if (ratioTaken > 1.0f)
            globalRatio += 0.1f;

        //Ratio tirs atteignant leur cible
        if (player.GetRatioHits() > 0.5f)
            globalRatio += 0.1f;

        return globalRatio;
    }
}
