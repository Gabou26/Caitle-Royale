using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    public static bool paused = false;

    [Header("Weapon")]
    [SerializeField] protected GameObject defaultWeapon;

    [Header("Fixed Component")]
    [SerializeField] protected PlayerInput playerInput;
    [SerializeField] protected Camera cam;
    [SerializeField] protected GameObject caster;
    [SerializeField] protected Transform mouseCursor;

    protected float angleDirAim;
    protected Vector2 directionAim;
    protected Weapon currentWeapon;

    [SerializeField] protected GameObject particles;
    float damageMultiplier = 1;

    //Levels
    [SerializeField] public int[] levelTresholds;
    protected int lvlCour = 0;

    private void Start()
    {
        if(!cam) cam = Camera.main;
        TakeDefaultWeapon();
    }

    private void Update()
    {
        if (directionAim.Equals(Vector2.zero))
            currentWeapon.isFiring = false;
        if(currentWeapon && currentWeapon.isFiring)
            particles.GetComponent<ParticleSystem>().Play();
        transform.rotation = Quaternion.Euler(angleDirAim * Vector3.forward);
        mouseCursor.rotation = Quaternion.Slerp(mouseCursor.rotation, Quaternion.Euler(angleDirAim * Vector3.forward), Time.deltaTime * 50f);
        mouseCursor.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, mouseCursor.eulerAngles.z);
    }

    private void OnView(InputValue inputValue)
    {
        if (paused)
        {
            currentWeapon.isFiring = false;
            return;
        }

        var aim = directionAim;

        switch (playerInput.currentControlScheme)
        {
            case "MouseKeyboard":
                directionAim = inputValue.Get<Vector2>();
                Vector2 posMouse = cam != null ?
                    cam.ScreenToWorldPoint(directionAim) :
                    Camera.main.ScreenToWorldPoint(directionAim);
                Vector2 diff = posMouse - (Vector2)mouseCursor.position;
                directionAim = diff.normalized;
                break;
            case "Gamepad":
                directionAim = inputValue.Get<Vector2>();
                #if UNITY_WEBGL
                    directionAim.y = -directionAim.y;
                #endif
                
                if (currentWeapon && directionAim.Equals(Vector2.zero))
                {
                    directionAim = aim;
                    currentWeapon.isFiring = false;
                }
                else if(currentWeapon)
                {
                    currentWeapon.isFiring = true;
                }
                break;
        }
        
        angleDirAim = Vector2.Angle(Vector2.right, directionAim);
        var cross = Vector3.Cross(Vector2.right, directionAim);
        if (cross.z <= 0f)
            angleDirAim = 360f - angleDirAim;
    }
    
    private void OnFire(InputValue inputValue)
    {
        if (paused)
        {
            currentWeapon.isFiring = false;
            return;
        }

        if (currentWeapon)
            currentWeapon.isFiring = inputValue.isPressed;
    }

    public void TakeWeapon(GameObject newWeapon)
    {
        StopAllCoroutines();
        bool isFiring = false;
        if (currentWeapon)
        {
            isFiring = currentWeapon.isFiring;
            Destroy(currentWeapon.gameObject);
        }

        currentWeapon = Instantiate(newWeapon, transform).GetComponent<Weapon>();

        currentWeapon.transform.localPosition = new Vector2(0.75f, 0f);

        currentWeapon.isFiring = isFiring;
        currentWeapon.dmgMult = damageMultiplier;
        currentWeapon.playerInput = playerInput;
        currentWeapon.cam = cam;
        currentWeapon.caster = caster;

        if (gameObject.activeInHierarchy)
            StartCoroutine(TakeBackWeapon());
        else
            TakeDefaultWeapon();
    }

    public void TakeDefaultWeapon()
    {
        bool isFiring = false;
        if (currentWeapon)
        {
            isFiring = currentWeapon.isFiring;
            Destroy(currentWeapon.gameObject);
        }
        currentWeapon = Instantiate(defaultWeapon, transform).GetComponent<Weapon>();

        currentWeapon.transform.localPosition = new Vector2(0.75f, 0f);
        
        currentWeapon.playerInput = playerInput;
        currentWeapon.cam = cam;
        currentWeapon.caster = caster;
        currentWeapon.isFiring = isFiring;
        currentWeapon.dmgMult = damageMultiplier;
    }

    public void UpgradeDamage()
    {
        damageMultiplier += 0.15f;
        currentWeapon.dmgMult = damageMultiplier;
    }

    public void ResetDamage()
    {
        damageMultiplier = 1;
        currentWeapon.dmgMult = 1;
    }

    private IEnumerator TakeBackWeapon()
    {
        yield return new WaitForSeconds(currentWeapon.time);
        TakeDefaultWeapon();
    }
}
