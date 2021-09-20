using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelHealth : MonoBehaviour
{
    [SerializeField] private AnimationClip animClip;

    [HideInInspector] public GameObject casterExplosion;
    
    [SerializeField] private int maxHealth;

    private int currentHealth;
    private Transform explosif;

    void Start()
    {
        explosif = transform.GetChild(0);
        currentHealth = maxHealth;
        UpdateHealthColor();
    }

    public void TakeDamage(GameObject caster, int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            casterExplosion = caster;
            StartCoroutine(Exploser());
        }

        UpdateHealthColor();
    }

    void UpdateHealthColor()
    {
        float healthNorm = (float) currentHealth / maxHealth;

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();

        //Set alpha
        float alpha = 1;
        if (currentHealth <= 0)
        {
            healthNorm = 1;
            alpha = 0.45f;
        }

        sprite.color = new Color(1, healthNorm, healthNorm, alpha);
    }

    IEnumerator Exploser()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<SpriteRenderer>().color = new Color(1,1,1,0.1f);
        GetComponentInChildren<PartiManager>().Exploser(casterExplosion);

        for (int i = 0; i < explosif.childCount; i++)
        {
            explosif.GetChild(i).GetComponent<ExplosionBarrel>().Exploser();
            //explosif.GetChild(i).GetComponent<Animator>().SetBool("Explode", true);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(5f);
        currentHealth = maxHealth;
        UpdateHealthColor();
        GetComponent<BoxCollider2D>().enabled = true;
        //Destroy(gameObject);
    }
}
