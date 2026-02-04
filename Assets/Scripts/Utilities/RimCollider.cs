using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RimCollider : MonoBehaviour
{
    [SerializeField] private string rimSide = "left"; // "left" or "right"
    private RingSpawner parentRing;

    private void Start()
    {
        parentRing = GetComponentInParent<RingSpawner>();

        if (parentRing == null)
        {
            Debug.LogError("RimCollider: Parent RingSpawner not found!");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        // Notify parent ring about rim collision
        if (parentRing != null)
        {
            if (rimSide == "left")
            {
                parentRing.OnRimLeftEnter();
            }
            else if (rimSide == "right")
            {
                parentRing.OnRimRightEnter();
            }
        }
    }
}