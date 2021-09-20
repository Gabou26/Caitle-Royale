using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AIWeaponManager : WeaponManager
{
    //Champs
    GameObject character;
    Transform playerList;
    [HideInInspector] public Transform target;

    bool modeTir = true;
    const float interSearch = 2f;

    [SerializeField] private SpriteRenderer spriteR;

    //Tir System
    float delaiShoot = 1;
    float interShoot = 0.9f;
    float angleSin;

    [SerializeField] private LayerMask MursRecherche;

    // Start is called before the first frame update
    void Awake()
    {
        character = transform.parent.parent.gameObject;
        playerList = GameObject.Find("PlayerList").transform;
        TakeDefaultWeapon();
    }

    private void Start()
    {
        UpdateTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (modeTir)
        {
            if (target != null && target.gameObject.activeInHierarchy)
                directionAim = (target.position - transform.position).normalized;
            else if (target == null || !target.gameObject.activeInHierarchy)
            {
                currentWeapon.GetComponent<Weapon>().isFiring = false;
                modeTir = false;
                directionAim = Vector2.right;
            }

            angleDirAim = Vector2.Angle(Vector2.right, directionAim);
            if (Vector3.Cross(Vector2.right, directionAim).z <= 0f)
                angleDirAim = 360f - angleDirAim;
            //Ajuster Angle Sprite
            spriteR.flipX = angleDirAim > 270 || angleDirAim < 90 ? false : true;
        }

        //Angle Sinuosidale
        angleSin = Mathf.Sin(Time.time * 16) * 8;
        transform.rotation = Quaternion.Euler((angleDirAim + angleSin) * Vector3.forward);
    }

    public void UpdateTarget()
    {
        FindNearestTarget();
        currentWeapon.GetComponent<Weapon>().isFiring = true;
        modeTir = true;
    }

    void FindNearestTarget()
    {
        //Obtention de la cible la plus proche
        target = null;
        float distMin = 10000;
        float dist;
        float distCibles;

        foreach (GameObject player in AiUpdater.instance.players) //Verif avec limite distance
        {
            if (player && player != character && player.activeInHierarchy)
            {
                distCibles = Vector2.Distance(player.transform.position, transform.position);
                if (distCibles < 20 && player != character)
                {
                    dist = CalculateDistValue(player, distCibles);
                    if (dist < distMin)
                    {
                        distMin = dist;
                        target = player.transform;
                    }
                }
            }
        }
        if (target == null) //Verif sans limite distance
        {
            foreach (GameObject player in AiUpdater.instance.players)
            {
                if (player && player != character && player.activeInHierarchy)
                {
                    distCibles = Vector2.Distance(player.transform.position, transform.position);
                    dist = CalculateDistValue(player, distCibles);
                    if (dist < distMin)
                    {
                        distMin = dist;
                        target = player.transform;
                    }
                }
            }
        }
        delaiShoot = interShoot;
    }

    float CalculateDistValue(GameObject target2, float facDist)
    {
        facDist += Random.Range(0, 3);

        float health = target2.GetComponent<HealSystem>().GetHealthRatio();
        if (facDist < 3)
            facDist += 4;
        else if (facDist < 6)
            facDist += 3;
        else if (facDist < 9)
            facDist += 2;
        else if (facDist < 12)
            facDist += 1;

        //Si Joueur
        if (target2.layer == 8) {
            if (health <= 0.25f)
                return facDist - 9f;
            else if (health <= 0.5f)
                return facDist - 3.5f;
            else
                return facDist;
        }
        else
        {
            if (health <= 0.25f)
                return facDist - 6f;
            else if (health <= 0.5f)
                return facDist - 1f;
            else
                return facDist + 4f;
        }

        return facDist + 10;
    }
}
