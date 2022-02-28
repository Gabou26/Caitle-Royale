using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnSpell : MonoBehaviour
{

    [SerializeField] private Sprite[] sprites;

    [SerializeField] protected LayerMask playerTrigger;
    [SerializeField] private Weapon[] weapons;

    private bool spawnable = true;

    [SerializeField] private float maxCooldown;
    private float actualTime = 0;

    private Weapon activeWeapon;

    private int currentSpell;
    
    [SerializeField] private Sprite basicSprite;

    [SerializeField] private ParticleSystem particlesOnPickUp;
    [SerializeField] private ParticleSystem particlesOnSpawn;
    void Start()
    {
        Spawn();
            
    }

    void Update()
    {
        if (actualTime < maxCooldown)
        {
            actualTime += Time.deltaTime;
        }
        else
        {
            Spawn();
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & playerTrigger) != 0)
        {
            if (activeWeapon != null)
            {
                other.transform.GetChild(1).transform.GetComponentInChildren<WeaponManager>()
                    .TakeWeapon(activeWeapon.gameObject);
                activeWeapon = null;
                particlesOnPickUp.Play();
                transform.GetComponent<SpriteRenderer>().sprite = basicSprite;
                actualTime = 0;
            }
        }
    }

    private void Spawn()
    {
        if (activeWeapon is null)
        {
            particlesOnSpawn.Play();
            currentSpell = Random.Range(0, weapons.Length);
            transform.GetComponent<SpriteRenderer>().sprite = sprites[currentSpell];
            activeWeapon = weapons[currentSpell];
            
        }
    }
}
