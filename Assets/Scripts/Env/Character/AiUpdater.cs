using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiUpdater : MonoBehaviour
{
    public class AIStr
    {
        public Transform trans;
        public Canvas canvVies;
        public SpriteRenderer sprite;
        public AIWeaponManager arme;
        public AIStr prev, next;
    }


    //Refs
    public static AiUpdater instance;
    [SerializeField] private Transform targetUI;

    //Champs
    List<AIStr> ais = new List<AIStr>();
    public List<GameObject> players = new List<GameObject>();

    //Intervalles (const et dyn)
    const float interMouv = 0.6f, interZone = 1.5f;
    const float interArme = 2f, interUI = 0.15f;
    float interMouvDyn, interZoneDyn;
    float interArmeDyn, interUIDyn;

    AIStr promCour, armeCour, uiVieCour;
    float delaiCourMouv, delaiCourZone;
    float delaiCourUI, delaiCourArme;

    //Cam Pos
    AIStr aiTemp;
    Camera cam;
    Vector3 camVP;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        instance = this;
    }

    private void Start()
    {
        cam = Camera.main;

        foreach (var player in PlayerManager.instance.players)
            players.Add(player);
    }

    void Update()
    {
        if (ais.Count <= 0)
            return;
        VerifArmes();
        VerifUI();
    }

    public void AjoutAI(Transform ai)
    {
        AIStr aiStr = new AIStr();
        
        aiStr.arme = ai.GetComponentInChildren<AIWeaponManager>();
        aiStr.trans = ai;
        aiStr.canvVies = ai.GetChild(0).GetComponent<Canvas>();
        ais.Add(aiStr);
        players.Add(ai.gameObject);

        //Assigner Prev/next
        if (ais.Count > 1)
        {
            aiStr.prev = ais[ais.Count - 2];
            aiStr.prev.next = aiStr;
        }
        else
        {
            aiStr.prev = aiStr;
            promCour = aiStr;
            armeCour = aiStr;
            uiVieCour = aiStr;
        }
        aiStr.next = ais[0];
        aiStr.next.prev = aiStr;

        /*if (targetUI)
            ais[ais.Count-1].uiVie.SetTarget(targetUI);*/

        ResetIntervalles();
    }

    public void RetraitAI(Transform ai)
    {
        for (int i = 0; i < ais.Count; i++)
        {
            if (ais[i].trans == ai)
            {
                aiTemp = ais[i];
                //Assigner Prev/next
                if (ais.Count > 1)
                {
                    aiTemp.prev.next = aiTemp.next;
                    aiTemp.next.prev = aiTemp.prev;
                }
                ResetVerifCour(aiTemp);

                ais.RemoveAt(i);
                players.Remove(ai.gameObject);
                ResetIntervalles();
                return;
            }
        }
    }

    void ResetVerifCour(AIStr aiStr)
    {
        if (armeCour == aiStr)
        {
            delaiCourArme -= interArmeDyn;
            armeCour = armeCour.next;
        }
        if (uiVieCour == aiStr)
        {
            delaiCourUI -= interUIDyn;
            uiVieCour = uiVieCour.next;
        }
    }


    void ResetIntervalles()
    {
        int count = ais.Count;
        interMouvDyn = interMouv / (float)count;
        interZoneDyn = interZone / (float)count;
        interArmeDyn = interArme / (float)count;
        interUIDyn = interUI / count;
    }

    void VerifProm()
    {
       /* while (delaiCourMouv > interMouvDyn)
        {
            //promenades[idMouvCour].

            idMouvCour = idMouvCour < promenades.Count ? idMouvCour++ : 0;
            delaiCourMouv -= interMouvDyn;
        }*/
    }

    void VerifArmes()
    {
        delaiCourArme += Time.deltaTime;
        while (delaiCourArme >= interArmeDyn)
        {
            armeCour.arme.UpdateTarget();
            armeCour = armeCour.next;
            delaiCourArme -= interArmeDyn;
        }
    }

    void VerifUI()
    {
        if (!targetUI)
            return;

        delaiCourUI += Time.deltaTime;
        while (delaiCourUI >= interUIDyn)
        {
            camVP = cam.WorldToViewportPoint(uiVieCour.trans.position);
            if (camVP.x > -0.2f && camVP.x <= 1.2f && camVP.y > -0.2f && camVP.y <= 1.2f)
                uiVieCour.canvVies.enabled = true;
            else
                uiVieCour.canvVies.enabled = false;

            uiVieCour = uiVieCour.next;
            delaiCourUI -= interUIDyn;
        }
    }
}
