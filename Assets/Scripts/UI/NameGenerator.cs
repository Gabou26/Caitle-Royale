using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameGenerator : MonoBehaviour
{
    public static NameGenerator instance;

    //Générateur de noms
    [SerializeField] private string[] nameGenerator;
    private List<string> namesList;
    static int indexNames = 0;

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("NameGenerator");

        if (objs.Length > 1)
            Destroy(this.gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        } 
    }

    public static string ObtRandName()
    {
        if (instance)
            return instance.RandName();
        else
            return "Player";
    }

    string RandName()
    {
        if (namesList == null || namesList.Count <= 0)
            ResetNamesList();

        if (namesList.Count == 0)
            return "Player";

        int idName = Random.Range(0, namesList.Count);
        string name = namesList[idName];
        if (indexNames > 1)
            name += "-" + indexNames;

        namesList.RemoveAt(idName);
        return name;
    }

    public static void ResetNamesList(bool restartIndex = false)
    {
        if (restartIndex)
            indexNames = 0;
        if (instance)
            instance.ResetList();
    }

    void ResetList()
    {
        CharacterController2D[] playerList = FindObjectsOfType<CharacterController2D>();
        string[] usedNames = new string[playerList.Length];
        for (int i = 0; i < playerList.Length; i++)
            usedNames[i] = playerList[i].name;

        namesList = new List<string>();
        bool used;
        foreach (string name in nameGenerator)
        {
            used = false;
            foreach (string usedName in usedNames)
                if (usedName == name)
                {
                    used = true;
                    break;
                }
            if (!used)
                namesList.Add(name);
        }

        indexNames++;
    }
}
