using UnityEngine;

public class Tornado : Bullet
{
    AudioSource sonHit;
    const float hitCooldown = 0.2f;
    float cooldownCour = hitCooldown;

    private void Awake()
    {
        sonHit = transform.GetChild(1).GetComponent<AudioSource>();
    }

    private void LateUpdate()
    {
        if (cooldownCour < hitCooldown)
            cooldownCour += Time.deltaTime;
    }

    public override void DetectHit()
    {
        foreach (RaycastHit2D hit in Physics2D.CircleCastAll(transform.position, dimRaycast, Vector2.zero, 0, layerTrigger))
        {
            if (!caster.GetInstanceID().Equals(hit.transform.gameObject.GetInstanceID()))
            {
                if (cooldownCour >= hitCooldown)
                {
                    hit.transform.GetComponent<HealSystem>()?.TakeDamage(caster, damage);
                    hit.transform.GetComponent<BarrelHealth>()?.TakeDamage(caster, damage);
                    cooldownCour = 0;
                }
                sonHit.Play();
                Bounce(hit.point);
                //bullUpdater.Retrait(bulSt, hit.transform);
                return;
            }
        }
    }

    void Bounce(Vector2 posContact)
    {
        float angle = Vector2.Angle(transform.position, posContact) * Mathf.Rad2Deg;

        Vector2 dirHit = -(posContact - position).normalized;
        //position += direction * 2.25f;
        if (Mathf.Abs(dirHit.x) > Mathf.Abs(dirHit.y))
        {
            if (dirHit.x > 0)
                direction.x = Mathf.Abs(direction.x);
            else
                direction.x = -Mathf.Abs(direction.x);
        }
        else
        {
            if (dirHit.y > 0)
                direction.y = Mathf.Abs(direction.y);
            else
                direction.y = -Mathf.Abs(direction.y);
        }
        return;



        if (posContact.x != 0)
            direction.x = -direction.x;
        if (posContact.y != 0)
            direction.y = -direction.y;
        position += direction * 0.25f;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        col.collider.GetComponent<BarrelHealth>()?.TakeDamage(caster, damage);
        sonHit.Play();
        Bounce(col.contacts[0].normal);
    }
}
