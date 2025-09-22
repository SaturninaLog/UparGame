using UnityEngine;
using System;

public class SwipeDetector : MonoBehaviour
{
    public float minSwipeDistance = 50f; // en píxeles
    public float maxSwipeTime = 1f;      // en segundos

    private Vector2 startPos;
    private float startTime;
    private bool isDragging = false;

    public event Action OnSwipeLeft;
    public event Action OnSwipeRight;
    public event Action OnSwipeUp;
    public event Action OnSwipeDown;

    void Update()
    {
        // Touch
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                isDragging = true;
                startPos = t.position;
                startTime = Time.time;
            }
            else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
            {
                if (!isDragging) return;
                Vector2 endPos = t.position;
                float dt = Time.time - startTime;
                HandleSwipe(startPos, endPos, dt);
                isDragging = false;
            }
        }
        else
        {
            // Mouse (editor/testing)
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                startPos = Input.mousePosition;
                startTime = Time.time;
            }
            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                Vector2 endPos = (Vector2)Input.mousePosition;
                float dt = Time.time - startTime;
                HandleSwipe(startPos, endPos, dt);
                isDragging = false;
            }
        }
    }

    private void HandleSwipe(Vector2 a, Vector2 b, float dt)
    {
        Vector2 delta = b - a;
        if (dt > maxSwipeTime) return;
        if (delta.magnitude < minSwipeDistance) return;

        Vector2 abs = new Vector2(Mathf.Abs(delta.x), Mathf.Abs(delta.y));
        if (abs.x > abs.y)
        {
            if (delta.x > 0) OnSwipeRight?.Invoke();
            else OnSwipeLeft?.Invoke();
        }
        else
        {
            if (delta.y > 0) OnSwipeUp?.Invoke();
            else OnSwipeDown?.Invoke();
        }
    }
}
