using UnityEngine;

public class TripleShot : Weapon
{
    protected override void Fire()
    {
        if(countCooldown > 0f)
            return;

        countCooldown = cooldown;

        Bullet bul1 = CreateBullet();
        Bullet bul2 = CreateBullet();
        Bullet bul3 = CreateBullet();
        //var projectile4 = Instantiate(prefabProjectile, transform.position, Quaternion.identity);
        //var projectile5 = Instantiate(prefabProjectile, transform.position, Quaternion.identity);

        bul1.caster = caster;
        bul2.caster = caster;
        bul3.caster = caster;
        //projectile4.GetComponent<Bullet>().caster = caster;
       // projectile5.GetComponent<Bullet>().caster = caster;

        directionAim = transform.parent.rotation * Vector2.right;
        bul1.Tirer(dmgMult, speed, directionAim, bullUpdater);
        //projectile.GetComponent<Rigidbody2D>().velocity = directionAim * speed;

        directionAim = transform.parent.rotation * new Vector2(Mathf.Sqrt(3)/2f, 0.5f);
        bul2.Tirer(dmgMult, speed, directionAim, bullUpdater);
        //projectile2.GetComponent<Rigidbody2D>().velocity = directionAim * speed;

        directionAim = transform.parent.rotation * new Vector2(Mathf.Sqrt(3) / 2f, -0.5f);
        bul3.Tirer(dmgMult, speed, directionAim, bullUpdater);
        //projectile3.GetComponent<Rigidbody2D>().velocity = directionAim * speed;

        /*directionAim = transform.parent.rotation * new Vector2(Mathf.Sqrt(6) / 5f, 0.75f);
        projectile4.GetComponent<Rigidbody2D>().velocity = directionAim * speed;

        directionAim = transform.parent.rotation * new Vector2(Mathf.Sqrt(6) / 5f, -0.75f);
        projectile5.GetComponent<Rigidbody2D>().velocity = directionAim * speed;*/
    }
}
