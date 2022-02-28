using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIEndResult : MonoBehaviour
{
    TMP_Text[] resultLabels;
    TMP_Text[] valueLabels;
    public GameObject lifeUIPrefab;

    const float offName = 20f;
    public const int playerCount = 10;

    void Start()
    {
        resultLabels = new TMP_Text[playerCount + 2];
        valueLabels = new TMP_Text[playerCount + 2];
        RectTransform rect;
        GameObject obj;
        for (int i = 0; i < resultLabels.Length; i++)
        {
            obj = Instantiate(lifeUIPrefab, transform);
            resultLabels[i] = obj.transform.GetChild(0).GetComponent<TMP_Text>();
            valueLabels[i] = obj.transform.GetChild(1).GetComponent<TMP_Text>();

            rect = obj.GetComponent<RectTransform>();
            rect.localPosition = new Vector2(0, i * -offName);
            rect.localPosition -= (Vector3)rect.sizeDelta / 2;
        }
        resultLabels[playerCount].text = "";
        valueLabels[playerCount].text = "";
    }

    void ShowResults(Score[] scores)
    {
        int count = scores.Length > playerCount ? playerCount : scores.Length;

        for (int i = 0; i < count; i++)
        {
            resultLabels[i].text = "";
            valueLabels[i].text = "";

            if (scores[i].rankingScore == 1) //Blue
            {
                resultLabels[i].color = new Color(0.5f, 0.5f, 1, 1);
                valueLabels[i].color = new Color(0.5f, 0.5f, 1f, 1);
            }
            else if (scores[i].isPlayer) //Green
            {
                resultLabels[i].color = new Color(0.5f, 1, 0.5f, 1);
                valueLabels[i].color = new Color(0.5f, 1, 0.5f, 1);
            }
            else //White
            {
                resultLabels[i].color = new Color(1, 1, 1, 1);
                valueLabels[i].color = new Color(1, 1, 1, 1);
            }
        }
        for (int i = count; i < resultLabels.Length; i++) //Hide obsolete text when playercount is small
        {
            resultLabels[i].text = "";
            valueLabels[i].text = "";
        }
    }

    public void ShowDMG(Score[] scores)
    {
        int count = scores.Length > playerCount ? playerCount : scores.Length;

        ShowResults(scores);
        for (int i = 0; i < count; i++)
        {
            resultLabels[i].text = (i + 1) + ". " + scores[i].name;
            valueLabels[i].text = scores[i].damageScore.ToString();
        }
        if (scores.Length > playerCount)
        {
            int length = resultLabels.Length - 1;
            Score player = scores[scores.Length - 1];
            resultLabels[length].text = player.dmgRanking + ". " + player.name;
            valueLabels[length].text = player.damageScore.ToString();
            resultLabels[length].color = new Color(0.5f, 1, 0.5f, 1);
            valueLabels[length].color = new Color(0.5f, 1, 0.5f, 1);
        }
    }

    public void ShowKills(Score[] scores)
    {
        int count = scores.Length > playerCount ? playerCount : scores.Length;

        ShowResults(scores);
        for (int i = 0; i < count; i++)
        {
            resultLabels[i].text = (i + 1) + ". " + scores[i].name;
            valueLabels[i].text = scores[i].killScore.ToString();
        }
        if (scores.Length > playerCount)
        {
            int length = resultLabels.Length-1;
            Score player = scores[scores.Length - 1];
            resultLabels[length].text = player.killsRanking + ". " + player.name;
            valueLabels[length].text = player.killScore.ToString();
            resultLabels[length].color = new Color(0.5f, 1, 0.5f, 1);
            valueLabels[length].color = new Color(0.5f, 1, 0.5f, 1);
        }
    }

    public void ShowRanking(Score[] scores)
    {
        int count = scores.Length > playerCount ? playerCount : scores.Length;

        ShowResults(scores);
        for (int i = 0; i < count; i++)
        {
            resultLabels[i].text = scores[i].name;
            valueLabels[i].text = "#" + scores[i].rankingScore;
        }
        if (scores.Length > playerCount)
        {
            int length = resultLabels.Length - 1;
            Score player = scores[scores.Length - 1];
            resultLabels[length].text = player.name;
            valueLabels[length].text = "#" + player.rankingScore;
            resultLabels[length].color = new Color(0.5f, 1, 0.5f, 1);
            valueLabels[length].color = new Color(0.5f, 1, 0.5f, 1);
        }
    }

    public void ShowGlobal(Score[] scores)
    {
        int count = scores.Length > playerCount ? playerCount : scores.Length;

        ShowResults(scores);
        for (int i = 0; i < count; i++)
        {
            resultLabels[i].text = (i + 1) + ". " + scores[i].name;
            valueLabels[i].text = scores[i].globalScore.ToString();
        }
        if (scores.Length > playerCount)
        {
            int length = resultLabels.Length - 1;
            Score player = scores[scores.Length - 1];
            resultLabels[length].text = player.globalRanking + ". " + player.name;
            valueLabels[length].text = player.globalScore.ToString();
            resultLabels[length].color = new Color(0.5f, 1, 0.5f, 1);
            valueLabels[length].color = new Color(0.5f, 1, 0.5f, 1);
        }
    }
}
