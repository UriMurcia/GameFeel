using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeAttacker : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        collision.GetComponent<Player>().SetGrabbed(transform);
    }
}
