using UnityEngine;
using UnityEngine.SceneManagement;

public class HealSystem : MonoBehaviour
{
    [HideInInspector] public bool isInvincible;
    
    [SerializeField] private int maxHealth;
    [SerializeField] private ParticleSystem healParti;
    [SerializeField] private HealthBar healthBar;

    private float currentHealth, baseHealth;
    
    protected void Start()
    {
        baseHealth = maxHealth;
        ResetHealth();
    }

    public void TakeDamage(GameObject killer, int damage)
    {
        GameManager manager = GameManager.instance;
        if (isInvincible || manager == null)
            return;

        //Désactivé le PvP/Suicide em mode royale (temporaire?)
        if (manager is RoyaleGame)
        {
            if (gameObject.layer == 8) //Joueur
            {
                if (manager.playersScores.Count > 0 && manager.playersScores[killer].isPlayer)
                    return;
                else if (killer != null && killer.gameObject.layer == 8)
                    return;
            }
            else if (gameObject.layer == 14) //AI (empêche seulement le suicide) 
            {
                if (killer.GetInstanceID().Equals(gameObject.GetInstanceID()))
                    return;
            }
        }     

        if (!killer.GetInstanceID().Equals(gameObject.GetInstanceID()))
            manager.AddDamageScore(killer, damage); //Legit hit

        if (damage >= currentHealth)
            Death(killer);
        else
            currentHealth -= damage;

        manager.AddDmgTakenScore(this.gameObject, damage);
        healthBar.SetHealth(currentHealth);
    }

    public virtual void Death(GameObject killer)
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            if (killer.GetInstanceID().Equals(gameObject.GetInstanceID()))
                GameManager.instance.AddSuicideScore(gameObject); //suicide
            else
            {
                //kill
                if (killer != null && killer.activeInHierarchy)
                {
                    HealSystem healKiller = killer.GetComponent<HealSystem>();
                    healKiller.RecoverHalf();
                    healKiller.UpgradeHealth();
                } 
                GameManager.instance.AddKillScore(killer);
                GameManager.instance.AddDeathScore(gameObject);
            }
        }

        PlayerManager.instance.GetComponent<AudioSource>().Play();
        var lifeGame = GameManager.instance as LifeGame;
        var royaleGame = GameManager.instance as RoyaleGame;
        if (lifeGame && lifeGame.isPlaying)
        {
            if (lifeGame.LooseLife(gameObject))
                PlayerManager.instance.StartCoroutine(GameManager.instance.Respawn(gameObject));
            else
            {
                //Modif TargetGroup Cam
                GameManager.instance.DisablePlayer(gameObject);

                lifeGame.CheckGame();
            }
        }
        else if (royaleGame && royaleGame.isPlaying)
        {
            if (royaleGame.LooseLife(gameObject))
                PlayerManager.instance.StartCoroutine(GameManager.instance.Respawn(gameObject));
            else
            {
                //Modif TargetGroup Cam
                RoyaleGame royale = (RoyaleGame)GameManager.instance;
                royale.DisablePlayer(gameObject);
                royale.ReplaceTarget(killer);

                royaleGame.CheckGame();
            }
        }
        else
        {
            if(GameManager.instance && GameManager.instance.gameObject.scene.IsValid())
                PlayerManager.instance.StartCoroutine(GameManager.instance.Respawn(gameObject));
            else
                PlayerManager.instance.StartCoroutine(PlayerManager.instance.Respawn(gameObject));
        }
    }

    public void Recover(bool effect = false)
    {
        currentHealth = maxHealth;
        GetComponent<CharacterController2D>()?.ResetStatus();
        GetComponent<PromenadeAI>()?.ResetStatus();
        healthBar.SetHealth(currentHealth);

        if (effect)
        {
            healParti.Play();
            healParti.GetComponent<AudioSource>().Play();
        }
    }

    public void RecoverHalf(bool effect = false)
    {
        currentHealth += maxHealth / 2;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        GetComponent<CharacterController2D>()?.ResetStatus();
        GetComponent<PromenadeAI>()?.ResetStatus();
        healthBar.SetHealth(currentHealth);

        if (effect)
        {
            healParti.Play();
            healParti.GetComponent<AudioSource>().Play();
        }
    }

    public void UpgradeHealth()
    {
        GetComponentInChildren<WeaponManager>().UpgradeDamage();
        /*float rand = Random.Range(0, 1);
        if (rand <= 0.33) {
            GetComponentInChildren<WeaponManager>().UpgradeDamage();
        }*/

        maxHealth += 6;
        healthBar.SetMaxHealth(maxHealth, currentHealth);
        if (gameObject.layer == 8)
            GetComponentInChildren<UpgradePlayerText>().FlashText("DMG++", maxHealth);
    }

    public void ResetHealth()
    {
        maxHealth = (int)baseHealth;
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth, currentHealth);
    }

    public float GetHealthRatio()
    {
        return currentHealth == 0 ? 0 : currentHealth / maxHealth;
    }
}
