using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePlayerText : MonoBehaviour
{
    Text texte;
    CanvasGroup canvas;
    string message;
    float value;

    private void Start()
    {
        texte = GetComponent<Text>();
        canvas = GetComponent<CanvasGroup>();
    }

    public void FlashText(string message, float value)
    {
        this.message = message;
        this.value = value;
        StartCoroutine(FlashTextCo());
    }

    IEnumerator FlashTextCo()
    {
        string messDepart = message;
        texte.text = message;
        canvas.alpha = 0.9f;

        texte.enabled = true;
        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(0.04f);
            if (message != messDepart)
                yield break;

            canvas.alpha = (canvas.alpha + 0.5f) % 1;
        }
        if (message != messDepart)
            yield break;

        yield return new WaitForSeconds(1.5f);
        texte.enabled = false;
    }
}
