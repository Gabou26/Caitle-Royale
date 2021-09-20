using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEtage : MonoBehaviour
{
    List<Transform>[] healedPlayers;
    RoyaleGame royaleGame;
    LifeGame lifeGame;

    private void Start()
    {
        lifeGame = GameManager.instance as LifeGame;
        royaleGame = GameManager.instance as RoyaleGame;
        ResetHealedPlayers();
    }

    public void SetActiveEtage(int etage, bool active)
    {
        transform.GetChild(etage).gameObject.SetActive(active);
        if (active)
            healedPlayers[etage] = new List<Transform>();
    }

    public void HealPlayer(Transform player, Transform parentHeal)
    {
        int etage = ObtEtageId(parentHeal);
        if (!HasBeenHealed(player, etage))
        {
            healedPlayers[etage].Add(player);

            if (etage == 1)
            {
                if (royaleGame)
                    royaleGame.AddLife(player.gameObject, 1);
                else if (lifeGame)
                    lifeGame.AddLife(player.gameObject, 1);
            }
                
            player.GetComponent<HealSystem>().Recover(true);
        }
    }

    void ResetHealedPlayers()
    {
        healedPlayers = new List<Transform>[3];
        for (int i = 0; i < healedPlayers.Length; i++)
        {
            healedPlayers[i] = new List<Transform>();
        }
    }

    bool HasBeenHealed(Transform player, int idEtage)
    {
        bool hasBeenHealed = false;
        foreach (Transform healed in healedPlayers[idEtage])
        {
            if (healed == player)
            {
                hasBeenHealed = true;
                break;
            }
        }
        return hasBeenHealed;
    }

    int ObtEtageId(Transform parentHeal)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i) == parentHeal)
            {
                return i;
            }
        }
        return 0;
    }
}
