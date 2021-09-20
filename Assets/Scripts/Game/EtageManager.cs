﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EtageManager : MonoBehaviour
{
    //Refs
    [SerializeField] private zoneRougeCountdown zoneRouge;
    [SerializeField] private Transform[] portes;
    [SerializeField] private AIManager aiManager;
    [SerializeField] private ZoneManager zoneManager;
    [SerializeField] private RoyaleGame royaleManager;
    [SerializeField] private EtageCountdown etageUI;
    [SerializeField] private HealEtage healEtage;

    //public Champs
    public int etageCour = 1;

    //Champs
    bool modeTransition = false;

    public void Initialiser(int etageDepart)
    {
        this.etageCour = etageDepart;
        ConfigEtage();
        zoneRouge.ResetFoules(etageDepart);
        for (int i = 0; i < portes.Length; i++)
            portes[i].gameObject.SetActive(true);
    }

    void Update()
    {
        if (modeTransition)
            return;

        if (FaibleNbJoueurs())
            StartCoroutine(ModeEtage());
    }

    bool FaibleNbJoueurs()
    {
        if (etageCour <= 0 || !royaleManager.isStart)
            return false;

        int playerCount = royaleManager.GetPlayerCount();
        int baseCount = royaleManager.nbCharacters;

        switch (etageCour)
        {
            case 1:
                if (playerCount <= Mathf.Ceil((baseCount / 6)))
                    return true;
                break;
            case 2:
                if (playerCount <= (baseCount / 3f) * 2)
                    return true;
                break;
            default:
                break;
        }
        return false;
    }

    void ConfigEtage()
    {
        //aiManager.SetEtage(etageCour);
        zoneManager.SetEtage(etageCour);
        royaleManager.SetEtage(etageCour);
    }

    IEnumerator ModeEtage()
    {
        int countdown = 10;
        if (etageCour == 2)
            countdown = 16;
        modeTransition = true;
        etageCour--;
        int etage = etageCour;
        //AV UI
        etageUI.Activer(countdown);
        zoneRouge.Activer(countdown, etage);

        //Ouvrir Portes
        portes[etage].gameObject.SetActive(false);
        healEtage.SetActiveEtage(etage, true);

        yield return new WaitForSeconds(1);
        //Changer Étage Mouv
        ConfigEtage();
        yield return new WaitForSeconds(countdown);
        //Fermer Portes
        portes[etage].gameObject.SetActive(true);
        healEtage.SetActiveEtage(etage, false);

        modeTransition = false;
    }
}
