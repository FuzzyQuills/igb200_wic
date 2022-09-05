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

    public IHolder daddy; // for the room tiles
    TileInfo occupado = null; // The tile this draggable is on

    /// <summary>
    /// For getting the closest transform to this gameobject
    /// </summary>
    /// <returns></returns>
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
        if (getClosestObject()) // If there is a coordinate within this draggable's collider
        {
            GameObject target = getClosestObject();
            if (tag == "Tile" && !target.GetComponent<bPosScript>().tileInfo.tiled) // If this draggable is a tile and the target position isn't occupied
            {
                movementDestination = target.transform.position;
                target.GetComponent<bPosScript>().tileInfo.tiled = true;
                target.GetComponent<bPosScript>().tileInfo.tileName = gameObject.name;
                occupado = target.GetComponent<bPosScript>().tileInfo;
            }
            else if (tag == "Node" && !target.GetComponent<bPosScript>().tileInfo.noded) // If this is a node and the target position isnt 'noded'
            {
                movementDestination = target.transform.position;
                target.GetComponent<bPosScript>().tileInfo.noded = true;
                target.GetComponent<bPosScript>().tileInfo.nodeName = gameObject.name;
                occupado = target.GetComponent<bPosScript>().tileInfo;
            }
            else // Destroy object when conditions arent met
            {
                CleanDestroy();
            }
        }
        else
        {
            CleanDestroy();
        }
        
    }

    /// <summary>
    /// If this draggable is on a tile, then clear that tile's info about this object
    /// </summary>
    public void EmptyTile()
    {
        if (occupado != null)
        {
            switch (tag)
            {
                case "Tile":
                    occupado.tiled = false;
                    occupado.tileName = null;
                    break;
                case "Node":
                    occupado.noded = false;
                    occupado.nodeName = null;
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Destroy the draggable and update its Iholder if possible.
    /// </summary>
    void CleanDestroy(bool Bool = false)
    {
        daddy?.UpdateOpened(Bool);
        EmptyTile();
        
        
        Destroy(gameObject);
    }
}
