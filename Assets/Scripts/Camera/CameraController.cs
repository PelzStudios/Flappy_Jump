using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform followTarget;
    private float xOffset;

    private void Start()
    {
        if (followTarget == null)
        {
            Debug.LogError("CameraController: Follow target not assigned!");
            return;
        }

        // Get relative position of camera from player
        xOffset = transform.position.x - followTarget.position.x;
    }

    // Use LateUpdate to ensure positioning is complete after player movement
    void LateUpdate()
    {
        if (followTarget == null) return;

        // Move camera to follow player horizontally only
        Vector3 newPosition = transform.position;
        newPosition.x = followTarget.position.x + xOffset;
        transform.position = newPosition;
    }
}