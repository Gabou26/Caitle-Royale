using System;

[Serializable]
public class Score
{
    public int killScore;
    public int lifeLeft;
    public int deathScore;
    public int suicideScore;
    public int damageScore;
    public int globalScore;
    public string name;
    public bool isPlayer;

    //Valeurs Ranking
    public int rankingScore = 1;
    public int killsRanking;
    public int dmgRanking;
    public int globalRanking;

    //Facteurs Globaux
    public int takenhitsScore;
    public int damagehitsScore;
    public int totalShots; //Nombre de tirs fait. Sert à déterminer ratio


    public Score(string name, bool isPlayer = false)
    {
        this.name = name;
        this.isPlayer = isPlayer;
    }

    public int GetRankingPoints(int playerCount, int nbLifes) { 
        return rankingScore <= 10 ? (11 - rankingScore + 1) * (int)(playerCount * 0.61f * nbLifes) + (playerCount/2) : 0; 
    }
    public float GetRatioHits() {
        if (totalShots == 0) return 0;
        return (float)damagehitsScore / (float)totalShots; 
    }
    public float GetRatioTaken() {
        if (takenhitsScore == 0) return 1000; //1ere position nohit :))) (achievement lol)
        return (float)damagehitsScore / (float)takenhitsScore; 
    }
}