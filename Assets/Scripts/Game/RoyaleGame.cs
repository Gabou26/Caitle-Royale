using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Cinemachine.CinemachineTargetGroup;
using static UILifeRoyale;

public class RoyaleGame : GameManager
{
    public Dictionary<GameObject, int> playersLifes;
    public Dictionary<GameObject, bool> playersAlive;

    public static int numberOfLife;
    public static int numberOfCharacters;
    [SerializeField] private int nbLifes = 0;
    [SerializeField] private Transform lifeLabelsParent;
    [SerializeField] private UILifeRoyale royaleModeUILife;

    private int playerAlive;

    //Etages
    [SerializeField] private int etageIdDepart = -1;
    [SerializeField] private EtageManager etageManager;

    protected override void Awake()
    {
        if (numberOfCharacters > 0)
            base.nbCharacters = numberOfCharacters;
        if (numberOfLife <= 0)
            numberOfLife = nbLifes;
        if (etageIdDepart == -1)
            ConfigEtageDepart();
        base.Awake();
        playersLifes = new Dictionary<GameObject, int>();
        playersAlive = new Dictionary<GameObject, bool>();
    }

    void ConfigEtageDepart()
    {
        if (nbCharacters <= 8)
            etageIdDepart = 0;
        else if (nbCharacters < 100)
            etageIdDepart = 1;
        else
            etageIdDepart = 2;
    }

    public override void PrepareMatch()
    {
        if (resultsOpen)
        {
            base.CloseResults();
            return;
        }
        if (!canStartGame)
            return;

        base.PrepareMatch();
    }

    public override void StartMatch()
    {
        if (!canStartGame)
            return;

        if (royaleModeUILife)
            royaleModeUILife.ClearLifeUI();

        etageManager.Initialiser(etageIdDepart);
        base.StartMatch();
        playersLifes.Clear();
        playersAlive.Clear();

        foreach (var player in playersScores)
        {
            if (royaleModeUILife)
                royaleModeUILife.AddLifeUI(player.Key, playersScores[player.Key]);

            playersLifes.Add(player.Key, numberOfLife);
            playersAlive.Add(player.Key, true);
        };

        ResetLife();
        canStartGame = false;
    }

    public override void AddDamageScore(GameObject caster, int damage)
    {
        base.AddDamageScore(caster, damage);
        royaleModeUILife.UpdateStat(caster);
    }

    public bool LooseLife(GameObject player)
    {
        if (player == null)
            return false;
        playersLifes[player]--;
        var hasLife = playersLifes[player] > 0;
        if(!hasLife && playersAlive[player])
        {
            if (royaleModeUILife)
                royaleModeUILife.RemoveLifeUI(player);
            playersAlive[player] = false;
            playerAlive--;
        }
        return hasLife;
    }

    public void AddLife(GameObject player, int nb)
    {
        playersLifes[player] += nb;
    }

    public void CheckGame()
    {
        if (playerAlive <= 1)
        {
            // Game ends
            StopMatch();
        }
        else if (targetGroup.IsEmpty && !IsAnyPlayerAlive())
            StartCoroutine(AjustementCamera());
    }

    public void ReplaceTarget(GameObject killer)
    {
        if (targetGroup.IsEmpty && killer && killer.activeInHierarchy && !IsAnyPlayerAlive())
            targetGroup.AddMember(killer.GetComponent<MousePoint>().mousePoint, 1f, 0f);
    }

    protected override void Spawn(GameObject player)
    {
        player.SetActive(false);

        player.transform.position = GetRandomSpawn(player);

        player.SetActive(true);

        targetGroup.AddMember(player.GetComponent<MousePoint>().mousePoint, 1f, 0f);
    }

    GameObject GetWinner()
    {
        foreach (var player in playersAlive)
        {
            if (player.Value)
                return player.Key;
        }
        return null;
    }

    protected override void StopMatch()
    {
        foreach (Target play in targetGroup.m_Targets) //Vide le targetGroup
            targetGroup.RemoveMember(play.target);

        base.StartCoroutine(ShowScoreStats(GetWinner()));

        isStart = false;

        TMP_Text lblText;
        foreach (Transform lifeLabel in lifeLabelsParent)
        {
            lblText = lifeLabel.GetComponent<TMP_Text>();
            lblText.text = "";
        }
    }

    private void ResetLife()
    {
        foreach (var player in playersScores)
        {
            playersLifes[player.Key] = numberOfLife;
        }

        royaleModeUILife.UpdateUI();

        playerAlive = playersLifes.Count;
    }

    private IEnumerator AjustementCamera()
    {
        yield return new WaitForSeconds(2);
        if (targetGroup.IsEmpty)
        {
            Transform target = royaleModeUILife.statsManager.GetTopPickTrans();
            if (target != null)
                targetGroup.AddMember(target.GetComponent<MousePoint>().mousePoint, 1f, 0f);
        }
    }

    public bool IsAnyPlayerAlive()
    {
        return royaleModeUILife.IsAnyPlayerAlive(this);
    }

    public int GetPlayerCount()
    {
        return playerAlive;
    }

    public void SetEtage(int etageId)
    {
        this.etageId = etageId;
        base.ResetSpawnSpotsList();
    }
}
