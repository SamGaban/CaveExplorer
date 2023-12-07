using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlobMovement : MonoBehaviour
{
    [SerializeField] Collider2D bodyCollider;
    [SerializeField] Rigidbody2D rigidBody;
    [SerializeField] Animator animator;
    [SerializeField] Transform transformComponent;
    [SerializeField] float moveSpeed;
    [SerializeField] Collider2D feetCollider;
    [SerializeField] ParticleSystem particlePlayer;

    float playerDirection = 0f;
    [SerializeField] float jumpHeight = 1f;

    void Start()
    {
        
    }

    void Update()
    {
        RunAnimation();
        RunTowards(playerDirection);
    }

    /// <summary>
    /// Checks if the entity is moving, and if so, switches the "isRunning" animator bool on, while also
    /// flipping the entity's sprite related to direction of run.
    /// </summary>
    void RunAnimation()
    {
        if (Mathf.Abs(rigidBody.velocity.x) > Mathf.Epsilon)
        {
            animator.SetBool("isRunning", true);
            transform.localScale = new Vector2(Mathf.Sign(rigidBody.velocity.x), 1f);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }
    /// <summary>
    /// Runs toward the given direction
    /// </summary>
    /// <param name="direction">int parameter indicating direction to run to, -1 is left, 1 is right, 0 is idle</param>
    void RunTowards(float direction)
    {
        rigidBody.velocity = new Vector2(moveSpeed * direction, rigidBody.velocity.y);
    }
    /// <summary>
    /// Simply resets the player's direction to 0
    /// </summary>
    void ResetPlayerDirection()
    {
        playerDirection = 0f;
    }
    /// <summary>
    /// Checks for player position and updates playerDirection relatively
    /// <para>Makes the ennemy jump if the player is over him</para>
    /// </summary>
    /// <param name="toTrack">Collider of the player that entered the monster's detection range</param>
    public void TrackPlayer(Collider2D toTrack)
    {
        if (toTrack.gameObject.tag == "Player")
        {
            PlayerMovement script = toTrack.gameObject.GetComponent<PlayerMovement>();
            if (!script.IsAlive) { return; }
            float directionRaw = -(transform.position.x - toTrack.transform.position.x);
            playerDirection = Mathf.Sign(directionRaw);
        }

        float upDirectionRaw = -(transform.position.y - toTrack.transform.position.y);

        if (feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) && upDirectionRaw > 1)
        {
            Jump();
        }

    }

    /// <summary>
    /// Adds force to the rigidbody and sends the entity up, only works if its feet touch the ground.
    /// </summary>
    void Jump()
    {
        // Calculate the jump force as a Vector2
        Vector2 jumpForce = new Vector2(0f, jumpHeight);

        // Apply the jump force to the Rigidbody
        rigidBody.AddForce(jumpForce, ForceMode2D.Impulse);
    }
    /// <summary>
    /// Calls ResetPlayerDirection() after a 2s delay.
    /// </summary>
    public void DelayResetPlayerDirection()
    {
        Invoke("ResetPlayerDirection", 2);
    }
    /// <summary>
    /// Destroys the ennemy gameObject, detaches its particle effect the time to play it
    /// and then destroys the particle effect too.
    /// </summary>
    public void Die()
    {
        particlePlayer.transform.parent = null; // Detach the particle system
        particlePlayer.Play();

        // Destroy the particle system after its duration
        Destroy(particlePlayer.gameObject, particlePlayer.main.duration);

        // Destroy the GameObject
        Destroy(this.gameObject);
    }
    /// <summary>
    /// Sends player flying and kills him
    /// </summary>
    /// <param name="collision">Player</param>
    void KillPlayer(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerMovement script = collision.gameObject.GetComponent<PlayerMovement>();
            script.Die(Mathf.Sign(rigidBody.velocity.x));
        }
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        KillPlayer(coll);
    }


}
