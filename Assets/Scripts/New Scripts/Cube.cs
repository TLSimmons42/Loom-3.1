using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;


public class Cube : XRGrabInteractable
{
    //public GameManager gameManager;
    private Rigidbody rb;
    private BoxCollider collider;
    public Vector2Int index;
    

    public Vector3 playWallTargetPos, buildWallTargetPos;
    public Quaternion buildWallTargetRotation;

    public string currentZone;
    public int playersHoldingCube = 0;
    public Vector3 goldCubeHoldPos;

    public string playWallZone = "PlayWall";
    public string BuildWallZone = "BuildWall";
    public bool isHeld = false;
    public bool canDrop = false;
    public string NoZone = "No Zone";
    public string holdGold = "Hold Gold";

    private float playZoneFallSpeed = 3f;

    public GameObject rightRay;
    public GameObject leftRay;
    public GameObject currentDropZone;

    public LineRenderer rightLineRenderer;
    public LineRenderer leftLineRenderer;

    private Vector3[] rightRayPoints = new Vector3[2];
    public Vector3[] leftRayPoints;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<BoxCollider>();
        rightRay = GameObject.FindGameObjectWithTag("right ray");
        rightLineRenderer = rightRay.GetComponent<LineRenderer>();
        //StartCoroutine(CanDropCubeTimer());

        //canDrop = true;

    }
    void Update()
    {
        if (currentZone == playWallZone)
        {
            MoveCubePlayWall();
        }

        if (currentZone == BuildWallZone)
        {
            MoveCubeBuildWall();
        }
        if(currentZone == NoZone)
        {
            rightLineRenderer.GetPositions(rightRayPoints);
            gameObject.transform.position = rightRayPoints[rightRayPoints.Length - 1];
        }
        
    }
    public void SetZoneToPlay()
    {
        currentZone = playWallZone;
    }
    public void SetZoneToBuild()
    {
        Debug.Log("Changing to build zone...");
        currentZone = BuildWallZone;
    }

    public void MoveCubePlayWall() 
    {
        //if(currentZone == playWallZone && transform.position == gameManager.PlaywallDropPoints[0].transform.position)
        
        transform.position = Vector3.MoveTowards(transform.position, playWallTargetPos, Time.deltaTime * playZoneFallSpeed);
        transform.rotation = buildWallTargetRotation;
    }

    public void MoveCubeBuildWall()
    {
        //Debug.Log("moving the block on the BuildWall");
        transform.position = Vector3.MoveTowards(transform.position, buildWallTargetPos, Time.deltaTime * playZoneFallSpeed);
        transform.rotation = buildWallTargetRotation;
    }

    public void PlayerGrab()
    {
        //Analytics.instance.WriteData(gameObject.name +" was picked up", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());
        Analytics.instance.writeEvent(gameObject.name + " was picked up", 3);
        //Analytics.instance.WriteData2(gameObject.name + " was picked up", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());
        //Analytics.instance.WriteData3(gameObject.name + " was picked up", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());
        AudioManager.instance.grabCube.Play();

        currentZone = NoZone;
        collider.isTrigger = true;

        //rb.detectCollisions = false;
        //gameObject.layer = 2;
        //isHeld = true;
        //Debug.Log("GRABBED cube");
    }
    public void removeCube(int x, int y)
    {
        MasterBuildWall.instance.masterBuildArray[x, y] = null;
        Analytics.instance.writeEvent("Cube removal from Build wall", 3);
    }


    protected override void OnSelectEntered(XRBaseInteractor interactor)
    {
        base.OnSelectEntered(interactor);

        if(currentZone == playWallZone)
        {
            //canDrop = true;
        }
        //  this will grab all colors in single player mode
        if (GameManager.instance.playerCount == 1)
        {

            if(currentZone == BuildWallZone)
            {
                removeCube(index.x, index.y);
                //Destroy(gameObject);
            }

            PlayerGrab();
        }
        else if (GameManager.instance.playerCount > 1)   
        {
            Debug.Log("did nothing in the cube script");
            
        }

    }
    protected override void OnSelectExited(XRBaseInteractor interactor)
    {

        collider.isTrigger = false;

        if (canDrop)
        {
            Debug.Log("We ARE dropping");
            currentDropZone.GetComponent<MeshRenderer>().material = currentDropZone.GetComponent<DropzoneScript>().normalColor;
            FindObjectOfType<DropzoneScript>().RemoveArrow();
            MasterBuildWall.instance.dropZoneHit(currentDropZone.GetComponent<DropzoneScript>().index, currentDropZone.GetComponent<DropzoneScript>().direction, gameObject);
        }
        else
        {
            if (GameManager.instance.playerCount == 2)
            {
                //Debug.Log("destory this cube");
                //PhotonNetwork.Destroy(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
                Debug.Log("DESTORY THIS CUBE");
            }
        }

        //if (currentZone == BuildWallZone)
        //{
        //    Debug.Log("In the build wall");
        //}
        //else
        //{
        //    if (GameManager.instance.playerCount == 2)
        //    {
        //        //Debug.Log("destory this cube");
        //        //PhotonNetwork.Destroy(this.gameObject);
        //    }
        //    else
        //    {
        //        Destroy(this.gameObject);
        //        Debug.Log("DESTORY THIS CUBE");
        //    }
        //}
        //    }
        //}
    }

    IEnumerator CanDropCubeTimer()
    {
        yield return new WaitForSeconds(1.5f);
        canDrop = true;
    }




    private void OnTriggerEnter(Collider other)
    {

        if(other.tag == "DropZone")
        {
           // currentZone = BuildWallZone;
        }
        if (other.tag == "cube despawner")
        {
            
                Destroy(this.gameObject);

           
        }
    }


}
