using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelExit : MonoBehaviour
{
    [SerializeField] Collider2D ownCollider;
    [SerializeField] GameManager gameManager;

    public void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            gameManager.NextLevelRoutineStart();
        }
    }
}
