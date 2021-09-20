using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CostumePick : MonoBehaviour
{
    Animateur anim;

    void Start()
    {
        anim = GetComponent<Animateur>();
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            ReSkin(col.GetComponentInChildren<Animateur>());
            GetComponent<AudioSource>().Play();
        }
    }

    void ReSkin(Animateur anim)
    {
        anim.SetAnim(this.anim.GetAnim());
    }
}
