using System.Collections;
using UnityEngine;
using static BulletUpdater;

public class Bullet : MonoBehaviour
{
    //Sprites
    [SerializeField] protected Sprite[] spritesBase, spritesMed, spritesHigh, spritesUltra;

    public GameObject caster;
    protected BulletStr bulSt;
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
            bulSt = new BulletStr();
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
        /*int max = aiUpdater.players.Count / 1;
        int countCour = 0;
        float dist = 0;
        Vector2 test;*/
        /*for (int i = 0; i < max; i++)
        {
            dist = Mathf.Max(position.x, position.x) - Mathf.Min(position.x, position.x);
            dist += Mathf.Max(position.y, position.y) - Mathf.Min(position.y, position.y);
        }
        return;*/

        /*foreach (GameObject player in aiUpdater.players)
        {
            if (caster == player)
                continue;

            dist = Mathf.Max(position.x, position.x) - Mathf.Min(position.x, position.x);
            dist += Mathf.Max(position.y, position.y) - Mathf.Min(position.y, position.y);
            //dist = Vector2.Distance(player.transform.position, transform.position);
            if (dist <= -1f)
            {
                bullUpdater.Retrait(bulSt, player.transform);
                return;
            }
            countCour++;
            if (countCour > max)
                return;
            continue;
        }
        return;*/

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

    /*protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & layerTrigger) != 0
            && !caster.GetInstanceID().Equals(other.gameObject.GetInstanceID()))
        {
            
            DestroyProjectile(other.transform);
        }
    }*/

    public virtual void DestroyProjectile(Transform other = null)
    {
        if (other)
        {
            other.GetComponent<HealSystem>()?.TakeDamage(caster, damage);
            other.GetComponent<BarrelHealth>()?.TakeDamage(caster, damage);
        }
        bullUpdater.StoreBullet(this);
        //Destroy(gameObject);
    }

    public IEnumerator Fin()
    {
        yield return new WaitForSeconds(1);
        bullUpdater.StoreBullet(this);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dimRaycast);
    }
}
