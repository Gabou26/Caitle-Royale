using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AIManager : MonoBehaviour
{
    [SerializeField] private GameObject aiPrefab;
    [SerializeField] private CinemachineTargetGroup targetGroup;
    private AiUpdater aiUpdater;
    private Transform playerList;
    private GameManager gameManager;

    public int nbSpawns = 2;
    public Sprite[] skinsFrame1, skinsFrame2;
    private int skinId = -1;

    private int etageId = 1;

    private void Start()
    {
        playerList = GameObject.Find("PlayerList").transform;
        gameManager = GameManager.instance;
        aiUpdater = GetComponent<AiUpdater>();
        ShuffleSkins();
    }

    public List<GameObject> SpawnAllAI()
    {
        var AIs = new List<GameObject>();
        float delaiCour;
        GameObject AI;
        for (int i = 0; i < nbSpawns; i++)
        {
            delaiCour = ((float)i / (float)nbSpawns);
            AI = SpawnAI();
            AIs.Add(AI);
            AI.GetComponentInChildren<Weapon>().SetDelay(delaiCour);
            AI.GetComponent<PromenadeAI>().SetMouvDelai(delaiCour);
            AI.GetComponentInChildren<Animateur>().SetDelai(delaiCour);
        }

        return AIs;
    }
    
    private GameObject SpawnAI()
    {
        var ai = Instantiate(aiPrefab);
        OnAIAdd(ai.transform);

        return ai;
    }

    void ShuffleSkins()
    {
        // skinId = Random.Range(0, skinAnimators.Length);
        // return;

        List<int> skinsIds = new List<int>();
        for (int i = 0; i < skinsFrame1.Length; i++)
            skinsIds.Add(i);

        int[] skinsIdRand = new int[skinsIds.Count];
        for (int i = 0; i < skinsFrame1.Length; i++)
        {
            int id = Random.Range(0, 1000000) % skinsIds.Count;
            skinsIdRand[i] = skinsIds[id];
            skinsIds.RemoveAt(id);
        }

        Sprite[] frames1 = new Sprite[skinsIdRand.Length];
        Sprite[] frames2 = new Sprite[skinsIdRand.Length];
        for (int i = 0; i < skinsIdRand.Length; i++)
        {
            frames1[i] = skinsFrame1[skinsIdRand[i]];
            frames2[i] = skinsFrame2[skinsIdRand[i]];
        }

        skinsFrame1 = frames1;
        skinsFrame2 = frames2;
    }


    void AssignSkin(Transform ai)
    {
        skinId = (skinId + 1) % skinsFrame2.Length;
        Animateur anim = ai.GetComponentInChildren<Animateur>();
        Sprite[] frames = new Sprite[2];
        frames[0] = skinsFrame1[skinId];
        frames[1] = skinsFrame2[skinId];
        anim.SetAnim(frames);
    }


    public void OnAIAdd(Transform ai)
    {
        if (targetGroup == null)
            targetGroup = GameObject.Find("TargetGroup").GetComponent<CinemachineTargetGroup>();

        //Ne pas mettre dans liste target lorsque royale. Map trop grande.
        if (!(gameManager is RoyaleGame))
            targetGroup.AddMember(ai, 1f, 0f);

        if (playerList != null)
            ai.parent = playerList;

        //Assignation d'un skin
        if (skinsFrame1.Length > 0)
            AssignSkin(ai);

        aiUpdater.AjoutAI(ai);
        PlaceAI(ai);
    }

    public void OnAiDeath(Transform ai, GameObject killer)
    {
        StartCoroutine(DeathAIWait(ai));
        if (killer && killer.layer == 8)
            GetComponent<AudioSource>().Play();
    }

    void PlaceAI(Transform ai)
    {
        ai.position = gameManager.GetRandomSpawn(ai.gameObject); ;
    }

    IEnumerator DeathAIWait(Transform ai)
    {
        ai.gameObject.SetActive(false);
        aiUpdater.RetraitAI(ai);

        yield return new WaitForSeconds(3);

        HealSystem heal = ai.GetComponent<HealSystem>();
        if (ai != null)
        {
            heal.Recover();
            aiUpdater.AjoutAI(ai);
            PlaceAI(ai);
            ai.gameObject.SetActive(true);
            heal.isInvincible = true;
            yield return new WaitForSeconds(0.6f);
            heal.isInvincible = false;
        }
    }
}
