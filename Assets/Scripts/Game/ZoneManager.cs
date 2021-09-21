using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public LayerMask collisionMask;
    //public Transform zoneEtage;

    //Gestion Etage
    int etageCour = 1;
    List<List<Transform>> zonesEtage;

    //Temp vals
    RaycastHit2D hit;

    private void Awake()
    {
        CreateZonesList();
    }

    //Création de la liste de zones, triée.
    void CreateZonesList()
    {
        zonesEtage = new List<List<Transform>>();
        Transform child;
        Transform childZone;
        for (int i = 0; i < transform.childCount; i++)
        {
            zonesEtage.Add(new List<Transform>());
            child = transform.GetChild(i);
            for (int n = 0; n < child.childCount; n++)
            {
                childZone = child.GetChild(n);
                for (int x = 0; x < childZone.childCount; x++)
                    zonesEtage[i].Add(childZone.GetChild(x));
            }
        }
    }

    public void SetEtage(int etageId)
    {
        this.etageCour = etageId;
    }

    public List<Transform> ObtEtageCour()
    {
        return zonesEtage[etageCour];
    }

    public Transform ObtProchZone(Transform zoneCour, Transform baseChar, Transform target)
    {
        List<Transform> zonesAdj = new List<Transform>();

        zonesAdj.AddRange(ObtListeZonesEtage(etageCour, zoneCour, baseChar));

        //Ajoute zoneJoueur et change cible si joueur doit changer d'étage (royale)
        int etageZone = ObtEtageId(zoneCour);
        bool targetMid = etageCour != etageZone ? true : false;
        if (targetMid)
        {
            List<Transform> etageCible = ObtListeZonesEtage(etageZone, zoneCour, baseChar);
            zonesAdj.AddRange(etageCible);
            target = transform;
        }


        if (zonesAdj.Count == 0)
            return null;
        return ObtBestZone(zonesAdj, baseChar, target, targetMid);
    }

    int ObtEtageId(Transform zone)
    {
        if (zone == null)
            return etageCour;

        Transform zoneEtage = zone.parent.parent;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i) == zoneEtage)
                return i;
        }
        return etageCour;
    }

    public bool DansEtage(Transform zone)
    {
        int etageZone = ObtEtageId(zone);
        if (etageZone != etageCour)
            return false;
        return true;
    }

    List<Transform> ObtListeZonesEtage(int etageId, Transform zoneCour, Transform baseChar)
    {
        List<Transform> zonesAdj = new List<Transform>();

        Transform childZone;
        for (int i = 0; i < zonesEtage[etageId].Count; i++)
        {
            childZone = zonesEtage[etageId][i];
            if (!childZone.gameObject.activeInHierarchy || childZone == zoneCour)
                continue;
            if (Vector2.Distance(baseChar.position, childZone.position) > 25)
                continue;

            hit = Physics2D.Linecast(baseChar.position, childZone.position, collisionMask);
            if (hit.collider == null)
                zonesAdj.Add(zonesEtage[etageId][i]);
        }

        return zonesAdj;
    }

    private Transform ObtBestZone(List<Transform> zonesAdj, Transform baseChar, Transform target, bool targetMid)
    {
        if (zonesAdj == null || zonesAdj.Count == 0)
        {
            Debug.LogError("Erreur Recherche");
            return target;
        }
        Transform bestZone = zonesAdj[0];

        Vector2 posTarget = target.position;
        int idZoneTarget = ObtZoneProche(posTarget);
        Transform zoneProcheTarget = ObtSubZoneProche(idZoneTarget, posTarget);

        float bestDist = int.MaxValue;
        float health = 1, selfHealth = 1;
        if (!targetMid)
        {
            health = target.GetComponent<HealSystem>().GetHealthRatio();
            selfHealth = baseChar.GetComponent<HealSystem>().GetHealthRatio();
        }
        for (int i = 0; i < zonesAdj.Count; i++)
        {          
            float dist = Vector2.Distance(posTarget, zonesAdj[i].position);

            //Verif si voit cible
            if (target.gameObject.layer == 8)
            {
                hit = Physics2D.Linecast(zonesAdj[i].position, posTarget, collisionMask);
                if (hit.collider == null)
                    dist -= 8;
            }

            if (!targetMid)
            {
                //dist += CalculerDistZone(idZoneTarget, zonesAdj[i].parent); //Ajout Facteur dist zone
                if (selfHealth > 0.25f && (zonesAdj[i] == zoneProcheTarget || dist < 8))
                {
                        if (health <= 0.4f)
                            dist += 4;
                        else if (health <= 0.25f)
                            dist += 1;
                        else
                            dist += 6;
                }
                else if (selfHealth <= 0.25f && dist < 8)
                {
                    if (zonesAdj[i] == zoneProcheTarget)
                        dist += 8;

                    if (health <= 0.3f)
                        dist += 2;
                    else if (health <= 0.45f)
                        dist += 8;
                    else
                        dist += 11;
                }
            }

            if (dist < bestDist)
            {
                bestDist = dist;
                bestZone = zonesAdj[i];
            }
        }
        return bestZone;
    }

    Transform ObtSubZoneProche(int zoneId, Vector2 posTarget)
    {
        Transform child = transform.GetChild(etageCour).GetChild(zoneId);
        float distBest = Vector2.Distance(posTarget, child.GetChild(0).position);
        int index = 0;
        float dist;
        for (int n = 1; n < child.childCount; n++)
        {
            dist = Vector2.Distance(posTarget, child.GetChild(n).position);
            if (dist < distBest)
            {
                index = n;
                distBest = dist;
            }
        }
        return child.GetChild(index);
    }

    public int ObtZoneProche(Vector2 posTarget)
    {
        Transform child = transform.GetChild(etageCour);
        float distBest = Vector2.Distance(posTarget, child.GetChild(0).position);
        int index = 0;
        float dist;
        for (int n = 1; n < child.childCount; n++)
        {
            dist = Vector2.Distance(posTarget, child.GetChild(n).position);
            if (dist < distBest)
            {
                index = n;
                distBest = dist;
            }
        }
        return index;
    }

    private int CalculerDistZone(int idZoneTarget, Transform zoneGroupe)
    {
        Transform child = transform.GetChild(etageCour);
        int idZone = idZoneTarget;
        //Obtenir ID zoneGroupe
        for (int n = 0; n < child.childCount; n++)
        {
            if (child.GetChild(n) == zoneGroupe)
            {
                idZone = n;
                break;
            }
        }

        //Calcul Distance optimale
        int dFront, dBack;
        if (idZone < idZoneTarget)
        {
            dFront = idZoneTarget - idZone;
            dBack = (idZone + child.childCount) - idZoneTarget;
        }
        else if (idZone == idZoneTarget)
            return 0;
        else
        {
            dFront = idZone - idZoneTarget;
            dBack = (idZoneTarget + child.childCount) - idZone;
        }

        if (dFront < dBack)
            return dFront * 40;
        else
            return dBack * 40;
    }

    /*public Transform[] GetSpawnList(int nbCharacters, int etage)
    {
        Vector2[] spawns = new Vector2[nbCharacters];
        Transform zoneEtage = zonesEtage[etage];
        float intervalle = (float)nbCharacters / (float)zonesEtage[etage].Count;
        List<List<Transform>> zones = new List<List<Transform>>();

        for (int i = 0; i < zonesEtage[etage].Count; i++)
            zones.Add(new List<Transform>());

        int intCour;
        Transform zone;
        for (int i = 0; i < nbCharacters; i++)
        {
            intCour = (int)(i / intervalle);

            if (zones[intCour].Count <= 0)
            {
                for (int i = 0; i < zone.childCount; i++)
                    zones[intCour].Add(zone.GetChild(i));
            }
        }


        return spawns;
    }*/


    //Editor
    private void OnDrawGizmos()
    {
        foreach (GameObject zone in GameObject.FindGameObjectsWithTag("Zone"))
        {
            Gizmos.color = Color.blue;
            if (zone == Selection.activeGameObject)
                Gizmos.DrawWireSphere(zone.transform.parent.position, 0.2f);
            if (zone.transform.childCount == 0)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(zone.transform.position, 0.2f);
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(zone.transform.parent.position, 0.3f);
            }
        }     
    }
}
