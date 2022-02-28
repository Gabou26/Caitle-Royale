using System.Collections;
using UnityEngine;
using static BulletUpdater;

public class Bullet : MonoBehaviour
{
    //Sprites
    [SerializeField] protected Sprite[] spritesBase, spritesMed, spritesHigh, spritesUltra;

    public GameObject caster;
    protected BulletData bulSt;
    protected BulletUpdater bullUpdater;
    protected AiUpdater aiUpdater;
    Animateur anim;

    public BulletType bulletType = BulletType.BASE;
    [SerializeField] protected int baseDamage;
    [SerializeField] protected LayerMask layerTrigger; //d
    [SerializeField] protected float lifeTime; //d

    //protected float baseDamage;
    protected Vector2 direction = Vector2.zero;
    protected float vitesseTir;
    protected bool stopBullet = false;

    //Raycast
    public float dimRaycast = 0.4f;
    RaycastHit2D hit;

    protected int damage;
    public Vector2 position;
    bool canMove = true;

    private void Start()
    {
        position = transform.position;
    }

    public void SetPosition(Vector2 position, Quaternion quat)
    {
        transform.position = position;
        transform.rotation = quat;
        this.position = position;
    }

    public void CanMove(bool canMove)
    {
        this.canMove = canMove;
    }

    public virtual void Tirer(float dmgMult, float vitesseTir, Vector2 direction, BulletUpdater bullUpdater)
    {
        this.vitesseTir = vitesseTir;
        this.direction = direction;
        this.damage = (int)(baseDamage * dmgMult);

        //Sprite Config
        anim = GetComponent<Animateur>();
        if (dmgMult <= 1.25f)
            anim.SetAnim(spritesBase);
        else if (dmgMult <= 1.5f)
            anim.SetAnim(spritesMed);
        else if (dmgMult <= 2f)
            anim.SetAnim(spritesUltra);
        else
            anim.SetAnim(spritesHigh);

        //Ajout data dans Updater
        if (bulSt == null)
        {
            bulSt = new BulletData();
            bulSt.bullet = this;
            bulSt.sprite = GetComponent<SpriteRenderer>();
            this.bullUpdater = bullUpdater;
            this.aiUpdater = AiUpdater.instance;
        }
        bulSt.stopBullet = false;
        stopBullet = false;
        GetComponent<AudioSource>().Play();
        bullUpdater.Ajout(bulSt);
    }

    private void Update()
    {
        if (stopBullet)
            return;

        position += direction * Time.deltaTime * vitesseTir;
        if (canMove)
            transform.position = position;
    }

    public virtual void DetectHit()
    {
        if (stopBullet)
            return;

        //Verif Singulière (plus léger)
        hit = Physics2D.CircleCast(transform.position, dimRaycast, Vector2.zero, 0, layerTrigger);
        if (!hit)
            return;

        if (!caster.GetInstanceID().Equals(hit.transform.gameObject.GetInstanceID()))
        {
            if (hit.transform.gameObject.activeInHierarchy)
            {
                bullUpdater.Retrait(bulSt, hit.transform);
                return;
            }
        }

        //Verif Multiple (plus lourd)
        foreach (RaycastHit2D hit in Physics2D.CircleCastAll(transform.position, dimRaycast, Vector2.zero, 0, layerTrigger))
        {
            if (!caster.GetInstanceID().Equals(hit.transform.gameObject.GetInstanceID()))
            {
                if (hit.transform.gameObject.activeInHierarchy)
                {
                    bullUpdater.Retrait(bulSt, hit.transform);
                    return;
                }

            }
        }
    }

    public virtual void DestroyProjectile(Transform other = null)
    {
        if (other)
        {
            other.GetComponent<HealSystem>()?.TakeDamage(caster, damage);
            other.GetComponent<BarrelHealth>()?.TakeDamage(caster, damage);
        }
        bullUpdater.StoreBulletPool(this);
        //Destroy(gameObject);
    }

    public IEnumerator Fin()
    {
        yield return new WaitForSeconds(1);
        bullUpdater.StoreBulletPool(this);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dimRaycast);
    }
}
