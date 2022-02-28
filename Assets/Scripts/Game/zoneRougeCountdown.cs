using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class zoneRougeCountdown : MonoBehaviour
{
    [SerializeField] private Tilemap[] etages; //Rouge des insta-kills
    [SerializeField] private Tilemap[] foules; //Foule de chats
    [SerializeField] private Tilemap[] zonesNoir; //Plancher dessous les foules
    [SerializeField] private GameObject[] deathGates; //Collider des insta-kills
    private AudioSource audio;
    bool[] modeNoir;

    //Fade
    float fadeBase, fadeCible;
    float[] fadesCour;
    Color[] couls;

    void Start()
    {
        couls = new Color[etages.Length];
        fadesCour = new float[etages.Length];
        modeNoir = new bool[etages.Length];
        for (int c = 0; c < couls.Length; c++)
        {
            couls[c] = new Color(1, 1, 1, 0);
            modeNoir[c] = false;
            fadesCour[c] = 1;
        }
        audio = GetComponent<AudioSource>();
    }

    public void Activer(int duree, int etage)
    {
        StartCoroutine(Countdown(duree, etage));
    }

    void FadeNoir(int etage, float depart, float cible) {
        Fade(etage, depart, cible);
        this.modeNoir[etage] = true;  
    }

    void Fade(int etage, float depart, float cible)
    {
        this.modeNoir[etage] = false;
        fadeBase = depart;
        fadeCible = cible;
        fadesCour[etage] = 0;
    }

    private void Update()
    {
        for (int f = 0; f < fadesCour.Length; f++)
        {
            if (fadesCour[f] >= 1)
                continue;

            fadesCour[f] += Time.deltaTime * 2;
            couls[f].a = Mathf.Lerp(fadeBase, fadeCible, fadesCour[f]);
            if (modeNoir[f])
            {
                foules[f].color = couls[f];
                zonesNoir[f].color = couls[f];
            }
            else
                etages[f].color = couls[f];

            if (fadesCour[f] > 1)
            {
                if (modeNoir[f])
                    modeNoir[f] = false;
                fadesCour[f] = 1;
            }
        }
    }

    public void ResetFoules(int etage)
    {
        for (int i = 0; i < etage; i++)
        {
            couls[i].a = 0;
            etages[i].color = couls[i];
            foules[i].color = couls[i];
            zonesNoir[i].color = couls[i];
            fadesCour[i] = 1;
        }
        for (int i = etage; i < etages.Length; i++)
            FadeNoir(i, 0, 1);
    }

    IEnumerator Countdown(int duree, int etage)
    {
        WaitTimer wait = new WaitTimer();

        for (int i = duree - 1; i > 0; i--)
        {
            audio.Play();
            Fade(etage, couls[etage].a, 0.65f);

            wait.SetWaitTIme(1);
            yield return new WaitUntil(wait.WaitIfPlaying);

            Fade(etage, couls[etage].a, 0.05f);
            i--;
            if (i <= 0)
                continue;

            wait.SetWaitTIme(1);
            yield return new WaitUntil(wait.WaitIfPlaying);
        }
        Fade(etage, couls[etage].a, 0.8f);

        wait.SetWaitTIme(1);
        yield return new WaitUntil(wait.WaitIfPlaying);

        //Expl
        deathGates[etage].SetActive(true);
        yield return new WaitForSeconds(1);
        FadeNoir(etage, 0, 1);
        yield return new WaitForSeconds(1);
        Fade(etage, 1, 0);

        //Delai Death (empêche AI d'être pris l'autre côté (prévention ca brise jeu))
        wait.SetWaitTIme(10);
        yield return new WaitUntil(wait.WaitIfPlaying);
        deathGates[etage].SetActive(false);
    }
}
