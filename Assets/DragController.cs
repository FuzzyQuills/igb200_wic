using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragController : MonoBehaviour
{
    public Draggable Lastdragged => lastDragged;

    private bool isDragActive;
    Vector2 screenPosition;
    Vector3 worldPosition;
    private Draggable lastDragged;

    private void Awake()
    {
        // Checks for duplicate scripts like this and destroys them.
        DragController[] controller = FindObjectsOfType<DragController>();
        if (controller.Length > 1)
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        if (isDragActive && (Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)))
        {
            Drop();
            return;
        }
        // Mouse
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            screenPosition = new Vector2(mousePos.x,mousePos.y);
        }
        // Touchscreen
        if (Input.touchCount > 0)
        {
            screenPosition = Input.GetTouch(0).position;
        }
        else
        {
            return; // stops all proceeding code from working if no touch is detected. efficient.
        }

        worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        if (isDragActive)
        {
            Drag();
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);
            if (hit.collider != null)
            {
                Draggable draggable = hit.transform.gameObject.GetComponent<Draggable>();
                if (draggable != null)
                {
                    lastDragged = draggable;
                    InitDrag();
                }
            }
        }
    }
    void InitDrag()
    {
        lastDragged.lastPosition = lastDragged.transform.position;
        UpdateDragStatus(true);
    }
    void Drag()
    {
        lastDragged.transform.position = new Vector2(worldPosition.x,worldPosition.y);
    }
    void Drop()
    {
        UpdateDragStatus(false);
        lastDragged.PlaceDown();
    }

    void UpdateDragStatus(bool IsDragging)
    {
        isDragActive = lastDragged.isDragging = IsDragging;
        //lastDragged.gameObject.layer = IsDragging ? Layer.Dragging : Layer.Default; // Change layer based on IsDragging status
    }
}
