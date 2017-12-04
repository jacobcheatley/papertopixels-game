using System.Collections;
using UnityEngine;
using XInputDotNetPure;

public class Player : MonoBehaviour
{
    // Editor Fields
    [Header("References")]
    [Header("Control")]
    [SerializeField] private PlayerIndex playerIndex;
    [SerializeField] private float maxSpeed = 8.0f;
    [Header("Dash")]
    [SerializeField] private float dashCooldown = 2.0f;
    [SerializeField] private float dashSpeed = 30.0f;
    [SerializeField] private float dashDuration = 0.2f;

    // Constants
    private const float deadMag = 0.1f;
    private const float triggerThreshold = 0.5f;

    // Private Variables
    private Rigidbody rb;
    private Vector3 currentVelocity = Vector3.zero;
    private bool dashing = false;
    private bool canDash = true;
    private GamePadState state;
    private GamePadState prevState;

    void Awake()
    {
        state = prevState = GamePad.GetState(playerIndex);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        prevState = state;
        state = GamePad.GetState(playerIndex);

        #region Movement
        Vector3 leftStick = Vector3.ClampMagnitude(new Vector3(state.ThumbSticks.Left.X, 0, state.ThumbSticks.Left.Y), 1);
        bool leftTriggerPress = state.Triggers.Left >= triggerThreshold;

        if (canDash && leftTriggerPress && leftStick.sqrMagnitude >= 0.5f)
        {
            dashing = true;
            canDash = false;
            StartCoroutine(Dash(leftStick.normalized));
        }

        if (!dashing)
            rb.velocity = leftStick * maxSpeed;

        #endregion

        #region Turning
        Vector3 rightStick = Vector3.ClampMagnitude(new Vector3(state.ThumbSticks.Right.X, 0, state.ThumbSticks.Right.Y), 1);
        if (rightStick.sqrMagnitude >= deadMag)
            gameObject.transform.rotation = Quaternion.LookRotation(rightStick);
        #endregion
    }

    private IEnumerator Dash(Vector3 direction)
    {
        rb.velocity = dashSpeed * direction;
        yield return new WaitForSeconds(dashDuration);
        dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
        Debug.Log("Dash Cooled Down");
    }
}
