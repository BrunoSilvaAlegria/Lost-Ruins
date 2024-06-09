using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopePoint : MonoBehaviour
{
    private GameObject player;
    private RopePointDetection pointDetection;
    private RopePoint ropePoint;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        pointDetection = player.GetComponent<RopePointDetection>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseDown()
    {
        ropePoint = this;
        pointDetection.SelectNode(ropePoint);
    }

    public void OnMouseUp()
    {
        ropePoint = null;
        pointDetection.DeselectNode();
    }
}
