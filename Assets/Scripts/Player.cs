﻿using System.Collections;
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
    [Header("Control")]
    [SerializeField] private float accelFactor = 8.0f;
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

    public void Init(SlotInfo slotInfo)
    {
        playerIndex = slotInfo.Index;
        appearance.SetColor(slotInfo.Color);
        bulletObject.GetComponent<Bullet>().Init(slotInfo);
    }

    void Start()
    {
        state = prevState = GamePad.GetState(playerIndex);
        rb = GetComponent<Rigidbody>();
        col = rb.GetComponent<Collider>();

        playerUI = Instantiate(followUIPrefab).GetComponent<PlayerUI>();
        playerUI.Init(transform);
    }

    void Update()
    {
        prevState = state;
        state = GamePad.GetState(playerIndex);

        Movement();
        Shooting();
        Effects();
    }

    void OnCollisionEnter(Collision other)
    {
        GameObject g = other.gameObject;

        if (g.tag == "Player")
        {
            if (dashing)
            {
                Player player = g.GetComponent<Player>();
                Debug.Log($"{playerIndex} hit {player.GetIndex()} with dash");
//                g.GetComponent<Rigidbody>().AddForce(rb.velocity, ForceMode.Impulse);
            }

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
        float left = dashing ? 0.5f : 0;
        float right = (shooting ? 0.1f : 0) + (canShoot ? 0 : 0.1f) + (dashing ? 0.1f : 0);
        GamePad.SetVibration(playerIndex, left, right);
    }

    private void Shoot(Vector3 direction)
    {
        if (canShoot)
        {
            // Bullet shot
            GameObject bullet = Instantiate(bulletObject, bulletOrigin.position, Quaternion.LookRotation(direction));
            bullet.GetComponent<Rigidbody>().velocity = direction * bulletSpeed;
            Physics.IgnoreCollision(col, bullet.GetComponent<Collider>());
            bullet.SetActive(true);
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
        playerUI.StartCooldown(dashCooldown + dashDuration);
        yield return new WaitForSeconds(dashDuration);
        dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public PlayerIndex GetIndex()
    {
        return playerIndex;
    }
}
