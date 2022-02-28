using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using static Cinemachine.CinemachineTargetGroup;

public class GameManager : MonoBehaviour
{
    [SerializeField] protected bool freezePlayers = false;
    protected List<GameObject> players;
    
    public static GameManager instance;
    public Dictionary<GameObject, Score> playersScores;

    [HideInInspector] public bool isPlaying;

    [SerializeField] private AiUpdater aiUpdater;
    [SerializeField] private DepartCountdown departUI;
    [SerializeField] private GameObject startGameLabel;
    [SerializeField] private GameObject resultsGameLabel;

    //Spawns
    [SerializeField] protected Vector2[] spawnSpots;
    protected SpawnManager spawnManager;

    [SerializeField] protected CinemachineTargetGroup targetGroup;
    [SerializeField] public int nbCharacters = 4;
    [SerializeField] protected int nbLifes = 1;

    //Royale Spawn Spot
    [SerializeField] private Transform spawnSpotsParent;
    protected int etageId;
    private List<Transform> spawnSpotsList;

    //[SerializeField] private TMP_Text[] resultsPlayerText;
    [SerializeField] private UIEndResult[] endResults;

    protected bool resultsOpen = false;
    protected bool canStartGame = true;
    protected int nextSpawn;
    protected List<GameObject> AIs;

    [SerializeField] protected TransitionUI transUI, transStartUI1, transStartUI2;

    public Transform objTemp;

    //Bull Updater : Sert à instancier X bullets en début partie
    [SerializeField] BulletUpdater bullUpdater;

    protected virtual void Awake()
    {
        Physics.reuseCollisionCallbacks = true; //Optimisation Physics

        if (instance != null && instance.gameObject.scene.IsValid())
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        
        players = new List<GameObject>();
        playersScores = new Dictionary<GameObject, Score>();
    }

    protected virtual void Start()
    {
        players = PlayerManager.instance.players;
        spawnManager = new SpawnManager();

        foreach (GameObject play in players)
            play.SetActive(false);
        foreach (GameObject play in players)
        {
            Spawn(play);
            play.SetActive(true);
        }
    }

    public virtual void PrepareMatch()
    {
        ResetSpawnSpotsList();
        StartCoroutine(MatchCountDown());
    }

    IEnumerator MatchCountDown()
    {
        transStartUI1.Transition(new Vector2(-2000, 0), new Vector2(0, 0), false);
        transStartUI2.Transition(new Vector2(2000, 0), new Vector2(0, 0), false);
        yield return new WaitForSeconds(0.45f);
        StartMatch();
        foreach (GameObject player in PlayerManager.instance.players)
        {
            player.GetComponentInChildren<HealSystem>().ResetHealth();
            player.GetComponentInChildren<WeaponManager>().ResetDamage();
            player.GetComponent<CharacterController2D>().enabled = false;
        }
            
        aiUpdater.enabled = false;
        yield return new WaitForEndOfFrame();

        bullUpdater.GenerateBullets(nbCharacters, 5);

        yield return new WaitForSeconds(0.05f);
        foreach (GameObject player in PlayerManager.instance.players)
            player.GetComponentInChildren<Weapon>().enabled = false;
        transStartUI1.Transition(new Vector2(0, 0), new Vector2(2000, 0), true);
        transStartUI2.Transition(new Vector2(0, 0), new Vector2(-2000, 0), true);
        departUI.Activer(2);
        yield return new WaitForSeconds(2);
        //Activer
        aiUpdater.enabled = true;
        if (!freezePlayers)
        {
            foreach (GameObject ai in AIs)
            {
                ai.GetComponent<PromenadeAI>().enabled = true;
                ai.GetComponentInChildren<AIWeaponManager>().enabled = true;
                ai.GetComponentInChildren<Weapon>().enabled = true;
            }
        }
        foreach (GameObject player in PlayerManager.instance.players)
        {
            player.GetComponent<CharacterController2D>().enabled = true;
            player.GetComponentInChildren<Weapon>().enabled = true;
        }
    }

    public virtual void StartMatch()
    {
        if(!canStartGame)
            return;

        playersScores.Clear();
        NameGenerator.ResetNamesList(true);

        foreach (var player in players)
            playersScores.Add(player, new Score(player.transform.parent.name, true));

        var AI = FindObjectOfType<AIManager>();
        string playerName;
        if (AI)
        {
            AI.nbSpawns = nbCharacters - players.Count;
            AIs = AI.SpawnAllAI();
            for (int i = 0; i < AIs.Count; i++)
            {
                playerName = NameGenerator.ObtRandName(); // + "-AI#" + (i+1).ToString();
                instance.playersScores.Add(AIs[i], new Score(playerName));
                AIs[i].GetComponent<PromenadeAI>().label.text = playerName;
            }
        }

        startGameLabel.SetActive(false);
        canStartGame = false;
        isPlaying = true;
        nextSpawn = 0;
        RespawnAll(true);
    }
    
    protected virtual void StopMatch()
    {
        isPlaying = false;
        StartCoroutine(ShowScore());
    }

    public void AddKillScore(GameObject caster)
    {
        if (!isPlaying)
            return;

        playersScores[caster].killScore++;
    }


    public virtual void AddDamageScore(GameObject caster, int damage)
    {
        if (!isPlaying)
            return;

        if (playersScores.ContainsKey(caster))
        {
            playersScores[caster].damageScore += damage;
            playersScores[caster].damagehitsScore++;
        }
    }

    public void AddShotData(GameObject caster)
    {
        if (!isPlaying)
            return;

        if (playersScores.ContainsKey(caster))
            playersScores[caster].totalShots++;
    }

    public void AddDmgTakenScore(GameObject receiver, int damage)
    {
        ;
        if (!isPlaying)
            return;

        if (playersScores.ContainsKey(receiver))
            playersScores[receiver].takenhitsScore++;
    }

    public void AddDeathScore(GameObject caster)
    {
        if(!isPlaying)
            return;
        
        if (playersScores.ContainsKey(caster))
            playersScores[caster].deathScore++;
    }

    public void AddSuicideScore(GameObject caster)
    {
        if(!isPlaying)
            return;
        
        playersScores[caster].suicideScore++;
    }
    
    public void DisablePlayer(GameObject player)
    {
        player.SetActive(false);

        aiUpdater.RetraitAI(player.transform);
        targetGroup.RemoveMember(player.transform);
        //Visée Curseur
        MousePoint mousePoint = player.GetComponent<MousePoint>();
        if (mousePoint)
            targetGroup.RemoveMember(mousePoint.mousePoint);

        player.GetComponent<HealSystem>().Recover();
        player.GetComponentInChildren<SpriteRenderer>().GetComponentInChildren<WeaponManager>().TakeDefaultWeapon();
    }

    protected virtual void Spawn(GameObject player)
    {
        if (spawnSpotsParent != null)
            player.transform.position = GetRandomSpawn(player);
        else
        {
            player.transform.position = spawnSpots[nextSpawn];
            nextSpawn++;
            if (nextSpawn >= spawnSpots.Length)
                nextSpawn = 0;
        }

        targetGroup.AddMember(player.transform, 1f, 0f);
    }
    
    public IEnumerator Respawn(GameObject player)
    {
        if (!player.GetComponentInChildren<SpriteRenderer>().GetComponentInChildren<WeaponManager>())
            yield break;

        DisablePlayer(player);

        yield return new WaitForSeconds(0.5f);

        WaitTimer wait = new WaitTimer();
        wait.SetWaitTIme(2.5f);
        yield return new WaitUntil(wait.WaitIfPlaying);

        player.SetActive(true);

        Spawn(player);
    }

    public void RespawnAll(bool forcePos)
    {
        if (forcePos)
            foreach (var player in PlayerManager.instance.players)
                player.SetActive(false);
        foreach (var player in PlayerManager.instance.players)
        {
            if (!player.activeSelf && !forcePos) //&& !(GameManager.instance is RoyaleGame)
            {
                if (spawnSpotsParent != null)
                    player.transform.position = GetRandomSpawn(player);
                else
                {
                    player.transform.position = spawnSpots[nextSpawn];
                    nextSpawn++;
                    if (nextSpawn >= spawnSpots.Length)
                        nextSpawn = 0;
                }

                var royaleGame = GameManager.instance as RoyaleGame;
                if (royaleGame)
                    targetGroup.AddMember(player.GetComponentInChildren<MousePoint>().mousePoint, 1f, 0f);
                else
                    targetGroup.AddMember(player.transform, 1f, 0f);
                player.SetActive(true);

                player.GetComponentInChildren<WeaponManager>().TakeDefaultWeapon();
                player.GetComponent<CharacterController2D>().direction = Vector2.zero;
            }
            else if (forcePos)
            {
                if (spawnSpotsParent != null)
                    player.transform.position = GetRandomSpawn(player);
                else
                {
                    player.transform.position = spawnSpots[nextSpawn];
                    nextSpawn++;
                    if (nextSpawn >= spawnSpots.Length)
                        nextSpawn = 0;
                }
                player.SetActive(true);
                player.GetComponentInChildren<SpriteRenderer>().GetComponentInChildren<WeaponManager>().TakeDefaultWeapon();
            }
            
            player.GetComponent<HealSystem>().Recover();
        }
    }

    Score BestPlayer()
    {
        Score topScore = new Score("None");
        foreach (var playerScore in playersScores)
        {
            topScore = playerScore.Value;
            break;
        }

        foreach (var playerScore in playersScores)
            if (playerScore.Value.damageScore > topScore.damageScore)
                topScore = playerScore.Value;

        return topScore;
    }

    Score WorstPlayer()
    {
        Score topScore = new Score("None");
        foreach (var playerScore in playersScores)
        {
            topScore = playerScore.Value;
            break;
        }

        foreach (var playerScore in playersScores)
            if (playerScore.Value.damageScore < topScore.damageScore)
                topScore = playerScore.Value;

        return topScore;
    }
    string RandomPlayer()
    {
        List<string> playerList = new List<string>();
        foreach (var playerScore in playersScores)
            playerList.Add(playerScore.Value.name);

        string randName = playerList[Random.Range(0, playerList.Count)];
        return randName + " :)";
    }

    private IEnumerator ShowScore()
    {
        transUI.Transition(new Vector2(-2000, 0), new Vector2(0, 0), false);
        yield return new WaitForSeconds(0.5f);
        BulletUpdater.instance.EmptyPooling();

        foreach (GameObject AI in AIs)
        {
            if (!AI)
                continue;
            aiUpdater.RetraitAI(AI.transform);
            Destroy(AI);
        }
        AIs = new List<GameObject>();

        RespawnAll(false);

        resultsGameLabel.SetActive(true);

        transUI.Transition(new Vector2(0, 0), new Vector2(2000, 0), true);
        yield return new WaitForSeconds(7.5f);

        resultsGameLabel.SetActive(false);
        canStartGame = true;
        startGameLabel.SetActive(true);
    }

    protected IEnumerator ShowScoreStats(GameObject winner)
    {
        transUI.Transition(new Vector2(-2000, 0), new Vector2(0, 0), false);
        yield return new WaitForSeconds(0.5f);
        BulletUpdater.instance.EmptyPooling();

        foreach (GameObject AI in AIs)
        {
            if (!AI)
                continue;
            aiUpdater.RetraitAI(AI.transform);
            Destroy(AI);
        }
        AIs = new List<GameObject>();

        resultsGameLabel.SetActive(true);
        yield return new WaitForEndOfFrame();

        //Score[] results = new Score[4];
        //results[0] = playersScores[winner];
        //results[1] = MostKills();
        //results[2] = BestPlayer();
        //results[3] = WorstPlayer();
        //for (int i = 0; i < results.Length; i++) //Attribution Couleurs
        //    if (results[i].isPlayer)
        //        resultsPlayerText[i].color = new Color(0.5f, 1, 0.5f, 1);
        //    else
        //        resultsPlayerText[i].color = new Color(1, 1, 1, 1);

        //New UI End
        int numberRes = UIEndResult.playerCount;
        if (nbCharacters < numberRes)
            numberRes = nbCharacters;

        endResults[0].ShowDMG(StatsCalculator.SortRankings(playersScores, numberRes, nbLifes, StatsCalculator.StatType.MOSTDMG));
        endResults[1].ShowKills(StatsCalculator.SortRankings(playersScores, numberRes, nbLifes, StatsCalculator.StatType.MOSTKILLS));
        endResults[2].ShowRanking(StatsCalculator.SortRankings(playersScores, numberRes, nbLifes, StatsCalculator.StatType.RANKING));
        endResults[3].ShowGlobal(StatsCalculator.SortRankings(playersScores, numberRes, nbLifes, StatsCalculator.StatType.GLOBAL));
        //resultsPlayerText[0].text = "Winner: " + ObtStatResult(results[0]);
        //resultsPlayerText[1].text = "Most kills: " + ObtStatResult(results[1]);
        //resultsPlayerText[2].text = "Most DMG: " + ObtStatResult(results[2]);
        //resultsPlayerText[3].text = "Least DMG: " + ObtStatResult(results[3]);

        transUI.Transition(new Vector2(0, 0), new Vector2(2000, 0), true);
        resultsOpen = true;

        foreach (var player in PlayerManager.instance.players)
            player.SetActive(false);

        RespawnAll(false);
    }

    string ObtStatResult(Score result)
    {
        return result.name + "\n" + result.killScore + " Kills | " + result.damageScore + "DMG | #" + result.rankingScore;
    }

    protected void CloseResults()
    {
        resultsOpen = false;
        resultsGameLabel.SetActive(false);
        canStartGame = true;
        startGameLabel.SetActive(true);
    }

    bool JoueurVie()
    {
        foreach (GameObject player in PlayerManager.instance.players)
        {
            if (player.activeInHierarchy)
                return true;
        }
        return false;
    }

    public Vector2 GetRandomSpawn(GameObject player)
    {
        //Si joueur Spawn dans zone
        if (player.layer == 8 && JoueurVie())
            return ObtSpawnProche(ObtMoyTGroup());

        if (spawnSpotsList == null || spawnSpotsList.Count <= 0)
            ResetSpawnSpotsList();

        Vector2 spawn;
        if (spawnSpotsList.Count > 0)
            spawn = spawnSpotsList[0].position;
        else
            spawn = player.transform.position;
        spawnSpotsList.RemoveAt(0);

        return spawn;
    }

    public Vector2 ObtMoyTGroup()
    {
        return targetGroup.transform.position;
        Vector2 pos = Vector2.zero;
        int count = 0;
        foreach (GameObject play in players)
        {
            if (play.activeInHierarchy)
            {
                if (count <= 0)
                    pos = play.transform.position;
                else
                    pos += (Vector2)play.transform.position;
                count++;
            }
        }      
        //objTemp.position = pos / count;
        return pos / count;
    }

    Vector2 ObtSpawnProche(Vector2 posBase)
    {
        Transform spawnEtage = spawnSpotsParent.GetChild(etageId);
        Vector2 bestPos = spawnEtage.GetChild(0).position;
        float bestDist = Vector2.Distance(posBase, bestPos);
        float dist;
        Vector2 pos;
        for (int i = 1; i < spawnEtage.childCount; i++)
        {
            pos = spawnEtage.GetChild(i).position;
            dist = Vector2.Distance(posBase, pos);
            if (dist < bestDist)
            {
                bestDist = dist;
                bestPos = pos;
            }
        }
        return bestPos;
    }

    public void ResetSpawnSpotsList()
    {
        Transform spawnEtage = spawnSpotsParent.GetChild(etageId);
        spawnSpotsList = spawnManager.ObtRandSpawns(nbCharacters, spawnEtage);
        /*for (int i = 0; i < spawnEtage.childCount; i++)
            spawnSpotsList.Add(spawnEtage.GetChild(i));*/
    }
}