using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobHead : MonoBehaviour
{
    [SerializeField] BlobMovement mainScript;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerMovement script = collision.gameObject.GetComponent<PlayerMovement>();
            if (!script.IsAlive) { return; }
            mainScript.Die();
        }
    }
}
