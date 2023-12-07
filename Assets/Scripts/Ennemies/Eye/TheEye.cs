using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheEye : MonoBehaviour
{
    [SerializeField] Transform ownTransform;
    GameObject player;
    Transform playerTransform;
    Vector2 playerPosition;
    Vector2 ownPosition;
    Vector2 shootTrajectory;

    [SerializeField] GameObject projectile;

    void Start()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Player");
        player = gameObjects[0];
        playerTransform = player.transform;
        ownPosition = ownTransform.position;
        InvokeRepeating("Shoot", 2, 1.5f);
    }

    void Update()
    {
        // Using update methods to constantly keep track of player's position and trajectory of projectile
        playerPosition = UpdatePlayerPosition();
        shootTrajectory = UpdateShootTrajectory();
    }

    /// <summary>
    /// Tracks player position
    /// </summary>
    /// <returns>Player's position in form of Vector2</returns>
    Vector2 UpdatePlayerPosition()
    {
        Vector2 toReturn = playerTransform.position;
        return toReturn;
    }
    /// <summary>
    /// Calculates the trajectory of projectile based on player's position relative to own position
    /// </summary>
    /// <returns>Shoot target in Vector2</returns>
    Vector2 UpdateShootTrajectory()
    {
        Vector2 toReturn = new Vector2(playerPosition.x - ownPosition.x, playerPosition.y - ownPosition.y);
        return toReturn;
    }
    /// <summary>
    /// Instantiates a projectile prefab, gets its script, initializes it and communicates player's position before sending it
    /// </summary>
    void Shoot()
    {
        GameObject newProjectile = Instantiate(projectile);
        Projectile script = newProjectile.GetComponent<Projectile>();
        script.InitializeProjective(ownPosition, shootTrajectory);
        script.Send();
    }
}
