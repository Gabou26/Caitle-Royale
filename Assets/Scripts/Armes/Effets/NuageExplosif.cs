using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NuageExplosif : MonoBehaviour
{
    //Champs
    public GameObject nuagePrefab;
    public float nbSpawn = 12;
    public float dureeVie = 1;
    public float dimension = 1;
    public Vector2 zoneSpawn = new Vector2(6, 6);
    
    void AjouterNuage()
    {
        var nuage = Instantiate(nuagePrefab);
        nuage.transform.localScale = new Vector2(dimension, dimension);
        Vector2 pos = new Vector2();
        pos.x = Random.Range(-zoneSpawn.x, zoneSpawn.x);
        pos.y = Random.Range(-zoneSpawn.y, zoneSpawn.y);
        nuage.GetComponent<Nuage>().Activer(dureeVie, pos + (Vector2)transform.position);
    }

    public void Exploser()
    {
        
        for (int i = 0; i < nbSpawn; i++)
        {
            AjouterNuage();
        }

        
    }

    
}
