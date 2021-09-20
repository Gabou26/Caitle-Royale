using System;

[Serializable]
public class Score
{
    public int killScore;
    public int lifeLeft;
    public int deathScore;
    public int suicideScore;
    public int damageScore;
    public int rankingScore = 1;
    public string name;
    public bool isPlayer;

    public Score(string name, bool isPlayer = false)
    {
        this.name = name;
        this.isPlayer = isPlayer;
    }
}