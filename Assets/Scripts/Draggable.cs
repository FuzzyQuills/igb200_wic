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

    //private float movementTime = 15f;
    private Vector3? movementDestination;

    public List<GameObject> objectsWithinRange;
    Transform targetObject;

    public IHolder daddy; // for the room tiles
    TileInfo occupado = null; // The tile this draggable is on

    int flatPrice = 20; // Flat price per tile is 20k
    public int price = 0; // Price of this draggable object.


    public Vector2[] AdditionalPositions;
    public Vector2[] GoldPositions;
    List<TileInfo> AdditionalOccupados = new List<TileInfo>();

    public int nodeStrength = 0;

    public TileInfo standingTile;

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
                // Enable the comment if you want the smooth movmement back. It makes small lerps jiggle though
                transform.position = movementDestination.Value; //Vector3.Lerp(transform.position,movementDestination.Value, movementTime * Time.deltaTime);
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
                // This is where the cost begins. Each tile costs twice as much if it is not shadowed (it overhangs)
                if (target.GetComponent<bPosScript>().shadowed)
                {
                    price = -flatPrice;
                }
                else
                {
                    price = -flatPrice * 2;
                }

                int vibecheck = 0;
                for (int i = 0; i < AdditionalPositions.Length; i++)
                {
                    foreach (bPosScript t in GameObject.FindObjectsOfType<bPosScript>())
                    {
                        if (t.tileInfo.coordinates == target.GetComponent<bPosScript>().tileInfo.coordinates + AdditionalPositions[i])
                        {
                            if (!t.tileInfo.tiled)
                            {
                                //Debug.Log("A");
                                vibecheck++;
                                // Cost added per tile
                                if (t.shadowed)
                                {
                                    price -= flatPrice;
                                }
                                else
                                {
                                    price -= flatPrice * 2;
                                }
                            }
                        }
                    }
                }
                int goldCheck = 0;
                for (int i = 0; i < GoldPositions.Length; i++)
                {
                    foreach (bPosScript t in GameObject.FindObjectsOfType<bPosScript>())
                    {
                        if (t.tileInfo.coordinates == target.GetComponent<bPosScript>().tileInfo.coordinates + GoldPositions[i])
                        {
                            if (!t.tileInfo.tiled)
                            {
                                //Debug.Log("A");
                                goldCheck++;
                                // Cost added per tile
                                price -= flatPrice;
                            }
                        }
                    }
                }
                if (vibecheck != AdditionalPositions.Length || goldCheck != GoldPositions.Length)
                {
                    CleanDestroy();
                }
                else
                {
                    FindObjectOfType<AudioManager>().Play("PutdownTile");
                    movementDestination = target.transform.position - Vector3.forward;
                    target.GetComponent<bPosScript>().tileInfo.tiled = true;
                    target.GetComponent<bPosScript>().tileInfo.tileName = gameObject.name;
                    occupado = target.GetComponent<bPosScript>().tileInfo;

                    for (int i = 0; i < AdditionalPositions.Length; i++)
                    {
                        foreach (bPosScript t in GameObject.FindObjectsOfType<bPosScript>())
                        {
                            if (t.tileInfo.coordinates == target.GetComponent<bPosScript>().tileInfo.coordinates + AdditionalPositions[i])
                            {
                                t.tileInfo.tiled = true;
                                t.tileInfo.tileName = "Fill";
                                AdditionalOccupados.Add(t.tileInfo);
                            }
                        }
                    }
                    // Golds
                    for (int i = 0; i < GoldPositions.Length; i++)
                    {
                        foreach (bPosScript t in GameObject.FindObjectsOfType<bPosScript>())
                        {
                            if (t.tileInfo.coordinates == target.GetComponent<bPosScript>().tileInfo.coordinates + GoldPositions[i])
                            {
                                t.tileInfo.tiled = true;
                                t.tileInfo.tileName = "FillGold";
                                AdditionalOccupados.Add(t.tileInfo);
                            }
                        }
                    }
                }
                fixNodes();
                standingTile = target.GetComponent<bPosScript>().tileInfo;
            }
            else if (tag == "Node" && !target.GetComponent<bPosScript>().tileInfo.noded && target.GetComponent<bPosScript>().tileInfo.tiled) // If this is a node and the target position isnt 'noded'
            {
                movementDestination = target.transform.position - (Vector3.forward * 2);
                target.GetComponent<bPosScript>().tileInfo.noded = true;
                target.GetComponent<bPosScript>().tileInfo.nodeName = gameObject.name;
                occupado = target.GetComponent<bPosScript>().tileInfo;

                GetNodeStrength();

                standingTile = target.GetComponent<bPosScript>().tileInfo;
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
                    ClearNodeStrength();
                    occupado.noded = false;
                    occupado.nodeName = null;
                    nodeStrength = 0;
                    break;
                default:
                    break;
            }
        }
        if (AdditionalOccupados.Count > 0)
        {
            switch (tag) // Kinda redundant. For future work
            {
                case "Tile":
                    for (int i = AdditionalOccupados.Count - 1; i >= 0; i--)
                    {
                        //Debug.Log("B");
                        AdditionalOccupados[i].tiled = false;
                        AdditionalOccupados[i].tileName = null;
                        AdditionalOccupados.RemoveAt(i);
                    }
                    break;
                case "Node":

                    break;
                default:
                    break;
            }

        }
        standingTile = null;
    }

    /// <summary>
    /// Destroy the draggable and update its Iholder if possible.
    /// </summary>
    void CleanDestroy(bool Bool = false)
    {
        //Debug.Log("a");        

        daddy?.UpdateOpened(Bool);




        EmptyTile();


        Destroy(gameObject);
    }

    public static void fixNodes()
    {
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");
        for (int i = 0; i < nodes.Length; i++)
        {
            if (!nodes[i].GetComponent<Draggable>().standingTile.tiled)
            {
                nodes[i].GetComponent<Draggable>().CleanDestroy();
                continue;
            }
            //nodes[i].GetComponent<Draggable>().GetNodeStrength();
        }
    }

    public void GetNodeStrength()
    {
        nodeStrength = 0;
        for (int i = 0; i < AdditionalPositions.Length; i++)
        {
            foreach (bPosScript t in GameObject.FindObjectsOfType<bPosScript>())
            {
                if (t.tileInfo.coordinates == getClosestObject().GetComponent<bPosScript>().tileInfo.coordinates + AdditionalPositions[i] && t.tileInfo.tiled == true)
                {
                    if (!t.tileInfo.nodeInfluenced)
                    {
                        t.tileInfo.nodeInfluenced = true;
                        nodeStrength++;
                    }

                }
            }
        }
    }
    /// <summary>
    /// Should be triggered on pickup / deletion
    /// </summary>
    public void ClearNodeStrength()
    {
        if (nodeStrength > 0)
        {
            //for (int i = 0; i < AdditionalPositions.Length; i++)
            //{
            //    foreach (bPosScript t in GameObject.FindObjectsOfType<bPosScript>())
            //    {
            //        if (t.tileInfo.coordinates == getClosestObject().GetComponent<bPosScript>().tileInfo.coordinates + AdditionalPositions[i] && t.tileInfo.tiled == true)
            //        {
            //            t.tileInfo.nodeInfluenced = false;
            //        }
            //    }
            //}
            foreach (bPosScript t in GameObject.FindObjectsOfType<bPosScript>())
            {
                t.tileInfo.nodeInfluenced = false;
            }
            foreach (Draggable d in GameObject.FindObjectsOfType<Draggable>())
            {
                if (d != this)
                {
                    if (d.transform.tag == "Node")
                    {
                        d.GetNodeStrength();
                    }
                }
            }

        }
    }
}
