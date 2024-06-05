using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Boulder :MonoBehaviour
{
    Rigidbody2D rb;
    private LineRenderer lineRend;
    private DistanceJoint2D distJoint;
    private Node selectedNode;

    public static Boulder instance;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 
        lineRend = GetComponent<LineRenderer>();
        distJoint = GetComponent<DistanceJoint2D>();

        lineRend.enabled = false;
        distJoint.enabled = false;
        selectedNode = null;
    }

    // Update is called once per frame
    void Update()
    {
        if(selectedNode == null)
        {
            lineRend.enabled = false;
            distJoint.enabled = false;

            return;
        }

        lineRend.enabled = true;
        distJoint.enabled = true;

        distJoint.connectedBody = selectedNode.GetComponent<Rigidbody2D>();
    }

    public void SelectNode(Node node)
    {
        selectedNode = node;
    }

    public void DeselectNode()
    {
        selectedNode = null;
    }


}
