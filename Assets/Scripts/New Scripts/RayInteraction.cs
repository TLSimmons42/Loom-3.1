using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RayInteraction : XRRayInteractor
{


    //public XRRayInteractor rightRay;
    //public XRRayInteractor leftRay;

    public LineRenderer rightLineRenderer;
    public GameObject right;

    public Vector3[] rightRayPoints;
    public Vector3[] leftRayPoints;

    public int test = 10;



    // Start is called before the first frame update
    void Start()
    {
        rightLineRenderer = right.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    int noPoins;
    public void ResetRayObjectPos(GameObject obj)
    {
        //bool rightTemp = rightRay.GetLinePoints(ref rightRayPoints, out int numPoints);
        //bool leftTemp = rightRay.GetLinePoints(ref leftRayPoints, out int numPoints2);

        rightLineRenderer.GetPositions(rightRayPoints);

        obj.transform.position = rightRayPoints[rightRayPoints.Length - 1];
        Debug.Log("moving the object");

        //if (rightTemp)
        //{
        //    obj.transform.position = rightRayPoints[rightRayPoints.Length - 1];

        //    //Debug.Log(rightRayPoints[rightRayPoints.Length-1]);
        //    //Debug.Log(rightRayPoints.Length);

        //}
        //if (leftTemp)
        //{
        //    obj.transform.position = leftRayPoints[leftRayPoints.Length - 1];

        //    Debug.Log(leftRayPoints[leftRayPoints.Length - 1]);
        //    Debug.Log(leftRayPoints.Length);

        //}
    }

    

}
