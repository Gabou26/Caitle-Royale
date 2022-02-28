using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nuage : MonoBehaviour
{
    //Champs
    SpriteRenderer spriteR;
    float dureeVie = 1, dureeCourante = 0;
    float dimC = 0, alphaC = 0;

    // Start is called before the first frame update
    void Awake()
    {
        spriteR = GetComponent<SpriteRenderer>();
        //spriteR.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        dureeCourante += Time.deltaTime;
        if (dureeCourante > dureeVie)
            dureeCourante = dureeVie;

        AjusterNuage();
    }

    void AjusterNuage()
    {
        float progres = dureeCourante / dureeVie;
        alphaC = Mathf.Lerp(1, 0, progres);

        Color coul = spriteR.color;
        coul.a = alphaC;
        spriteR.color = coul;

        if (alphaC <= 0)
            Destroy(this.gameObject);
    }

    public void Activer(float dureeVie, Vector2 pos)
    {
        transform.position = pos;
        this.dureeVie = dureeVie;
        dureeCourante = 0;
        AjusterNuage();
        spriteR.enabled = true;
    }
}