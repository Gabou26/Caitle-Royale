using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterController2D : MonoBehaviour
{
    public static bool paused = false;
    private const int LEFT_ROTATION = 180;
    private const int RIGHT_ROTATION = 0;

    [Header("Movement")]
    public float maxSpeed;
    public float speed;
    
    [Header("Dash")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float timeDash;
    [SerializeField] private float cooldownDash;
    [SerializeField] private int numberTrail;
    
    [Header("Fixed Component")]
    public Text playerLabel;
    [SerializeField] private Animateur anim;
    [SerializeField] protected PlayerInput playerInput;
    [SerializeField] private GameObject trailSprite;
    [SerializeField] private Rigidbody2D rigidbody2D;
    [SerializeField] private SpriteRenderer spriteRenderer, spriteRWeapon;
    public GameObject cursorGamepad;

    public Vector2 direction;
    private Vector2 velTemp;
    private Vector2 directionAim;
    private Camera camera;
    private bool isDashing;
    private float cooldownDashCount;
    
    private void Start()
    {
        speed = maxSpeed;
        camera = Camera.main;
        
        DontDestroyOnLoad(transform.parent.gameObject);
    }

    private void OnEnable()
    {
        direction = new Vector2(0, 0);
    }

    private void FixedUpdate()
    {
        if(!isDashing)
        {
            if (CanMove((Vector2)transform.position, direction))
            {
                rigidbody2D.velocity = direction * (speed * Time.deltaTime);
                if (direction != Vector2.zero)
                    anim.MouvAnim();
            }
            else
                rigidbody2D.velocity = Vector2.zero;
        }
    }

    protected void Update()
    {
        CheckFlip();
        if (cooldownDashCount >= 0f)
            cooldownDashCount -= Time.deltaTime;
    }

    private bool CanMove(Vector2 pos, Vector2 dir)
    {
        if (dir.x > 0)
            dir.x = 1;
        else if (dir.x < 0)
            dir.x = -1;
        if (dir.y > 0)
            dir.y = 1;
        else if (dir.y < 0)
            dir.y = -1;
        pos += dir;

        GameManager manager = GameManager.instance;
        if (manager != null && manager is RoyaleGame)
        {
            Vector2 posMilieu = manager.ObtMoyTGroup();
            if (Mathf.Abs(pos.x - posMilieu.x) >= 12)
                return false;
            if (Mathf.Abs(pos.y - posMilieu.y) >= 9)
                return false;
        }
        return true;
    }

    private void CheckFlip()
    {
        var rotation = transform.eulerAngles;
        var localRotation = spriteRenderer.transform.localRotation.eulerAngles.y;

        if (directionAim.x >= 0f && localRotation == LEFT_ROTATION)
        {
            spriteRenderer.transform.eulerAngles = new Vector3(rotation.x, RIGHT_ROTATION, rotation.z);
            spriteRWeapon.flipY = false;
        }
        else if (directionAim.x < 0f && localRotation == RIGHT_ROTATION)
        {
            spriteRenderer.transform.eulerAngles = new Vector3(rotation.x, LEFT_ROTATION, rotation.z);
            spriteRWeapon.flipY = true;
        }
    }

    private void OnMovement(InputValue input)
    {
        if (paused)
        {
            direction = new Vector2(0,0);
            return;
        }

        direction = input.Get<Vector2>();
        #if UNITY_WEBGL
            if (playerInput.currentControlScheme.Equals("Gamepad"))
                direction.y = -direction.y;
        #endif
    }
    
    private void OnView(InputValue inputValue)
    {
        if (paused)
            return;

        var aim = directionAim;
        directionAim = inputValue.Get<Vector2>();

        switch (playerInput.currentControlScheme)
        {
            case "MouseKeyboard":
                Vector2 posMouse = camera != null ?
                    camera.ScreenToWorldPoint(directionAim) :
                    Camera.main.ScreenToWorldPoint(directionAim);
                
                Vector2 diff = posMouse - (Vector2) transform.position;
                directionAim = diff.normalized;
                break;
            case "Gamepad":
                if (directionAim.Equals(Vector2.zero))
                    directionAim = aim;
                break;
        }
    }

    private void OnDash(InputValue inputValue)
    {
        if (paused)
            return;

        if (playerInput.currentControlScheme.Equals("Gamepad") && inputValue.Get<float>() < 0.8f ||
           isDashing || cooldownDashCount > 0f)
            return;

        StartCoroutine(Dash());
    }

    private IEnumerator Dash()
    {
        GetComponent<HealSystem>().isInvincible = true;
        isDashing = true;

        var counter = 0f;
        var trailCounter = 0f;
        
        while (counter < timeDash)
        {
            rigidbody2D.velocity = direction * dashSpeed;
            if (trailCounter > timeDash / numberTrail)
            {
                var trail = Instantiate(trailSprite, transform.position, Quaternion.identity);
                trail.GetComponent<SpriteRenderer>().sprite = spriteRenderer.sprite;
                trail.transform.rotation = spriteRenderer.transform.rotation;
                trailCounter = 0f;
            }
            counter += Time.fixedDeltaTime;
            trailCounter += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        cooldownDashCount = cooldownDash;
        isDashing = false;
        GetComponent<HealSystem>().isInvincible = false;
    }

    private void OnStartGame(InputValue inputValue)
    {     
        if(inputValue.isPressed && GameManager.instance && GameManager.instance.gameObject.scene.IsValid())
            GameManager.instance.PrepareMatch();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        speed = maxSpeed;
        isDashing = false;
        cooldownDashCount = 0f;
        GetComponent<HealSystem>().isInvincible = false;
    }

    public IEnumerator Slow(float slowRate, float duration)
    {
        speed = maxSpeed * slowRate;
        SpriteRenderer spriteR = GetComponentInChildren<SpriteRenderer>();
        spriteR.color = new Color(0.3f, 0.6f, 1, 1);
        yield return new WaitForSeconds(duration);
        ResetStatus();
    }

    public void ResetStatus()
    {
        SpriteRenderer spriteR = GetComponentInChildren<SpriteRenderer>();
        spriteR.color = new Color(1, 1, 1, 1);
        speed = maxSpeed;
    }
}
