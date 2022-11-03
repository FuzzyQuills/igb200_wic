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
        // Note added by Jean-Luc Mackail: Basically forces a single instance. bruh
        DragController[] controller = FindObjectsOfType<DragController>();
        if (controller.Length > 1)
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        // If the mouse button is released and/or the player's finger leaves the touchscreen, drop the active tile.
        if (isDragActive && ((Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)))
        {
            Drop();
            return;
        }
        // Handle touchscreen input, if active. 
        if (Input.touchCount > 0)
        {
            screenPosition = Input.GetTouch(0).position;
        }
        else
        {
            // Bail early; nothing to do.
            return;
        }

        // Update tile dragging parameters. 
        worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        if (isDragActive)
        {
            Drag();
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);
            //Debug.Log(hit.collider.gameObject.name);
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

    // Initiates dragging a tile.
    void InitDrag()
    {
        FindObjectOfType<AudioManager>().Play("PickupTile");
        lastDragged.lastPosition = lastDragged.transform.position;
        lastDragged.EmptyTile();
        UpdateDragStatus(true);

    }

    // Updates a dragged tile. 
    void Drag()
    {
        lastDragged.transform.position = new Vector2(worldPosition.x, worldPosition.y);
    }

    // Drops a tile when all input events are released. 
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
