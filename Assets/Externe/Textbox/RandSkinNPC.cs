using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandSkinNPC : MonoBehaviour
{
    public RuntimeAnimatorController[] skinAnimators;
    private int skinId = -1;


    void Start()
    {
        ShuffleSkins();
        //Assignation d'un skin
        if (skinAnimators.Length > 0)
            AssignSkin();
    }

    void ShuffleSkins()
    {
        skinId = Random.Range(0, skinAnimators.Length);
        return;
    }


    void AssignSkin()
    {
        skinId = (skinId + 1) % skinAnimators.Length;
        Animator anim = GetComponentInChildren<Animator>();
        anim.runtimeAnimatorController = skinAnimators[skinId];
    }
}
