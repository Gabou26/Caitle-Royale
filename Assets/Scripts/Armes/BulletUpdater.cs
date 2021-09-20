using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletUpdater : MonoBehaviour
{
    public enum BulletType { BASE, FIREBALL, LIGHTNING, ROCK, STALLAGLITE, TORNADE }
    public class BulletStr
    {
        public Bullet bullet;
        public SpriteRenderer sprite;
        public bool stopBullet = false;
        public float delaiDernTir;
        public BulletStr prev, next;
    }

    public class BulletPool
    {
        public Bullet bullet;
        public BulletPool next;
    }

    //Refs
    public static BulletUpdater instance;
    [SerializeField] private Transform targetUI;

    //Champs Vals
    [SerializeField] public LayerMask layerTrigger;

    //Champs
    List<BulletStr> bullets = new List<BulletStr>();
    Dictionary<BulletType, BulletPool> pooledBlt = new Dictionary<BulletType, BulletPool>();

    //Intervalles (const et dyn)
    const float interSprite = 0.5f, interLife = 1.7f, interCam = 0.16f;
    float interHit = 0.1f;
    float interSpriteDyn, interHitDyn, interLifeDyn, interCamDyn;

    BulletStr spriteCour, hitCour, lifeCour, camCour;
    float delaiSpriteCour, delaiHitCour, delaiLifeCour, delaiCamCour;
    float delaiTir;

    Camera cam;
    Vector3 camVP;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        instance = this;

        cam = Camera.main;

        foreach (BulletType bullType in System.Enum.GetValues(typeof(BulletType)))
            pooledBlt[bullType] = null;
    }

    void Update()
    {
        if (bullets.Count <= 0)
            return;
        VerifHit();
        VerifLife();
        VerifCam();
        //VerifSprite();
    }

    public void ExtendHitDelay(float delay)
    {
        delaiHitCour += delay;
    }

    public void Ajout(BulletStr bullet)
    {
        if (bullets.Count <= 0)
            bullet.delaiDernTir = interLife;
        else
            bullet.delaiDernTir = delaiTir;

        bullets.Add(bullet);

        //Assigner Prev/next
        if (bullets.Count > 1)
        {
            bullet.prev = bullets[bullets.Count - 2];
            bullet.prev.next = bullet;
        }
        else
        {
            bullet.prev = bullet;
            spriteCour = bullet;
            hitCour = bullet;
            lifeCour = bullet;
            camCour = bullet;
            delaiLifeCour = 0;
            interLifeDyn = interLife;
        }
        bullet.next = bullets[0];
        bullet.next.prev = bullet;

        if (cam != null)
            UpdateViewBul(bullet);
        //bullet.bullet.DetectHit();
        ResetIntervalles();
        delaiTir = 0;
    }

    public void Retrait(BulletStr bullet, Transform hitTarget = null)
    {
        for (int i = 0; i < bullets.Count; i++)
        {
            if (bullets[i].Equals(bullet))
            {                
                //Assigner Prev/next
                if (bullets.Count > 1)
                {
                    bullets[i].prev.next = bullets[i].next;
                    bullets[i].next.prev = bullets[i].prev;
                }

                ResetVerifCour(bullets[i]);

                bullet.bullet.DestroyProjectile(hitTarget);
                bullets.RemoveAt(i);

                if (bullets.Count == 0) //Empêche infinite loop
                    interLifeDyn = interLife;

                ResetIntervalles();
                return;
            }
        }
    }

    void ResetVerifCour(BulletStr bullStr)
    {
        if (lifeCour == bullStr)
        {
            delaiLifeCour -= interLifeDyn;
            lifeCour = lifeCour.next;
            interLifeDyn = lifeCour.delaiDernTir;
        }
        else if (bullStr.prev != bullets[0])
            bullStr.prev.delaiDernTir += bullStr.delaiDernTir;
        else
            delaiTir += bullStr.delaiDernTir;
        if (hitCour == bullStr)
        {
            delaiHitCour -= interHitDyn;
            hitCour = hitCour.next;
        }
        if (camCour == bullStr)
        {
            delaiCamCour -= interCamDyn;
            camCour = camCour.next;
        }
    }

    void ResetIntervalles()
    {
        int count = bullets.Count;
        if (count <= 0)
            return;
        interHitDyn = interHit / (float)count;
        interSpriteDyn = interSprite / (float)count;
        interCamDyn = interCam / (float)count;

        //Modif Hit selon nb actif
        if (count < 50)
            interHit = 0.01f;
        else if (count < 200)
            interHit = 0.02f;
        else if (count < 450)
            interHit = 0.03f;
        else if (count < 800)
            interHit = 0.05f;
        else if (count < 1300)
            interHit = 0.07f;
        else if (count < 2000)
            interHit = 0.1f;
        else
            interHit = 0.2f;
    }

    public void StoreBullet(Bullet bullet)
    {
        BulletPool newPool = new BulletPool();
        newPool.bullet = bullet;
        if (pooledBlt[bullet.bulletType] != null)
            newPool.next = pooledBlt[bullet.bulletType];
        pooledBlt[bullet.bulletType] = newPool;

        //Desactiver Bullet
        bullet.gameObject.SetActive(false);
    }

    public void EmptyPooling()
    {
        BulletPool bul;
        foreach (BulletPool bullet in pooledBlt.Values)
        {
            bul = bullet;
            while (bul != null)
            {
                Destroy(bul.bullet.gameObject);
                bul = bul.next;
            }
        }
        foreach (BulletType bullType in System.Enum.GetValues(typeof(BulletType)))
            pooledBlt[bullType] = null;
    }

    public Bullet GetBullet(BulletType bullType)
    {
        Bullet bullet = null;

        if (pooledBlt[bullType] != null && pooledBlt[bullType].bullet != null)
        {
            bullet = pooledBlt[bullType].bullet;
            pooledBlt[bullType] = pooledBlt[bullType].next;
            bullet.gameObject.SetActive(true);
        }

        return bullet;
    }

    void VerifHit()
    {
        delaiHitCour += Time.deltaTime;
        while (delaiHitCour >= interHitDyn)
        {
            hitCour.bullet.DetectHit();

            delaiHitCour -= interHitDyn;
            hitCour = hitCour.next;
        }
    }

    void VerifLife()
    {
        if (!targetUI)
            return;

        delaiTir += Time.deltaTime;
        delaiLifeCour += Time.deltaTime;
        while (delaiLifeCour >= interLifeDyn)
        {
            Retrait(lifeCour);
        }
    }

    void VerifCam()
    {
        if (cam == null)
            return;

        delaiCamCour += Time.deltaTime;
        while (delaiCamCour >= interCamDyn)
        {
            UpdateViewBul(camCour);

            delaiCamCour -= interCamDyn;
            camCour = camCour.next;
        }
    }

    void UpdateViewBul(BulletStr bul)
    {
        camVP = cam.WorldToViewportPoint(bul.bullet.transform.position);
        if (camVP.x > -0.25f && camVP.x <= 1.25f && camVP.y > -0.25f && camVP.y <= 1.25f)
        {
            bul.bullet.CanMove(true);
            bul.sprite.enabled = true;
        }
        else
        {
            bul.bullet.CanMove(false);
            bul.sprite.enabled = false;
        }
    }
}
