using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private GameObject deathExplosionPrefab;
    [SerializeField] private Transform gunTransform;
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
    [SerializeField] private float lavaCooldownTime = 1f;

    // Public
    public GameStats Stats;

    // Constants
    private const float deadMag = 0.01f;
    private const float triggerThreshold = 0.35f;
    private const float maxAimDistance = 7f;

    // Private Variables
    private PlayerIndex playerIndex;
    private PlayerIndex? lastDamagedBy = null;
    private Rigidbody rb;
    private Collider col;
    private bool dashing = false;
    private bool canDash = true;
    private bool canShoot = true;
    private bool lavaDamagedRecently = false;
    private GamePadState state;
    private GamePadState prevState;
    private Vector3 currentLook = Vector3.forward;
    private bool shooting;
    private PlayerUI playerUI;
    private int maxAmmo;
    private int maxHealth;
    private int damagedRecently;

    public void Init(SlotInfo slotInfo)
    {
        playerIndex = slotInfo.Index;
        bulletObject.GetComponent<Bullet>().Init(slotInfo);
        SetColor(slotInfo.Color);
        try
        {
            Stats = Persistent.PlayerStats[playerIndex];
        }
        catch (KeyNotFoundException)
        {
            Stats = new GameStats();
        }
    }

    public void SetColor(Color color)
    {
        appearance.SetColor(color);
        bulletObject.GetComponent<Bullet>().SetColor(color);
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

        transform.rotation = Quaternion.LookRotation(-transform.position + Vector3.up * transform.position.y); // Point to centre, IGNORE Y
        currentLook = -transform.position.normalized;
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
                Player otherPlayer = g.GetComponent<Player>();
                Persistent.PlayerStats[playerIndex].DashesHit++;
                if (otherPlayer.Damage(playerIndex))
                {
                    Persistent.PlayerStats[playerIndex].DashKills++;
                    Debug.Log("Dash kill");
                }
            }

        }
    }

    public bool Damage(PlayerIndex source, int damage = 1)
    {
        // Return true if die
        GameStats sourceStats = Persistent.PlayerStats[source];
        sourceStats.DamageDealt += damage;
        lastDamagedBy = source;

        SoundManager.PlayHitSound();

        return TakeDamage(damage);
    }

    public void LavaHit()
    {
        if (lavaDamagedRecently) return;

        if (!TakeDamage())
            StartCoroutine(LavaCooldown());
        else
        {
            if (lastDamagedBy != null)
                Persistent.PlayerStats[(PlayerIndex) lastDamagedBy].LavaKills++;
        }

        SoundManager.PlayLavaSound();
    }

    private bool TakeDamage(int damage = 1)
    {
        // Return true if die
        health -= damage;
        playerUI.SetHealth(health, maxHealth);
        Stats.DamageTaken += damage;

        if (health == 0)
        {
            Respawn();
            Stats.Deaths++;
            return true;
        }

        StartCoroutine(DamagedRecently());
        return false;
    }

    private IEnumerator DamagedRecently()
    {
        damagedRecently++;
        yield return new WaitForSeconds(0.3f);
        damagedRecently--;
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
        shooting = ammo > 0 && (state.Triggers.Right >= triggerThreshold ||
            state.Buttons.RightShoulder == ButtonState.Pressed && prevState.Buttons.RightShoulder == ButtonState.Released);

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
        float left = (dashing ? 0.4f : 0) + (damagedRecently > 0 ? 0.35f : 0);
        float right = (shooting ? 0.2f : 0) + (canShoot ? 0 : 0.1f) + (dashing ? 0.1f : 0) + (damagedRecently > 0 ? 0.35f : 0);
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
                      $"K/D: {Stats.ShotKills} / {Stats.Deaths} | DMG: {Stats.DamageDealt} / {Stats.DamageTaken}");
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
            Stats.ShotsFired++;

            SoundManager.PlayShootSound();
        }
    }

    private void Respawn()
    {
        // Explosion
        GameObject explosion = Instantiate(deathExplosionPrefab, transform.position, Quaternion.identity);
        var ps = explosion.GetComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = GetColor();
        Destroy(explosion, 2f);

        // SFX
        SoundManager.PlayDeathSound();

        // Gameplay
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
        damagedRecently = 0;
        lastDamagedBy = null;
        GamePad.SetVibration(playerIndex, 0, 0);
        playerUI.SetAmmo(ammo);
        rb.velocity = Vector3.zero;
    }

    private IEnumerator Reload()
    {
        canShoot = false;
        float currentTime = 0;
        Vector3 originalPosition = gunTransform.localPosition;
        while (currentTime <= shootCooldown)
        {
            float x = (shootCooldown - currentTime);
            float y = -x * (x - 1); // Quadratic movement
            gunTransform.localPosition = new Vector3(originalPosition.x, originalPosition.y, originalPosition.z - y);
            yield return null;
            currentTime += Time.deltaTime;
        }
        gunTransform.localPosition = originalPosition;
        canShoot = true;
    }

    private IEnumerator Dash(Vector3 direction)
    {
        SoundManager.PlayDashSound();
        dashing = true;
        canDash = false;
        Stats.DashesLaunched++;
        rb.velocity = dashSpeed * direction;
        playerUI.StartCooldown(dashCooldown + dashDuration);
        yield return new WaitForSeconds(dashDuration);
        dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private IEnumerator LavaCooldown()
    {
        lavaDamagedRecently = true;
        yield return new WaitForSeconds(lavaCooldownTime);
        lavaDamagedRecently = false;
    }

    public Color GetColor()
    {
        return appearance.GetColor();
    }

    public bool RefillAmmo()
    {
        if (ammo >= maxAmmo) return false;

        ammo = maxAmmo;
        playerUI.SetAmmo(ammo);
        return true;
    }

    public bool RefillHealth()
    {
        if (health >= maxHealth) return false;

        health = maxHealth;
        playerUI.SetHealth(health, maxHealth);
        return true;
    }
}
