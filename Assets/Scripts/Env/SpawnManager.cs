using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager
{
    List<Transform> spawns, listeZones;

    public List<Transform> ObtRandSpawns(int nbChars, Transform etageZones)
    {
        ObtListSpawns(nbChars, etageZones);
        RandomizeArray();

        return spawns;
    }

    void ObtListSpawns(int nbChars, Transform etageZones)
    {
        spawns = new List<Transform>();

        int nbZones = etageZones.childCount;
        float interZone = (float)nbChars / (float)nbZones;

        float interCour = 0;
        int charCour = 0;
        int randZoneId;
        int indexChild = 0;
        Transform randZone, randSubZone;
        for (int z = 0; z < nbZones; z++)
        {
            listeZones = new List<Transform>();
            foreach (Transform child in etageZones.GetChild(z))
                if (child.tag != "DoNotSpawn")
                    listeZones.Add(child);

            while (interCour < interZone)
            {
                interCour += 1;
                randZoneId = Random.Range(0, listeZones.Count);
                randZone = listeZones[randZoneId];
                randSubZone = randZone.GetChild(indexChild % randZone.childCount);

                spawns.Add(randSubZone);
                listeZones.RemoveAt(randZoneId);

                if (listeZones.Count <= 0)
                {
                    foreach (Transform child in etageZones.GetChild(z))
                        if (child.tag != "DoNotSpawn")
                            listeZones.Add(child);
                    indexChild++;
                }
            }
            interCour -= interZone;
        }
    }

    void RandomizeArray()
    {
        //Randomize la liste spawns
        listeZones = new List<Transform>();
        foreach (Transform spawn in spawns)
            listeZones.Add(spawn);

        int count = listeZones.Count;
        int randCount;
        for (int i = 0; i < count; i++)
        {
            randCount = Random.Range(0, listeZones.Count);
            spawns[i] = listeZones[randCount];
            listeZones.RemoveAt(randCount);
        }
    }
}
