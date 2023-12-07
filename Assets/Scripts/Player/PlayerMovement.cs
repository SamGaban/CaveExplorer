using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] Collider2D feetCollider;
    public Rigidbody2D rigidBody;
    [SerializeField] Collider2D playerCollider;
    Vector2 moveInput;
    public Animator animator;
    [SerializeField] float playerJumpHeight;
    [SerializeField] float climbingSpeed;
    public float horizontalSpeed;

    [SerializeField] float maxVerticalSpeed = 25f;

    float originalGravityScale;

    #region Props
    private bool _isAlive = true;
    public bool IsAlive
    {
        get { return _isAlive; }
        private set { _isAlive = value; }
    }
    #endregion

    private void Start()
    {
        originalGravityScale = rigidBody.gravityScale;
        StartCoroutine(HasPlayerMoved());
    }

    private void Update()
    {
        HazardCheck();
        VerticalSpeedLimiter();

    }



    private void FixedUpdate()
    {
        if (!_isAlive) { return; }

        Run();
        FlipSprite();
        ClimbLadder();
        GravityFlip();
    }

    /// <summary>
    /// If the player's collider is touching the "Ground" layer, and the key is pressed, add vertical force to the
    /// rigidbody, through the playerJumpHeight variable.
    /// </summary>
    /// <param name="value">Key linked to the input</param>
    void OnJump(InputValue value)
    {
        if (!_isAlive) { return; }

        if (value.isPressed && feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            // Calculate the jump force as a Vector2
            Vector2 jumpForce = new Vector2(0f, playerJumpHeight);

            // Apply the jump force to the Rigidbody
            rigidBody.AddForce(jumpForce, ForceMode2D.Impulse);
        }
    }
    void OnMove(InputValue value)
    {
        if (!_isAlive) { return; }

        moveInput = value.Get<Vector2>();
    }
    void OnExitGame(InputValue value)
    {
#if UNITY_EDITOR
        // This will quit the game in the Unity Editor
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
    void OnReloadGame()
    {
        GameSession session = FindObjectOfType<GameSession>();
        session.Kill();
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Adds horizontalSpeed float's value to the X axis of the rigidbody, keeps the Y axis the same
    /// </summary>
    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * horizontalSpeed , rigidBody.velocity.y);
        rigidBody.velocity = playerVelocity;
    }
    /// <summary>
    /// When executed, if player has horizontal speed, the method turns the "isRunning" animation bool on
    /// <para>Also, it will turn the bool off if executed when the player horizontal speed is under mathf.epsilon</para>
    /// </summary>
    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(rigidBody.velocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(rigidBody.velocity.x), 1f);
            animator.SetBool("isRunning", feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) ? true:false );
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }
    /// <summary>
    /// Method that activates vertical movement upon contact with the "ladder" layer.
    /// Additionnally, it activates the "isClimbing" state, and turns it false when out of the layer
    /// </summary>
    void ClimbLadder()
    {
        bool playerHasVerticalSpeed = Mathf.Abs(rigidBody.velocity.y) > Mathf.Epsilon;

        if (playerCollider.IsTouchingLayers(LayerMask.GetMask("Ladder")))
        {
            Vector2 climbingVelocity = new Vector2(rigidBody.velocity.x, moveInput.y * climbingSpeed);
            rigidBody.velocity = climbingVelocity;
        }

        if (playerCollider.IsTouchingLayers(LayerMask.GetMask("Ladder")) && playerHasVerticalSpeed)
        {
            animator.SetBool("isClimbing", true);
        }
        else
        {
            animator.SetBool("isClimbing", false);
        }
    }
    /// <summary>
    /// Turns gravity off when touching the "Ladder" layer, so that the player doesn't drift down when climbing.
    /// Turns gravity on again if executed when player is off the layer, uses the originalGravityScale that is attributed
    /// on start.
    /// </summary>
    void GravityFlip()
    {
        rigidBody.gravityScale = (playerCollider.IsTouchingLayers(LayerMask.GetMask("Ladder"))) ? 0 : originalGravityScale;
    }
    /// <summary>
    /// turns _isAlive false
    /// <para>Sends the player flying in the modifier direction (-1 for left, +1 for right)</para>
    /// </summary>
    public void Die(float modifier)
    {
        if (!_isAlive) { return; }
        _isAlive = false;
        Vector2 throwForce = new Vector2(5f * modifier, 5f);
        rigidBody.AddForce(throwForce, ForceMode2D.Impulse);
        animator.SetBool("isDead", true);
    }
    /// <summary>
    /// Checks for contact with hazardous layers and applies death upon touch
    /// </summary>
    public void HazardCheck()
    {
        if (rigidBody.IsTouchingLayers(LayerMask.GetMask("Hazards")))
        {
            Die(0);
        }
        if (rigidBody.IsTouchingLayers(LayerMask.GetMask("Water")))
        {
            Die(0);
        }

    }
    /// <summary>
    /// Little ternary created in order to check if the rigidbody reaches a max vertical speed (maxVerticalSpeed) and, if so,
    /// caps it, and he's not able to go past it. (Mainly for boucing exploit reasons)
    /// </summary>
    public void VerticalSpeedLimiter()
    {
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, rigidBody.velocity.y > maxVerticalSpeed ? maxVerticalSpeed : rigidBody.velocity.y);
    }
    /// <summary>
    /// Coroutine tracking if player has made its first move, so that the speedrun timer can begin running
    /// </summary>
    /// <returns>Coroutine</returns>
    IEnumerator HasPlayerMoved()
    {
        yield return new WaitForSecondsRealtime(2);

        yield return new WaitUntil(() => rigidBody.velocity != new Vector2(0f, 0f));

        gameManager.PlayerMoved();
    }

}
