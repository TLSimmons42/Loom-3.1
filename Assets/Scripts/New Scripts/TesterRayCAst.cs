using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TesterRayCAst : MonoBehaviour
{
    public GameObject obj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray testRay = new Ray(transform.position, obj.transform.position);
        int layerMask = ~LayerMask.GetMask("walls");
        Debug.DrawRay(transform.position, obj.transform.position, Color.yellow);
        if(Physics.Raycast(testRay, out hit, 1000f, layerMask))
        {
            if (hit.collider.tag == "red cube")
            {
                Debug.Log("red cube");
            }
            if (hit.collider.tag == "blue cube")
            {
                Debug.Log("blue cube");
            }
        }
        
    }
}
