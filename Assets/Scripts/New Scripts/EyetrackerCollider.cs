using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyetrackerCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "blue cube" || other.gameObject.tag == "B")
        {
            //Analytics.instance.WriteData("looking at blue cube", "", "", gameObject.transform.position.x.ToString(), gameObject.transform.position.y.ToString(), gameObject.transform.position.z.ToString());
            Analytics.instance.writeEvent("looking at blue cube", 2);
            Debug.Log("looking at: " + other.ToString());
        }
        if (other.gameObject.tag == "red cube" || other.gameObject.tag == "R")
        {
            //Analytics.instance.WriteData("looking at red cube", "", "", gameObject.transform.position.x.ToString(), gameObject.transform.position.y.ToString(), gameObject.transform.position.z.ToString());
            Analytics.instance.writeEvent("looking at red cube", 2);
            Debug.Log("looking at: " + other.ToString());
        }
        if (other.gameObject.tag == "invis cube" || other.gameObject.tag == "I")
        {
            //Analytics.instance.WriteData("looking at invis cube", "", "", gameObject.transform.position.x.ToString(), gameObject.transform.position.y.ToString(), gameObject.transform.position.z.ToString());
            Analytics.instance.writeEvent("looking at invis cube", 2);
            Debug.Log("looking at: " + other.ToString());
        }
        if (other.gameObject.tag == "gold cube" || other.gameObject.tag == "G")
        {
            //Analytics.instance.WriteData("looking at gold cube", "", "", gameObject.transform.position.x.ToString(), gameObject.transform.position.y.ToString(), gameObject.transform.position.z.ToString());
            Analytics.instance.writeEvent("looking at gold cube", 2);
            Debug.Log("looking at: " + other.ToString());
        }
        if (other.gameObject.tag == "DropZone")
        {
            //Analytics.instance.WriteData("looking at gold cube", "", "", gameObject.transform.position.x.ToString(), gameObject.transform.position.y.ToString(), gameObject.transform.position.z.ToString());
            Analytics.instance.writeEvent("looking at gold cube", 2);
            Debug.Log("looking at: " + other.ToString());
        }
    }
}
