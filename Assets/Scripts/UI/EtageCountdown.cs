using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EtageCountdown : MonoBehaviour
{
    [SerializeField] private TMP_Text etageText;

    //Fade
    float fadeBase, fadeCible;
    float fadeCour = 1;
    CanvasGroup canvas;

    private void Start()
    {
        canvas = GetComponent<CanvasGroup>();
    }

    public void Activer(int duree)
    {
        StartCoroutine(Countdown(duree));
    }

    void Fade(float cible)
    {
        fadeBase = canvas.alpha;
        fadeCible = cible;
        fadeCour = 0;
    }

    private void Update()
    {
        if (fadeCour >= 1)
            return;

        fadeCour += Time.deltaTime * 2;
        canvas.alpha = Mathf.Lerp(fadeBase, fadeCible, fadeCour);
        if (fadeCour > 1)
            fadeCour = 1;
    }

    IEnumerator Countdown(int duree)
    {
        for (int i = duree - 1; i >= 0; i--)
        {
            Fade(0.95f);
            etageText.text = "Warning : The Gates are closing in " + i;
            yield return new WaitForSeconds(1);

            if (i > 0)
            {
                Fade(0.65f);
                i--;
                etageText.text = "Warning : The Gates are closing in " + i;
                yield return new WaitForSeconds(1);
            }
        }
        Fade(0);
    }
}
