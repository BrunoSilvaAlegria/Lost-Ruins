using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappler : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private DistanceJoint2D _distanceJoint;

    private RopePoint nearestPoint;
    private List<RopePoint> points;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        _distanceJoint.enabled = false;
        _lineRenderer.enabled = false;
        points = new List<RopePoint>(FindObjectsOfType<RopePoint>());
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0)) 
        { 
            FindNearestPoint();
            if (nearestPoint != null)
            {
                Vector2 mousePos = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);
                _lineRenderer.SetPosition(0, mousePos);
                _lineRenderer.SetPosition(1, transform.position);
                _distanceJoint.connectedAnchor = mousePos;
                _distanceJoint.enabled = true;
                _lineRenderer.enabled = true;
            }
        }

        else if(Input.GetKeyUp(KeyCode.Mouse0))
        {
            _distanceJoint.enabled = false;
            _lineRenderer.enabled = false;
        }

        if(_distanceJoint.enabled)
        {
            _lineRenderer.SetPosition(1, transform.position);
        }

    }

    private void FindNearestPoint()
    {
        float closestDistance = Mathf.Infinity;
        nearestPoint = null;
        Vector2 currentPosition = transform.position;

        foreach (RopePoint point in points)
        {
            float distance = Vector2.Distance(currentPosition, point.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestPoint = point;
            }
        }
    }
}
