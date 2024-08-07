using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public Transform cursorImage;
    private float magnitudeOfMouseDistance;
    private float cursorMovementRange;
    private float maxDistanceFromCenter = 50f;

    private float x;
    private float y;

    private Vector2 worldPosition;
    private bool isPressed = false;
    private Vector2 mouseDirection;
    public Vector2 MouseDirection
    {
        get { return mouseDirection; }
        set
        {
            mouseDirection = value;
        }
    }

    private void Update()
    {
        if (!isPressed)
        {
            transform.position = Vector2.Lerp(transform.position, cursorImage.position, 10f * Time.deltaTime);
            ResetJoyStickPos();
        }
    }

    void Start()
    {
        worldPosition = new Vector2(transform.position.x, transform.position.y); // 1920 * 1080 and at center it is 960 * 540
    }

    public void OnDrag(PointerEventData pointerEventData)
    {
        isPressed = true;
        // pointerEventData.position - position of the drag pointer (at start 0,0) with respect to world position (1920 * 1080)
        // mouseDirection - position of the mouse with respect to pointerEventData.position
        if (isPressed)
        {
            MouseDirection = pointerEventData.position - worldPosition;
            magnitudeOfMouseDistance = MouseDirection.magnitude;
            cursorMovementRange = Mathf.InverseLerp(0, maxDistanceFromCenter, magnitudeOfMouseDistance);
            transform.localPosition = MouseDirection.normalized * cursorMovementRange * maxDistanceFromCenter;
            SetInputs();
        }
    }

    public static Vector3 input;

    public void SetInputs()
    {
        x = transform.localPosition.x;
        y = transform.localPosition.y;

        if (x > 0)
        {
            input.x = Mathf.InverseLerp(0, maxDistanceFromCenter, x + 1);
        }
        if (x < 0)
        {
            input.x = -Mathf.InverseLerp(0, -maxDistanceFromCenter, x - 1);
        }
        if (y > 0)
        {
            input.y = Mathf.InverseLerp(0, maxDistanceFromCenter, y + 1);
        }
        if (y < 0)
        {
            input.y = -Mathf.InverseLerp(0, -maxDistanceFromCenter, y - 1);
        }
    }

    public void ResetJoyStickPos()
    {
        input = new Vector2(x, y) * 0f;
        transform.localPosition = Vector2.zero;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isPressed = false;
    }
}