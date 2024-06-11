using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private GameObject player;
    private Boulder boulderScript;
    private Node node;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Point");
        node = null;
        boulderScript = player.GetComponent<Boulder>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseDown()
    {
        node = this;
        boulderScript.SelectNode(node);
    }

    public void OnMouseUp()
    {
        node = null;
        boulderScript.DeselectNode();
    }
}
