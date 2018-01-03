using System.Collections;
using UnityEngine;
using XInputDotNetPure;

public class Player : MonoBehaviour
{
    // Editor Fields
    [Header("References")]
    [SerializeField] private Transform bulletOrigin;
    [SerializeField] private GameObject bulletObject;
    [SerializeField] private PlayerAppearance appearance;
    [SerializeField] private GameObject followUIPrefab;
    [SerializeField] private LineRenderer aimLine;
    [Header("Control")]
    [SerializeField] private float accelFactor = 8.0f;
    [Header("Dash")]
    [SerializeField] private float dashCooldown = 2.0f;
    [SerializeField] private float dashSpeed = 30.0f;
    [SerializeField] private float dashDuration = 0.2f;
    [Header("Shooting")]
    [SerializeField] private float shootCooldown = 0.2f;
    [SerializeField] private float bulletSpeed = 40f;
    [SerializeField] private int ammo = 6;
    [Header("Other")]
    [SerializeField] private int health = 4;
    [SerializeField] private LayerMask aimMask;

    // Public
    public GameStats Stats;

    // Constants
    private const float deadMag = 0.1f;
    private const float triggerThreshold = 0.5f;
    private const float maxAimDistance = 7f;

    // Private Variables
    private PlayerIndex playerIndex;
    private Rigidbody rb;
    private Collider col;
    private bool dashing = false;
    private bool canDash = true;
    private bool canShoot = true;
    private GamePadState state;
    private GamePadState prevState;
    private Vector3 currentLook = Vector3.forward;
    private bool shooting;
    private PlayerUI playerUI;
    private int maxAmmo;
    private int maxHealth;

    public void Init(SlotInfo slotInfo)
    {
        playerIndex = slotInfo.Index;
        appearance.SetColor(slotInfo.Color);
        bulletObject.GetComponent<Bullet>().Init(slotInfo);
        Stats = Persistent.PlayerStats[playerIndex];
    }

    void Start()
    {
        state = prevState = GamePad.GetState(playerIndex);
        rb = GetComponent<Rigidbody>();
        col = rb.GetComponent<Collider>();

        playerUI = Instantiate(followUIPrefab).GetComponent<PlayerUI>();
        playerUI.Init(transform);
        playerUI.SetAmmo(ammo);

        maxAmmo = ammo;
        maxHealth = health;
    }

    void Update()
    {
        prevState = state;
        state = GamePad.GetState(playerIndex);
        
        Movement();
        Shooting();
        Effects();

        DebugInteraction();
    }

    void OnCollisionEnter(Collision other)
    {
        GameObject g = other.gameObject;

        if (g.tag == "Player")
        {
            if (dashing)
            {
                Player player = g.GetComponent<Player>();
                player.Damage(playerIndex, 1);
            }

        }
    }

    public void Damage(PlayerIndex source, int damage)
    {
        health -= damage;
        playerUI.SetHealth(health, maxHealth);

        Debug.Log($"{source} -> {playerIndex}");

        GameStats sourceStats = Persistent.PlayerStats[source];
        Stats.DamageTaken += damage;
        sourceStats.DamageDealt += damage;

        if (health == 0)
        {
            Respawn();
            Stats.Deaths++;
            sourceStats.Kills++;
        }
    }

    private void Movement()
    {
        Vector3 leftStick = Vector3.ClampMagnitude(new Vector3(state.ThumbSticks.Left.X, 0, state.ThumbSticks.Left.Y), 1);
        bool dashButton = state.Triggers.Left >= triggerThreshold || state.Buttons.LeftShoulder == ButtonState.Pressed;

        if (canDash && dashButton && leftStick.sqrMagnitude >= 0.5f)
            StartCoroutine(Dash(leftStick.normalized));

        if (!dashing)
            rb.AddForce(leftStick * accelFactor);
    }

    private void Shooting()
    {
        Vector3 rightStick = new Vector3(state.ThumbSticks.Right.X, 0, state.ThumbSticks.Right.Y).normalized;
        shooting = state.Triggers.Right >= triggerThreshold ||
            state.Buttons.RightShoulder == ButtonState.Pressed && prevState.Buttons.RightShoulder == ButtonState.Released;

        if (rightStick.sqrMagnitude >= deadMag)
        {
            transform.rotation = Quaternion.LookRotation(rightStick);
            currentLook = rightStick.normalized;
        }

        if (shooting)
            Shoot(currentLook);
    }

    private void Effects()
    {
        // Vibration
        float left = dashing ? 0.5f : 0;
        float right = (shooting ? 0.1f : 0) + (canShoot ? 0 : 0.1f) + (dashing ? 0.1f : 0);
        GamePad.SetVibration(playerIndex, left, right);

        // Aiming Line
        Vector3 start = aimLine.gameObject.transform.position;
        aimLine.SetPosition(0, start);
        RaycastHit info;
        bool hit = Physics.Raycast(new Ray(start, currentLook), out info, maxAimDistance, aimMask);
        if (hit)
            aimLine.SetPosition(1, info.point);
        else
            aimLine.SetPosition(1, start + currentLook * maxAimDistance);
    }

    private void DebugInteraction()
    {
        if (state.Buttons.Y == ButtonState.Pressed && prevState.Buttons.Y == ButtonState.Released)
        {
            Debug.Log($"Stats for {playerIndex} ({GetColor()})\n" +
                      $"K/D: {Stats.Kills} / {Stats.Deaths} | DMG: {Stats.DamageDealt} / {Stats.DamageTaken}");
        }
    }

    private void Shoot(Vector3 direction)
    {
        if (canShoot && ammo > 0)
        {
            // Bullet shot
            GameObject bullet = Instantiate(bulletObject, bulletOrigin.position, Quaternion.LookRotation(direction));
            bullet.GetComponent<Rigidbody>().velocity = direction * bulletSpeed;
            Physics.IgnoreCollision(col, bullet.GetComponent<Collider>());
            bullet.SetActive(true);
            StartCoroutine(Reload());
            Destroy(bullet, 2f);

            ammo--;
            playerUI.SetAmmo(ammo);
        }
    }

    private void Respawn()
    {
        // TODO: Explode myself
        Refresh();
        GameControl.Respawn(this);
    }

    private void Refresh()
    {
        health = maxHealth;
        playerUI.SetHealth(health, maxHealth);
        ammo = maxAmmo;
        canDash = true;
        dashing = false;
        canShoot = true;
        GamePad.SetVibration(playerIndex, 0, 0);
        playerUI.SetAmmo(ammo);
        rb.velocity = Vector3.zero;
    }

    private IEnumerator Reload()
    {
        canShoot = false;
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }

    private IEnumerator Dash(Vector3 direction)
    {
        dashing = true;
        canDash = false;
        rb.velocity = dashSpeed * direction;
        playerUI.StartCooldown(dashCooldown + dashDuration);
        yield return new WaitForSeconds(dashDuration);
        dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public Color GetColor()
    {
        return appearance.GetColor();
    }

    public void RefillAmmo()
    {
        ammo = maxAmmo;
        playerUI.SetAmmo(ammo);
    }
}
