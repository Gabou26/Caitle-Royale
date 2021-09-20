using System.Collections;
using UnityEngine;

public class Stalactite : Bullet
{
    [SerializeField] private float slowTime;
    [SerializeField] private float slowRate;
    public AudioSource finVieSFX;
    PolygonCollider2D polygon;
    SpriteRenderer spriteR;

    private void Awake()
    {
        finVieSFX = transform.GetChild(0).GetComponent<AudioSource>();
        spriteR = GetComponent<SpriteRenderer>();
        polygon = GetComponent<PolygonCollider2D>();
    }

    /*public override void Tirer(float vitesseTir, Vector2 direction, BulletUpdater bullUpdater)
    {
        base.Tirer(vitesseTir, direction, bullUpdater);
        spriteR.enabled = true;
    }*/

    public override void DestroyProjectile(Transform other)
    {
        if (stopBullet)
            return;
        if (other == null || !other.gameObject.activeInHierarchy)
            return;
        other.GetComponent<CharacterController2D>()?.StartCoroutine(other.GetComponent<CharacterController2D>()?.Slow(slowRate, slowTime));
        other.GetComponent<PromenadeAI>()?.StartCoroutine(other.GetComponent<PromenadeAI>()?.Slow(slowRate, slowTime));
        if (other)
        {
            other.GetComponent<HealSystem>()?.TakeDamage(caster, damage);
            other.GetComponent<BarrelHealth>()?.TakeDamage(caster, damage);
        }
        GetComponent<AudioSource>().Stop();
        finVieSFX.Play();
        spriteR.enabled = false;
        stopBullet = true;
        StartCoroutine(Fin());
    }
}
