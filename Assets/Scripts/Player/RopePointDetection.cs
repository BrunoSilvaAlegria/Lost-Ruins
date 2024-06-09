using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RopePointDetection : MonoBehaviour
{
    private Rigidbody2D rb;
    private LineRenderer lineRend;
    private DistanceJoint2D distJoint;
    private RopePoint selectedPoint;

    public static RopePointDetection instance;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 
        lineRend = GetComponent<LineRenderer>();
        distJoint = GetComponent<DistanceJoint2D>();

        lineRend.enabled = false;
        distJoint.enabled = false;
        selectedPoint = null;
    }

    // Update is called once per frame
    void Update()
    {
        if(selectedPoint == null)
        {
            lineRend.enabled = false;
            distJoint.enabled = false;

            return;
        }

        lineRend.enabled = true;
        distJoint.enabled = true;

        distJoint.connectedBody = selectedPoint.GetComponent<Rigidbody2D>();
    }

    public void SelectNode(RopePoint node)
    {
        selectedPoint = node;
    }

    public void DeselectNode()
    {
        selectedPoint = null;
    }


}
