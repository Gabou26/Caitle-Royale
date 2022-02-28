using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bullet Updater du jeu.
/// S'occupe de gérer le mouvement, affichage, détection de contact et gestion de vie de l'ensemble des balles de jeu.
/// Sa gestion en un endroit permet de minimiser et contrôler le nombre d'appels de chaque balle ainsi que manipuler ceux-ci facilement.
/// Points importants :
/// Liste personnalisée retire le délai requis pour atteindre un index d'array.
/// Délai d'appel : Permet d'appeler un nombre dynamique de balles afin de limiter le nombre d'appels par "frame" et minimiser les risques de "lag".
/// Affichage : Désactive l'affichage et simule son mouvement lorsqu'il est en dehors de l'écran.
/// Pooling : Limite le nombre de balles détruites et instantiées en gérant ceux-ci en arrière-plan. Est aussi détruit constamment afin d'empêcher des lag spikes.
/// Instance : Similaire à un static, permet son accès rapide pour chaque personnages et arme associée. (Peut être changé si je trouve une solution plus viable)
/// </summary>
public class BulletUpdater : MonoBehaviour
{
    public enum BulletType { BASE, FIREBALL, LIGHTNING, ROCK, STALLAGLITE, TORNADE }

    //Données des Bullets. Utilisée dans la recherche et manipulation de masse.
    public class BulletData
    {
        public Bullet bullet;
        public SpriteRenderer sprite;
        public bool stopBullet = false;
        public float delayLastShot;
        public BulletData prev, next;
    }

    public class BulletPool
    {
        public Bullet bullet;
        public BulletPool next;
    }

    //instance publique 
    public static BulletUpdater instance;

    //Références externes
    GameManager gameManager = GameManager.instance;
    public Transform bulletParent;
    [SerializeField] private Transform targetUI;
    Camera mainCamera;

    //Champs
    List<BulletData> bulletList = new List<BulletData>();

    //Intervalles (const et dyn)
    const float interLife = 1.7f, interCam = 0.16f;
    float interHit = 0.1f;
    float interDynHit, interDynLife, interDynCam;

    //Délai Courant des intervalles
    BulletData bullCurHit, bullCurLife, bullCurCam;
    float delayCurHit, delayCurLife, delayCurCam;
    float delayLastShot;

    //Champs Pooling Bullets
    Dictionary<BulletType, BulletPool> bullPoolingDict = new Dictionary<BulletType, BulletPool>();
    [SerializeField] GameObject[] bullPrefabs;
    const float interPooling = 1;
    float delayCurPooling;

    private void Awake()
    {
        //Assigne un nouvel Updater comme instance
        if (instance != null)
            Destroy(instance.gameObject);
        instance = this;

        mainCamera = Camera.main;

        foreach (BulletType bullType in System.Enum.GetValues(typeof(BulletType)))
            bullPoolingDict[bullType] = null;
    }

    //Vérification des différentes optimisations
    void Update()
    {
        VerifPooling();
        if (bulletList.Count <= 0) 
            return; //Stop Verif si aucune balle dans le jeu.

        VerifHit();
        VerifLife();
        VerifCam();
    }

    /// <summary>
    /// Ajoute la nouvelle balle dans le stack de balles.
    /// Met à jour les statistiques
    /// </summary>
    /// <param name="bullet">Nouveau bullet</param>
    public void Ajout(BulletData bullet)
    {
        if (bulletList.Count <= 0)
            bullet.delayLastShot = interLife;
        else
            bullet.delayLastShot = delayLastShot;

        bulletList.Add(bullet);

        //Ajout bullet dans le stack de bullets
        if (bulletList.Count > 1)
        {
            bullet.prev = bulletList[bulletList.Count - 2];
            bullet.prev.next = bullet;
        }
        else
        {
            bullet.prev = bullet;
            bullCurHit = bullet;
            bullCurLife = bullet;
            bullCurCam = bullet;
            delayCurLife = 0;
            interDynLife = interLife;
        }
        bullet.next = bulletList[0];
        bullet.next.prev = bullet;

        if (mainCamera != null)
            UpdateViewBul(bullet);
        ResetIntervalles();
        delayLastShot = 0;

        //Ajout du nouveau tir dans les données de jeu. (Statistiques Après-match)
        if (gameManager != null) 
            gameManager.AddShotData(bullet.bullet.caster);
    }

    /// <summary>
    /// Retire la balle de la liste active
    /// </summary>
    /// <param name="bullet"></param>
    /// <param name="hitTarget"></param>
    public void Retrait(BulletData bullet, Transform hitTarget = null)
    {
        for (int i = 0; i < bulletList.Count; i++)
        {
            if (bulletList[i].Equals(bullet))
            {                
                //Assigner Prev/next
                if (bulletList.Count > 1)
                {
                    bulletList[i].prev.next = bulletList[i].next;
                    bulletList[i].next.prev = bulletList[i].prev;
                }

                ResetVerifCour(bulletList[i]);

                bullet.bullet.DestroyProjectile(hitTarget);
                bulletList.RemoveAt(i);

                if (bulletList.Count == 0) //Empêche infinite loop
                    interDynLife = interLife;

                ResetIntervalles();
                return;
            }
        }
    }

    /// <summary>
    /// Met à jour les délais de bullets.
    /// Utilisé lorsqu'un bullet est désactivé/retiré de la liste.
    /// </summary>
    void ResetVerifCour(BulletData bullStr)
    {
        if (bullCurLife == bullStr)
        {
            delayCurLife -= interDynLife;
            bullCurLife = bullCurLife.next;
            interDynLife = bullCurLife.delayLastShot;
        }
        else if (bullStr.prev != bulletList[0])
            bullStr.prev.delayLastShot += bullStr.delayLastShot;
        else
            delayLastShot += bullStr.delayLastShot;
        if (bullCurHit == bullStr)
        {
            delayCurHit -= interDynHit;
            bullCurHit = bullCurHit.next;
        }
        if (bullCurCam == bullStr)
        {
            delayCurCam -= interDynCam;
            bullCurCam = bullCurCam.next;
        }
    }

    /// <summary>
    /// Réinitialise les intervalles d'appels pour les bullets, selon le nombre actif.
    /// </summary>
    void ResetIntervalles()
    {
        int count = bulletList.Count;
        if (count <= 0)
            return;
        interDynHit = interHit / (float)count;
        interDynCam = interCam / (float)count;

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

    /// <summary>
    /// Place le nouvel bullet dans la liste de pooling
    /// </summary>
    public void StoreBulletPool(Bullet bullet)
    {
        BulletPool newPool = new BulletPool();
        newPool.bullet = bullet;
        if (bullPoolingDict[bullet.bulletType] != null)
            newPool.next = bullPoolingDict[bullet.bulletType];
        bullPoolingDict[bullet.bulletType] = newPool;

        //Desactiver Bullet
        bullet.gameObject.SetActive(false);
    }

    /// <summary>
    /// Removes Ratio(betweem 0(0%) and 1(100%) of the pooling
    /// </summary>
    /// <param name="ratio">purcentage of pooling to be dealth with</param>
    public void EmptyPooling(float ratio = 1)
    {
        BulletPool bul;
        foreach (BulletType bullType in System.Enum.GetValues(typeof(BulletType)))
        {
            bul = bullPoolingDict[bullType];
            int length = GetPoolLength(bul);

            for (int i = 0; i < length * ratio; i++)
            {
                Destroy(bul.bullet.gameObject);
                bul = bul.next;
            }
            bullPoolingDict[bullType] = bul;
        }
    }

    /// <summary>
    /// Gets the amount of bullets inside the recursive list
    /// </summary>
    /// <param name="bul">bullets pool</param>
    int GetPoolLength(BulletPool bul)
    {
        int numbBullets = 0;
        while (bul != null)
        {
            numbBullets++;
            bul = bul.next;
        }

        return numbBullets;
    }

    /// <summary>
    /// Retourne une balle sur le stack pooling.
    /// </summary>
    /// <param name="bullType">Type de bullet désiré</param>
    public Bullet GetBullet(BulletType bullType)
    {
        Bullet bullet = null;

        if (bullPoolingDict[bullType] != null && bullPoolingDict[bullType].bullet != null)
        {
            bullet = bullPoolingDict[bullType].bullet;
            bullPoolingDict[bullType] = bullPoolingDict[bullType].next;
            bullet.gameObject.SetActive(true);
        }

        return bullet;
    }

    /// <summary>
    /// Vérifie s'il faut faire vérification de contact de bullets.
    /// </summary>
    void VerifHit()
    {
        delayCurHit += Time.deltaTime;
        while (delayCurHit >= interDynHit)
        {
            bullCurHit.bullet.DetectHit();

            delayCurHit -= interDynHit;
            bullCurHit = bullCurHit.next;
        }
    }

    /// <summary>
    /// Retire les Bullets ayant atteint leur fin de vie
    /// </summary>
    void VerifLife()
    {
        if (!targetUI)
            return;

        delayLastShot += Time.deltaTime;
        delayCurLife += Time.deltaTime;

        while (delayCurLife >= interDynLife)
            Retrait(bullCurLife);
    }

    /// <summary>
    /// Vérifie s'il faut faire vérification d'affichage de bullets.
    /// </summary>
    void VerifCam()
    {
        if (mainCamera == null)
            return;

        delayCurCam += Time.deltaTime;
        while (delayCurCam >= interDynCam)
        {
            UpdateViewBul(bullCurCam);

            delayCurCam -= interDynCam;
            bullCurCam = bullCurCam.next;
        }
    }

    /// <summary>
    /// Cache le bullet si elle n'est pas affichée dans le jeu.
    /// </summary>
    void UpdateViewBul(BulletData bul)
    {
        Vector3 camVP = mainCamera.WorldToViewportPoint(bul.bullet.transform.position);
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

    /// <summary>
    /// Appelle la destruciton d'une partie du pooling à chaque X intervalle
    /// </summary>
    void VerifPooling()
    {
        delayCurPooling += Time.deltaTime;
        while (delayCurPooling >= interPooling)
        {
            EmptyPooling(0.15f);

            delayCurPooling -= interPooling;
        }
    }

    /// <summary>
    /// Génération de balles avec nombre X.
    /// Utilisée dans le but de minimiser le lag en début de partie lorsqu'aucunes balles sont créées.
    /// </summary>
    /// <param name="amount">nombre de balles à générer</param>
    /// <param name="delayPool">délai appliqué au prochain split de pooling.</param>
    public void GenerateBullets(int amount, float delayPool)
    {
        Bullet bullet;
        int amountSpawn;
        for (int n = 0; n < bullPrefabs.Length; n++)
        {
            amountSpawn = n == 0 ? amount * 2 : amount / 4;
            for (int i = 0; i < amountSpawn; i++)
            {
                bullet = Instantiate(bullPrefabs[n], transform.position, Quaternion.identity, bulletParent).GetComponent<Bullet>();
                StoreBulletPool(bullet);
            }
        }
        delayCurPooling -= delayPool;
    }
}
