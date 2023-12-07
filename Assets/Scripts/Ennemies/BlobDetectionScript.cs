using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobDetectionScript : MonoBehaviour
{
    [SerializeField] BlobMovement mainScript;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        mainScript.TrackPlayer(collision);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        mainScript.DelayResetPlayerDirection();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        mainScript.TrackPlayer(collision);
    }
}
