namespace Armes
{
    public class WeaponFireBall : Weapon
    {
        protected override void Fire()
        {
            /*if(countCooldown > 0f)
                return;*/
            
            base.Fire();

            //projectile.transform.rotation = transform.parent.rotation;
        }
    }
}