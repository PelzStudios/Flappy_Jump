using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackAndForth : MonoBehaviour
{
    [SerializeField] private float distance = 50f;
    [SerializeField] private float speed = 2f;

    private RectTransform rect;
    private float startX;
    private float direction = 1f;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        startX = rect.anchoredPosition.x;
    }

    private void Update()
    {
        rect.anchoredPosition += new Vector2(speed * direction, 0) * Time.deltaTime;

        if (rect.anchoredPosition.x >= startX + distance)
        {
            direction = -1f;
        }
        else if (rect.anchoredPosition.x <= startX - distance)
        {
            direction = 1f;
        }
    }
}