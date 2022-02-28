using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealSystemAI : HealSystem
{
    //Champs
    AIManager aiManager;


    private void Awake()
    {
        aiManager = GameObject.Find("AIManager").GetComponent<AIManager>();
    }

    new void Start()
    {
        base.Start();
    }

    public override void Death(GameObject killer)
    {
        if (killer.GetInstanceID().Equals(gameObject.GetInstanceID()))
        {
            //suicide
            GameManager.instance.AddSuicideScore(gameObject);
        }
        else
        {
            //kill
            if (killer != null)
            {
                if (killer.activeInHierarchy)
                {
                    HealSystem healKiller = killer.GetComponent<HealSystem>();
                    healKiller.RecoverHalf();
                    healKiller.UpgradeHealth();
                }
                if (killer.layer == 8)
                    killer.GetComponent<AudioSource>().Play();

                GameManager.instance.AddKillScore(killer);
            }
            GameManager.instance.AddDeathScore(gameObject);
        }

        var lifeGame = GameManager.instance as LifeGame;
        var royaleGame = GameManager.instance as RoyaleGame;
        if (lifeGame && lifeGame.isPlaying)
        {
            if (lifeGame.LooseLife(gameObject))
                aiManager.OnAiDeath(transform, killer);
            else
            {
                lifeGame.CheckGame();
                Destroy(gameObject);
            }
        }
        else if (royaleGame && royaleGame.isPlaying)
        {
            if (royaleGame.LooseLife(gameObject))
                aiManager.OnAiDeath(transform, killer);
            else
            {

                royaleGame.DisablePlayer(gameObject);
                royaleGame.ReplaceTarget(killer);
                royaleGame.CheckGame();
                Destroy(gameObject);
            }
        }
        else
            aiManager.OnAiDeath(transform, killer);
    }
}
