using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    public bool isDragging;

    private Collider2D _collider;
    private DragController dragController;

    public Vector3 lastPosition;
    public Vector3 originalPos;

    private float movementTime = 15f;
    private Vector3? movementDestination;

    public List<GameObject> objectsWithinRange;
    Transform targetObject;

    public GameObject getClosestObject()
    {
        if (objectsWithinRange.Count > 0)
        {
            GameObject nominee = objectsWithinRange[0];
            foreach (GameObject go in objectsWithinRange)
            {
                if (Vector3.Distance(transform.position, go.transform.position) < Vector3.Distance(transform.position, nominee.transform.position))
                {
                    nominee = go;
                }
            }
            return nominee;
        }
        else
        {
            return null;
        }            
    }

    public TileHolderScript daddy;


    private void Start()
    {
        originalPos = transform.position;
        _collider = GetComponent<Collider2D>();
        dragController = FindObjectOfType<DragController>();
    }

    private void FixedUpdate()
    {
        if (movementDestination.HasValue)
        {
            if (isDragging)
            {
                movementDestination = null;
                return;
            }
            if (transform.position == movementDestination)
            {
                gameObject.layer = Layer.Default;
                movementDestination = null;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position,movementDestination.Value, movementTime * Time.deltaTime);
            }
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        Draggable collidedDraggable = _collider.GetComponent<Draggable>();
        // For stopping overlap between two draggable objects. May be useful.
        if (collidedDraggable != null && dragController.Lastdragged?.gameObject == gameObject)
        {
            ColliderDistance2D collDistance = collision.Distance(_collider);
            Vector3 diff = new Vector3(collDistance.normal.x, collDistance.normal.y) * collDistance.distance;
            transform.position -= diff;
        }



        if (collision.CompareTag("DroppableArea"))
        {
            objectsWithinRange.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("DroppableArea"))
        {
            objectsWithinRange.Remove(collision.gameObject);
            //movementDestination = collision.transform.position;
        }
    }

    public void PlaceDown()
    {
        if (getClosestObject() && !getClosestObject().GetComponent<bPosScript>().tileInfo.occupied)
        {
            movementDestination = getClosestObject().transform.position;
            getClosestObject().GetComponent<bPosScript>().tileInfo.occupied = true;
            getClosestObject().GetComponent<bPosScript>().tileInfo.tileName = gameObject.name;
        }
        else
        {
            //movementDestination = originalPos;
            daddy.UpdateOpened(false); // Reset the creator of this tile
            Destroy(gameObject);
        }
        
    }
}
