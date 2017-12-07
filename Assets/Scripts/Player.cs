using System.Collections;
using UnityEngine;
using XInputDotNetPure;

public class Player : MonoBehaviour
{
    // Editor Fields
    [Header("References")]
    [SerializeField] private Transform bulletOrigin;
    [SerializeField] private GameObject bulletPrefab;
    [Header("Control")]
    [SerializeField] public PlayerIndex playerIndex;
    [SerializeField] private float maxSpeed = 8.0f;
    [Header("Dash")]
    [SerializeField] private float dashCooldown = 2.0f;
    [SerializeField] private float dashSpeed = 30.0f;
    [SerializeField] private float dashDuration = 0.2f;
    [Header("Shooting")]
    [SerializeField] private float shootCooldown = 0.2f;
    [SerializeField] private float bulletSpeed = 40f;

    // Constants
    private const float deadMag = 0.1f;
    private const float triggerThreshold = 0.5f;

    // Private Variables
    private Rigidbody rb;
    private Collider col;
    private bool dashing = false;
    private bool canDash = true;
    private bool canShoot = true;
    private GamePadState state;
    private GamePadState prevState;
    private Vector3 currentLook = Vector3.forward;

    void Awake()
    {
        state = prevState = GamePad.GetState(playerIndex);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = rb.GetComponent<Collider>();
    }

    void Update()
    {
        prevState = state;
        state = GamePad.GetState(playerIndex);

        Movement();
        Shooting();
        Effects();
    }

    private void Movement()
    {
        Vector3 leftStick = Vector3.ClampMagnitude(new Vector3(state.ThumbSticks.Left.X, 0, state.ThumbSticks.Left.Y), 1);
        bool leftTriggerPress = state.Triggers.Left >= triggerThreshold;

        if (canDash && leftTriggerPress && leftStick.sqrMagnitude >= 0.5f)
            StartCoroutine(Dash(leftStick.normalized));

        if (!dashing)
            rb.velocity = leftStick * maxSpeed;
    }

    private void Shooting()
    {
        Vector3 rightStick = new Vector3(state.ThumbSticks.Right.X, 0, state.ThumbSticks.Right.Y).normalized;
        bool shooting = state.Triggers.Right >= triggerThreshold ||
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
        float left = dashing ? 0.5f : 0;
        float right = (canShoot ? 0 : 0.2f) + (dashing ? 0.1f : 0);
        GamePad.SetVibration(playerIndex, left, right);
    }

    private void Shoot(Vector3 direction)
    {
        if (canShoot)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletOrigin.position, Quaternion.LookRotation(direction));
            bullet.GetComponent<Rigidbody>().velocity = direction * bulletSpeed;
            bullet.GetComponent<Bullet>().playerIndex = playerIndex;
            Physics.IgnoreCollision(col, bullet.GetComponent<Collider>());
            StartCoroutine(Reload());
            Destroy(bullet, 1f);
        }
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
        GamePad.SetVibration(playerIndex, 0.3f, 0.1f);
        yield return new WaitForSeconds(dashDuration);
        GamePad.SetVibration(playerIndex, 0, 0);
        dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
