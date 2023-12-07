using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] Rigidbody2D ownRigidBody;
    public float speed = 5f; // Speed of the projectile
    Vector2 directionToShoot;
    [SerializeField] Transform ownTransform;


    /// <summary>
    /// Initializes the values necessary for the projectile use
    /// </summary>
    /// <param name="origin">Starting position of the projectile</param>
    /// <param name="direction">Direction for the projectile trajectory</param>
    public void InitializeProjective(Vector2 origin, Vector2 direction)
    {
        ownTransform.position = origin;
        directionToShoot = direction;
    }
    /// <summary>
    /// Sends the projectile towards the direction to shoot to, from the starting position
    /// <para>Calls the DestroySelf() after a 10s delay</para>
    /// </summary>
    public void Send()
    {
        // Normalize the direction and multiply by speed to get velocity
        ownRigidBody.velocity = directionToShoot.normalized * speed;
        Invoke("DestroySelf", 10f);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        CheckIfPlayerTouched(col);
    }

    /// <summary>
    /// Destroys this same gameobject
    /// </summary>
    private void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Checks if a collision with the player has happened
    /// <para>If player is touched, get its script, and activates its Die()</para>
    /// </summary>
    /// <param name="collision">Collision to test for</param>
    void CheckIfPlayerTouched(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerMovement script = collision.gameObject.GetComponent<PlayerMovement>();
            script.Die(ownRigidBody.velocity.x);
        }
    }

}

