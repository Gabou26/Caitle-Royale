using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static BulletUpdater;

public class Weapon : MonoBehaviour
{
    public BulletType bulletType = BulletType.BASE;
    [HideInInspector] public PlayerInput playerInput;
    [HideInInspector] public Camera cam;
    [HideInInspector] public GameObject caster;
    [HideInInspector] public bool isFiring;
    [HideInInspector] public float dmgMult = 1f;

    public float time = 8f;
    
    [SerializeField] protected GameObject prefabProjectile;
    [SerializeField] protected float cooldown = 0.2f;
    [SerializeField] protected float speed = 5f;
    [SerializeField] protected bool rotateFire = false;
    protected BulletUpdater bullUpdater;

    private Vector2 mousePos;
    protected Vector2 directionAim;
    protected float countCooldown;
    protected GameObject projectile;

    void Start()
    {
        bullUpdater = BulletUpdater.instance;
        directionAim = Vector2.right;
    }

    void Update()
    {
        if (countCooldown > 0f)
            countCooldown -= Time.deltaTime;
        else if(isFiring)
            Fire();
    }

    public void SetDelay(float delaiShoot)
    {
        this.enabled = false;
        this.countCooldown = delaiShoot * cooldown;
    }

    protected virtual void Fire()
    {
        countCooldown += cooldown;

        Bullet bullet = CreateBullet();

        bullet.caster = caster;
        directionAim = transform.parent.rotation * Vector2.right;
        //projectile.GetComponent<Rigidbody2D>().velocity = directionAim * speed;

        if (rotateFire)
        {
            bullet.Tirer(dmgMult, speed, directionAim, bullUpdater);
            bullet.transform.rotation = transform.parent.rotation;
        }
        else
            bullet.Tirer(dmgMult, speed, directionAim, bullUpdater);
    }

    protected Bullet CreateBullet()
    {
        Bullet bullet = bullUpdater.GetBullet(bulletType);
        if (bullet == null)
            bullet = Instantiate(prefabProjectile, transform.position, Quaternion.identity, bullUpdater.bulletParent).GetComponent<Bullet>();
        else
            bullet.SetPosition(transform.position, Quaternion.identity);
        return bullet;
    }

    private void OnDisable()
    {
        isFiring = false;
    }
}
