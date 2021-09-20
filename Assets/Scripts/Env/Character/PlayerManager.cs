using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public GameObject testPlayer;
    public List<GameObject> players;

    public static PlayerManager instance;

    private List<PlayerInput> playersInput;

    //public RuntimeAnimatorController[] skinAnimators;
    public Sprite[] skinsFrame1, skinsFrame2;
    private int skinId = 0;
    private bool canJoin;
    
    private void Awake()
    {
        if (instance != null)
        {
            if(SceneManager.GetActiveScene().name != "MainMenu")
                instance.GetComponent<PlayerInputManager>().DisableJoining();
            else
                instance.GetComponent<PlayerInputManager>().EnableJoining();
            
            instance.StartCoroutine(instance.CanJoin());
            
            Invoke("FollowPlayer", 0.1f);
            Destroy(gameObject);
            return;
        }
        
        if (skinsFrame1.Length > 0)
            ShuffleSkins();

        instance = this;
        DontDestroyOnLoad(gameObject);

        StartCoroutine(CanJoin());

        playersInput = new List<PlayerInput>();
        if (SceneManager.GetActiveScene().name != "MainMenu")
            GetComponent<PlayerInputManager>().DisableJoining();
        else
            instance.GetComponent<PlayerInputManager>().EnableJoining();
        
        if(players is null)
            players = new List<GameObject>();

        if (SceneManager.GetActiveScene().name != "MainMenu" && players.Count == 0)
        {
            GameObject player = Instantiate(testPlayer);
            players.Add(player.transform.GetChild(0).gameObject);
        }
    }

    private void FollowPlayer()
    {
        foreach (var player in instance.players)
            FindObjectOfType<CinemachineTargetGroup>().AddMember(player.transform, 1f, 0f);
    }

    public IEnumerator CanJoin()
    {
        yield return new WaitForSeconds(0.5f);
        canJoin = true;
    }
    
    private void OnPlayerJoined(PlayerInput playerInput)
    {
        if(SceneManager.GetActiveScene().name != "MainMenu")
            return;

        foreach (var playerInputCheck in playersInput)
            if (playerInputCheck.playerIndex.Equals(playerInput.playerIndex))
                return;

        //Assignation d'un skin
        if (skinsFrame1.Length > 0)
            AssignSkin(playerInput);


        string name = NameGenerator.ObtRandName();
        int playerId = playerInput.playerIndex + 1;

        playerInput.name = name; // + "-PL#" + (playerId).ToString();
        playerInput.transform.GetChild(0).name = name;

        if (playerId > 1)
        {
            playerInput.GetComponentInChildren<PlayerIndicator>().SetPlayer(playerId);
            if (playerId == 2)
                players[players.Count - 1].GetComponentInChildren<PlayerIndicator>().SetPlayer(1);
        }
        
        CharacterController2D chara = playerInput.GetComponentInChildren<CharacterController2D>();
        if (playerInput.currentControlScheme == "Gamepad")
            chara.cursorGamepad.SetActive(true);

        chara.playerLabel.text = playerInput.name;
        FindObjectOfType<CinemachineTargetGroup>().AddMember(chara.transform, 1f, 0f);
        players.Add(chara.gameObject);
        playersInput.Add(playerInput);
    }
    
    private void OnPlayerLeft(PlayerInput playerInput)
    {
        if(SceneManager.GetActiveScene().name != "MainMenu")
            return;
        
        players.Remove(playerInput.gameObject);
    }
    
    public IEnumerator Respawn(GameObject player)
    {
        if (!player.GetComponentInChildren<SpriteRenderer>().GetComponentInChildren<WeaponManager>())
            yield break;
            
        DisablePlayer(player);
    
        yield return new WaitForSeconds(3f);

        Spawn(player);
        HealSystem heal = player.GetComponent<HealSystem>();
        heal.isInvincible = true;
        yield return new WaitForSeconds(0.6f);
        heal.isInvincible = false;
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

    void AssignSkin(PlayerInput playerInput)
    {
        skinId = (skinId + 1) % skinsFrame1.Length;
        Animateur anim = playerInput.GetComponentInChildren<Animateur>();
        Sprite[] frames = new Sprite[2];
        frames[0] = skinsFrame1[skinId];
        frames[1] = skinsFrame2[skinId];
        anim.SetAnim(frames);
    }

    public void DisablePlayer(GameObject player)
    {
        player.SetActive(false);
        FindObjectOfType<CinemachineTargetGroup>().RemoveMember(player.transform);
        player.GetComponent<HealSystem>().Recover();
        player.GetComponentInChildren<SpriteRenderer>().GetComponentInChildren<WeaponManager>().TakeDefaultWeapon();
        if (GetComponent<AudioSource>())
            GetComponent<AudioSource>().Play();
    }
    
    public void Spawn(GameObject player)
    {
        player.SetActive(true);
        player.transform.position = Vector3.zero;
        FindObjectOfType<CinemachineTargetGroup>().AddMember(player.transform, 1f, 0f);
    }
}
