using System.Collections;
using UnityEngine;

public class FireBall : Bullet
{
    [SerializeField] private float explosionRadius;
    
    private Transform explosif;
    private bool isDestroying;

    public override void Tirer(float dmgMult, float vitesseTir, Vector2 direction, BulletUpdater bullUpdater)
    {
        explosif = transform.GetChild(0);

        base.Tirer(dmgMult, vitesseTir, direction, bullUpdater);
        isDestroying = false;
        //GetComponent<SpriteRenderer>().enabled = true;
        explosif.GetComponent<AudioSource>().Play();
    }

    public override void DestroyProjectile(Transform other)
    {
        if(isDestroying)
            return;
        
        isDestroying = true;
        RaycastHit2D[] targets = Physics2D.CircleCastAll(transform.position, explosionRadius, Vector2.zero, layerTrigger);
        foreach (var target in targets)
        {
            target.collider.GetComponent<HealSystem>()?.TakeDamage(caster, damage);
            target.collider.GetComponent<BarrelHealth>()?.TakeDamage(caster, damage);
        }
        StartCoroutine(Exploser());
    }

    IEnumerator Exploser()
    {
        //GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        stopBullet = true;
        GetComponent<SpriteRenderer>().enabled = false;
        explosif.GetComponent<AudioSource>().Stop();

        Transform exploChild;
        for (int i = 0; i < explosif.childCount; i++)
        {
            exploChild = explosif.GetChild(i);
            exploChild.GetComponent<SpriteRenderer>().enabled = true;
            exploChild.GetComponent<Animator>().SetBool("Explode", true);
            exploChild.GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(0.1f);
        }
        
        yield return new WaitForSeconds(1);
        bullUpdater.StoreBullet(this);
        //Destroy(gameObject);
    }
}
