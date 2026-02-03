using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenPositioner : MonoBehaviour
{
    [Range(-1, 2)]
    public float horizontalOffset = 0.5f;
    [Range(-1, 2)]
    public float verticalOffset = 0.5f;
    public bool updateEveryFrame = false;

    private void Start()
    {
        UpdatePosition();
    }

    void Update()
    {
        if (updateEveryFrame)
        {
            UpdatePosition();
        }
    }

    public void UpdatePosition()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("ScreenPositioner: Main camera not found!");
            return;
        }

        // Convert screen percentage to world position
        float worldX = mainCamera.ScreenToWorldPoint(
            new Vector3(
                Mathf.LerpUnclamped(0, Screen.width, horizontalOffset), 
                transform.position.y, 
                transform.position.z
            )
        ).x;

        float worldY = mainCamera.ScreenToWorldPoint(
            new Vector3(
                transform.position.x, 
                Mathf.LerpUnclamped(0, Screen.height, verticalOffset), 
                transform.position.z
            )
        ).y;

        transform.position = new Vector3(worldX, worldY, transform.position.z);
    }
}