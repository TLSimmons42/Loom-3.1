using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropzoneScript : MonoBehaviour
{
    public string direction;
    public Vector2Int index;

    public Material normalColor;
    public Material activeColor;
    private MeshRenderer arrowRenderer1;
    private MeshRenderer arrowRenderer2;
    private Color arrowColor;

    public GameObject arrow1;
    public GameObject arrow2;
    
    public string buildWallNumber;


    public void Start()
    {
        arrow1 = GameObject.Find("Arrow1");
        arrow2 = GameObject.Find("Arrow2");
        arrowRenderer1 = arrow1.GetComponent<MeshRenderer>();
        arrowRenderer2 = arrow2.GetComponent<MeshRenderer>();
        arrowColor = arrowRenderer1.material.color;
        arrowColor.a = 0.4f;
        arrowRenderer1.material.color = arrowColor;
        arrowRenderer2.material.color = arrowColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag + " THIS IS THE TAG");
        if (GameManager.instance.playerCount == 1)
        {
            if ((other.tag == "B Drop" || other.tag == "R Drop" || other.tag == "G Drop" || other.tag == "I Drop") && (other.gameObject.GetComponentInParent<Cube>().currentZone != "BuildWall")&& other.gameObject.GetComponentInParent<Cube>().canDrop == false)
            {
                Debug.Log("drop zone script");
                //Analytics.instance.WriteData(other.gameObject.name + "was placed in dropzone", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());
                Analytics.instance.writeEvent(other.gameObject.name + "was placed in dropzone", 0);
                //Analytics.instance.WriteData2(other.gameObject.name + "was placed in dropzone", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());
                //Analytics.instance.WriteData3(other.gameObject.name + "was placed in dropzone", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());

                //MasterBuildWall.instance.dropZoneHit(index, direction, other.gameObject);
                gameObject.GetComponent<MeshRenderer>().material = activeColor;
                other.gameObject.GetComponentInParent<Cube>().canDrop = true;
                other.gameObject.GetComponentInParent<Cube>().currentDropZone = gameObject;
                Vector3 arrowPos = gameObject.GetComponent<MeshRenderer>().transform.position;
                arrowPos.y += 1;
                MoveArrow(arrowPos);

                


            }
        }
        else
        if ((other.tag == "B Drop N" || other.tag == "R Drop N" || other.tag == "G Whole Drop N" || other.tag == "I Drop N") && (other.gameObject.GetComponentInParent<XRGrabNetworkInteractable>().currentZone != "BuildWall") && other.gameObject.GetComponentInParent<XRGrabNetworkInteractable>().canDrop == false)
        {
            //Analytics.instance.WriteData(other.gameObject.name + "was placed in dropzone", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());
            Analytics.instance.writeEvent(other.gameObject.name + "was placed in dropzone", 0);
            //Analytics.instance.WriteData2(other.gameObject.name + "was placed in dropzone", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());
            //Analytics.instance.WriteData3(other.gameObject.name + "was placed in dropzone", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());
            Debug.Log("drop zone script");
            gameObject.GetComponent<MeshRenderer>().material = activeColor;
            other.gameObject.GetComponentInParent<XRGrabNetworkInteractable>().canDrop = true;
            other.gameObject.GetComponentInParent<XRGrabNetworkInteractable>().currentDropZone = gameObject;
            Vector3 arrowPos = gameObject.GetComponent<MeshRenderer>().transform.position;
            arrowPos.y += 1;
            MoveArrow(arrowPos);

     
        }
        else if (other.tag == "LG Drop N" || other.tag == "RG Drop N" && (other.gameObject.GetComponentInParent<GoldCubeHalf>().currentZone != "BuildWall")&& other.gameObject.GetComponentInParent<GoldCubeHalf>().canDrop== false)
        {
            //Analytics.instance.WriteData(other.gameObject.name + "was placed in dropzone", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());
            Analytics.instance.writeEvent(other.gameObject.name + "was placed in dropzone", 0);
            //Analytics.instance.WriteData2(other.gameObject.name + "was placed in dropzone", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());
            //Analytics.instance.WriteData3(other.gameObject.name + "was placed in dropzone", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());
            GameManager.instance.holdingGoldHalf = false;
            Debug.Log("bout to drop a cube");
            gameObject.GetComponent<MeshRenderer>().material = activeColor;
            other.gameObject.GetComponentInParent<GoldCubeHalf>().canDrop = true;
            other.gameObject.GetComponentInParent<GoldCubeHalf>().currentDropZone = gameObject;
            Vector3 arrowPos = gameObject.GetComponent<MeshRenderer>().transform.position;
            arrowPos.y += 1;
            MoveArrow(arrowPos);



        }
        else if (other.tag == "G Drop N" && other.gameObject.GetComponentInParent<XRGrabNetworkInteractable>().canDrop==false)
        {
            if (GameManager.instance.host)
            {
                Analytics.instance.WriteData(other.gameObject.name + "was placed in dropzone", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());
                Analytics.instance.writeEvent(other.gameObject.name + "was placed in dropzone", 0);
                //Analytics.instance.WriteData2(other.gameObject.name + "was placed in dropzone", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());
                //Analytics.instance.WriteData3(other.gameObject.name + "was placed in dropzone", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());
                gameObject.GetComponent<MeshRenderer>().material = activeColor;
                other.gameObject.GetComponentInParent<XRGrabNetworkInteractable>().canDrop = true;
                other.gameObject.GetComponentInParent<XRGrabNetworkInteractable>().currentDropZone = gameObject;
                Vector3 arrowPos = gameObject.GetComponent<MeshRenderer>().transform.position;
                arrowPos.y += 1;
                MoveArrow(arrowPos);

            }
        }
        else
        {
            Debug.Log("nothing happens in this collision");
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(other.tag + " WE LEFT");
        if (GameManager.instance.playerCount == 1)
        {
            if ((other.tag == "B Drop" || other.tag == "R Drop" || other.tag == "G Drop" || other.tag == "I Drop") && (other.gameObject.GetComponentInParent<Cube>().currentZone != "BuildWall"))
            {
                Debug.Log("left the drop zone");
                //Analytics.instance.WriteData(other.gameObject.name + "was placed in dropzone", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());
                Analytics.instance.writeEvent(other.gameObject.name + "was placed in dropzone", 0);
                //Analytics.instance.WriteData2(other.gameObject.name + "was placed in dropzone", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());
                //Analytics.instance.WriteData3(other.gameObject.name + "was placed in dropzone", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());

                //MasterBuildWall.instance.dropZoneHit(index, direction, other.gameObject);
                other.gameObject.GetComponentInParent<Cube>().canDrop = false;
                gameObject.GetComponent<MeshRenderer>().material = normalColor;

                RemoveArrow();

            }
        }
        else
        if ((other.tag == "B Drop N" || other.tag == "R Drop N" || other.tag == "G Whole Drop N" || other.tag == "I Drop N") && (other.gameObject.GetComponentInParent<XRGrabNetworkInteractable>().currentZone != "BuildWall") && other.gameObject.GetComponentInParent<XRGrabNetworkInteractable>().canDrop)
        {
            //Analytics.instance.WriteData(other.gameObject.name + "was placed in dropzone", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());
            Analytics.instance.writeEvent(other.gameObject.name + "was placed in dropzone", 0);
            //Analytics.instance.WriteData2(other.gameObject.name + "was placed in dropzone", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());
            //Analytics.instance.WriteData3(other.gameObject.name + "was placed in dropzone", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());
            Debug.Log("drop zone script");
            other.gameObject.GetComponentInParent<XRGrabNetworkInteractable>().canDrop = false;
            gameObject.GetComponent<MeshRenderer>().material = normalColor;
            RemoveArrow();
        }
        else if (other.tag == "LG Drop N" || other.tag == "RG Drop N" && other.gameObject.GetComponentInParent<GoldCubeHalf>().canDrop)
        {
            //Analytics.instance.WriteData(other.gameObject.name + "was placed in dropzone", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());
            Analytics.instance.writeEvent(other.gameObject.name + "was placed in dropzone", 0);
            //Analytics.instance.WriteData2(other.gameObject.name + "was placed in dropzone", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());
            //Analytics.instance.WriteData3(other.gameObject.name + "was placed in dropzone", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());
            GameManager.instance.holdingGoldHalf = false;
            Debug.Log("bout to drop a cube");
            other.gameObject.GetComponentInParent<GoldCubeHalf>().canDrop = false;
            gameObject.GetComponent<MeshRenderer>().material = normalColor;
            RemoveArrow();

        }
        else if (other.tag == "G Drop N" && other.gameObject.GetComponentInParent<XRGrabNetworkInteractable>().canDrop)
        {
            if (GameManager.instance.host)
            {
                Analytics.instance.WriteData(other.gameObject.name + "was placed in dropzone", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());
                Analytics.instance.writeEvent(other.gameObject.name + "was placed in dropzone", 0);
                //Analytics.instance.WriteData2(other.gameObject.name + "was placed in dropzone", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());
                //Analytics.instance.WriteData3(other.gameObject.name + "was placed in dropzone", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());
                other.gameObject.GetComponentInParent<XRGrabNetworkInteractable>().canDrop = false;
                gameObject.GetComponent<MeshRenderer>().material = normalColor;
                RemoveArrow();
            }
        }
        else
        {
            Debug.Log("nothing happens in this collision");
        }

        
    }
    public void MoveArrow(Vector3 pos)
    {
        if (buildWallNumber == "h")
        {
            arrow1.transform.position = pos;
            arrowRenderer1.enabled = true;
        }
        else if (buildWallNumber == "c")
        {
            arrow2.transform.position = pos;
            arrowRenderer2.enabled = true;

        }
    }
    public void RemoveArrow()
    {
        Debug.Log("remove arrow");
        if (buildWallNumber == "h")
        {
            Debug.Log("remove arrow H");

            arrow1.GetComponent<MeshRenderer>().enabled = false;
        }
        else if (buildWallNumber == "c")
        {
            Debug.Log("remove arrow C");

            arrow2.GetComponent<MeshRenderer>().enabled = false;

        }
    }
}
