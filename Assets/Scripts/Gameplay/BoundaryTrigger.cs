using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == player)
        {
            GameManager.instance.SetGameOver();
            Debug.Log("Player hit boundary!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            GameManager.instance.SetGameOver();
            Debug.Log("Player hit boundary trigger!");
        }
    }
}