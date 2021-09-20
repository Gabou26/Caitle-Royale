using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionBarrel : MonoBehaviour
{
    public Sprite[] frames;
    SpriteRenderer spriteR;

    private void Start()
    {
        spriteR = GetComponent<SpriteRenderer>();
    }

    public void Exploser()
    {
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion()
    {
        spriteR.enabled = true;
        foreach (Sprite frame in frames)
        {
            spriteR.sprite = frame;
            yield return new WaitForSeconds(0.1f);
        }
        spriteR.enabled = false;
    }
}
