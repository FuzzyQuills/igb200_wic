using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentHeadScript : MonoBehaviour
{
    public int identity = -1;
    public Color thisColor;

    [HideInInspector]
    public LineRenderer lr;
    private VentObject father; // The parent of this object
    public VentObject ventoObjecto //creative.
    {
        set
        {
            father = value;
        }

    }

    public bool connected = false;

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.material.color = thisColor;
        lr.SetPosition(0, transform.position - Vector3.forward);
    }


    private void OnMouseDown()
    {
        StartCoroutine(Drag());
        //Debug.Log($"Down on vent {identity}");
        //selected = true;
    }

    private void OnMouseUp()
    {
        //Debug.Log($"Up on vent {identity}");
        //selected = false;
    }

    void CleanPaint()
    {
        lr.positionCount = 0;
        foreach (VentObject vo in GameObject.FindObjectsOfType<VentObject>())
        {
            if (vo.painted && vo.paintID == identity)
            {
                vo.painted = false;
                vo.paintID = -1;
            }
        }
        foreach (VentHeadScript vhs in GameObject.FindObjectsOfType<VentHeadScript>())
        {
            if (vhs.identity == identity)
            {
                vhs.connected = false;
                vhs.lr.positionCount = 0;
            }
        }
    }

    IEnumerator Drag()
    {
        Debug.Log("Drag Beginning");
        CleanPaint();
        Vector2 currentPos = father.coord;
        int lineCount = 0;
        lr.positionCount = 1;
        lr.SetPosition(0, transform.position - (Vector3.forward * transform.position.z));

        void Paint(RaycastHit2D hit)
        {
            lineCount++;
            lr.positionCount = lineCount + 1;
            lr.SetPosition(lineCount, new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y, transform.position.z));
        }

        while (Input.touchCount > 0)
        {
            Vector2 fingerPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            RaycastHit2D hit = Physics2D.Raycast(fingerPos, Vector2.zero);
            if (hit.collider != null)
            {
                //Debug.Log(hit.collider.name);
                if (hit.collider.GetComponent<VentObject>() && hit.collider.GetComponent<VentObject>().painted == false)
                {
                    //Debug.Log($"a - {father.coord} TO {hit.collider.GetComponent<VentObject>().coord}");
                    if (VentGameScript.near(currentPos, hit.collider.GetComponent<VentObject>().coord))
                    {
                        //Debug.Log("b");
                        Paint(hit);
                        currentPos = hit.collider.GetComponent<VentObject>().coord;
                        hit.collider.GetComponent<VentObject>().painted = true;
                        hit.collider.GetComponent<VentObject>().paintID = identity;
                    }
                }
                if (hit.collider.tag == "VentHead" && hit.collider.transform != this.transform)
                {
                    if (VentGameScript.near(currentPos, hit.collider.GetComponent<VentHeadScript>().father.coord))
                    {
                        if (hit.collider.GetComponent<VentHeadScript>().identity == identity)
                        {
                            Paint(hit);
                            connected = true;
                            hit.collider.GetComponent<VentHeadScript>().connected = true;
                            Debug.Log("End Detected. Disconnecting");
                            break;
                        }
                    }
                }
            }

            yield return null;
        }

        if (!connected)
        {
            CleanPaint();
            Debug.Log("Connection failed");
        }

        yield return null;
    }
}
