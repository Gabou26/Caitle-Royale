using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PromenadeAI : MonoBehaviour
{
    public Text label;
    [SerializeField] private Animateur anim;
    //Champs
    Transform map;
    Transform parentZones;
    public float speed = 8f;
    public float interMouv = 0.6f;
    const float interZone = 1.4f;

    //Vars
    Transform zoneCour;
    float intMouvCour = 0;
    List<Vector2> positionsZone;
    Vector2 posCible;
    float intCourZone = 0;

    //Pathfinding
    Vector2 ciblePath;
    bool modePath = false;
    AIWeaponManager aiWeapon;

    private float maxSpeed;

    //Temp
    Vector2 dir;
    float dist;
    Vector2 posCour;

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(speed * 0.9f, speed * 1.1f);

        maxSpeed = speed;
        map = GameObject.Find("map_test").transform;

        parentZones = GameObject.Find("Zones Mouv").transform;

        aiWeapon = GetComponentInChildren<AIWeaponManager>();

        PlacerSpawn();
    }

    public void SetMouvDelai(float delai)
    {
        this.intCourZone = delai * interZone;
    }

    public void OnEnable()
    {
        //Empêche l'appel à la création, ce qui fait un erreur
        if (modePath)
        {
            intCourZone = 0;
            PlacerSpawn();
        }
    }

    void PlacerSpawn()
    {
        posCour = transform.position;
        ObtZoneDepart();
        //Pathfinding
        modePath = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!modePath)
        {
            intCourZone += Time.deltaTime;
            if (intCourZone >= interZone)
            {
                intCourZone -= interZone;
                ChangerZone();
            }

            Bouger();
        }
        else
            BougerPath();
    }

    void BougerPath()
    {
        dist = Vector2.Distance(posCour, ciblePath);
        if (dist < 0.4f)
            modePath = false;

        dir = (ciblePath - posCour).normalized;
        posCour += dir * Time.deltaTime * speed * 1.15f;
        transform.position = posCour;
        anim.MouvAnim();
        //transform.Translate(dir * Time.deltaTime * speed * 1.15f);
    }

    void MonterChemin(Transform target)
    {
        ZoneManager zoneManager = parentZones.GetComponent<ZoneManager>();
        Transform zone = zoneManager.ObtProchZone(zoneCour, transform, target);
        bool dansEtage = zoneManager.DansEtage(zone);
        if (zone != null)
            zoneCour = zone;

        //Si pas dans même zone, coupe le délai d'attente de mouvement par 3
        float dist = Vector2.Distance(zoneCour.position, target.position);
        if (dist > 40 || !dansEtage)
            intCourZone += interZone * 20f * Time.deltaTime;
        else if (dist > 30)
            intCourZone += interZone * 10f * Time.deltaTime;
        else if (dist > 15)
            intCourZone += interZone * 4f * Time.deltaTime;
        //if (RoyaleGame.instance.) //à ajouter : SI joueur et fin partie, deviens très agressif car sinon trop facile et joueur fait du distancing.

        modePath = true;
        ciblePath = zoneCour.position;
    }

    void Bouger()
    {
        dist = Vector2.Distance(posCible, posCour);
        dir = (posCible - posCour).normalized;

        if (dist > 0.2f)
        {
            posCour += dir * Time.deltaTime * speed;
            transform.position = posCour;
            anim.MouvAnim();
        }
        else
        {
            intMouvCour += Time.deltaTime;
            if (intMouvCour >= interMouv)
            {
                ObtenirPosZone();
                intMouvCour = 0;
            }
        }
    }

    

    void ObtZoneDepart()
    {
        List<Transform> zonesEtage = parentZones.GetComponent<ZoneManager>().ObtEtageCour();
        //Obtention de la zone de départ
        zoneCour = zonesEtage[0];

        float distMin = Vector2.Distance(posCour, zonesEtage[0].position);
        for (int i = 1; i < zonesEtage.Count; i++)
        {
            float dist = Vector2.Distance(posCour, zonesEtage[i].position);
            if (dist < distMin)
            {
                distMin = dist;
                zoneCour = zonesEtage[i];
            }
        }

        ResetListPos();
        ObtenirPosZone();
    }

    void ChangerZone()
    {
        //Obtention zone
        Transform target = aiWeapon.target;
        if (target == null)
            target = transform;

        MonterChemin(target);

        ResetListPos();
        ObtenirPosZone();
    }

    void ObtenirPosZone()
    {
        if (positionsZone == null || positionsZone.Count == 0)
            ResetListPos();
        int idRand = Random.Range(0, positionsZone.Count);

        //Empeche obtention de la même position
        if (positionsZone[idRand] == posCible && positionsZone.Count > 1)
        {
            positionsZone.RemoveAt(idRand);
            idRand = Random.Range(0, positionsZone.Count);
        }
        posCible = positionsZone[idRand];
        positionsZone.RemoveAt(idRand);
    }

    void ResetListPos()
    {
        positionsZone = new List<Vector2>();
        foreach (Transform posZone in zoneCour)
            positionsZone.Add(posZone.position);
    }
    
    private void OnDisable()
    {
        StopAllCoroutines();
        speed = maxSpeed;
        GetComponent<HealSystem>().isInvincible = false;
    }

    IEnumerator Promenade()
    {
        Transform zoneBase = zoneCour;
        do
        {
            ObtenirPosZone();
            yield return new WaitForSeconds(interMouv);
        } while (zoneCour == zoneBase);
    }

    public IEnumerator Slow(float slowRate, float duration)
    {
        speed = maxSpeed * slowRate;
        SpriteRenderer spriteR = GetComponentInChildren<SpriteRenderer>();
        spriteR.color = new Color(0.3f, 0.6f, 1, 1);
        yield return new WaitForSeconds(duration);
        ResetStatus();
    }

    public void ResetStatus()
    {
        SpriteRenderer spriteR = GetComponentInChildren<SpriteRenderer>();
        spriteR.color = new Color(1, 1, 1, 1);
        speed = maxSpeed;
    }
}
