using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particule : MonoBehaviour
{
    [HideInInspector] public GameObject caster;
    
    //Champs
    public LayerMask layerTrigger;
    public int damage = 35;

    Vector2 posCible;
    float posCour = 1;

    private void Update()
    {
        if (posCour < 1)
        {
            posCour += Time.deltaTime * 4;
            transform.localPosition = Vector2.Lerp(Vector2.zero, posCible, posCour);
        }
    }

    public void FireDirection(Vector2 dir)
    {
        this.posCible = dir * 3;
        posCour = 0;
        RaycastHit2D hit = Physics2D.Linecast(transform.position, (Vector2)transform.position + posCible, layerTrigger);
        if (hit)
        {
            hit.transform.GetComponent<HealSystem>()?.TakeDamage(caster, damage);
            hit.transform.GetComponent<BarrelHealth>()?.TakeDamage(caster, damage);
        }
        StartCoroutine(DeathWait());
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & layerTrigger) != 0)
        {
            other.GetComponent<HealSystem>()?.TakeDamage(caster, damage);
            other.GetComponent<BarrelHealth>()?.TakeDamage(caster, damage);
        }
    }

    IEnumerator DeathWait()
    {
        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }
}
