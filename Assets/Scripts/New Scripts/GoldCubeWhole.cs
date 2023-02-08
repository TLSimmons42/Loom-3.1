using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;
using Photon.Realtime;

public class GoldCubeWhole : XRSimpleInteractable
{
    public string currentZone;
    public string playWallZone = "PlayWall";
    public string BuildWallZone = "BuildWall";
    public string NoZone = "No Zone";
    public string holdGold = "Hold Gold";
    public string deleteZone = "delete zone";
    public string currentBuildWall;

    public bool isHeld = false;
    public bool isHeldByBoth = false;
    public bool canDrop = false;
    public bool updateBuildWallState = false;

    private BoxCollider collider;

    public Vector2Int index;

    public int playersHoldingCube = 0;
    public int mirroredBuildWallCubeID;
    private float playZoneFallSpeed = 3f;

    public Vector3 playWallTargetPos, buildWallTargetPos;
    public Quaternion buildWallTargetRotation;

    public PhotonView PV;

    public GameObject rightRay;
    public GameObject leftRay;
    public GameObject mirroredBuildWallCube;


    public LineRenderer rightLineRenderer;
    public LineRenderer leftLineRenderer;
    private Vector3[] rightRayPoints = new Vector3[2];



    void Start()
    {
        StartCoroutine(CanDropCubeTimer());
        PV = GetComponent<PhotonView>();
        collider = GetComponent<BoxCollider>();
        rightRay = GameObject.FindGameObjectWithTag("right ray");
        rightLineRenderer = rightRay.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentZone == deleteZone)
        {
            if (PV.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
        if (updateBuildWallState)
        {
            PV.RPC("ChangeStateBuildWall", RpcTarget.AllBuffered);
            updateBuildWallState = false;
        }

        if (currentZone == playWallZone)
        {
            MoveCubePlayWall();
        }
        if (currentZone == BuildWallZone)
        {
            if (GameManager.instance.host)
            {
                MoveCubeBuildWall();
            }
        }
        if(currentZone == NoZone)
        {
            if (PV.IsMine)
            {
                rightLineRenderer.GetPositions(rightRayPoints);
                gameObject.transform.position = rightRayPoints[rightRayPoints.Length - 1];
            }
            // Added for removing cube.
        }
    }

    public void PlayerGrab()
    {
        PV.RequestOwnership();
        AudioManager.instance.grabCube.Play();

        currentZone = NoZone;
        Debug.Log("New zone: " + currentZone);
        PV.RPC("ChangeStateNoZone", RpcTarget.AllBuffered);
        collider.isTrigger = true;
    }

    public void MoveCubePlayWall()
    {
        //if(currentZone == playWallZone && transform.position == gameManager.PlaywallDropPoints[0].transform.position)

        transform.position = Vector3.MoveTowards(transform.position, playWallTargetPos, Time.deltaTime * playZoneFallSpeed);
        //transform.rotation = buildWallTargetRotation;
    }
    public void MoveCubeBuildWall()
    {
        //Debug.Log("moving the block on the BuildWall");
        transform.position = Vector3.MoveTowards(transform.position, buildWallTargetPos, Time.deltaTime * playZoneFallSpeed);
        transform.rotation = buildWallTargetRotation;
    }
    IEnumerator CanDropCubeTimer()
    {
        yield return new WaitForSeconds(1.5f);
        canDrop = true;
    }

    [PunRPC]
    public void IncreaseGoldCubeNetworkVar()
    {
        Debug.Log("adding a person to the gold box");
        playersHoldingCube++;
    }

    [PunRPC]
    public void SpawnCubeHalves(XRBaseInteractor interactor)
    {
        if (interactor.transform.parent.parent.gameObject.tag == "P1")
        {
            PhotonNetwork.Instantiate("Network Gold Left Half", transform.position, Quaternion.identity);
            PhotonNetwork.Destroy(gameObject);
        }
        else if(interactor.transform.parent.parent.gameObject.tag == "P2")
        {
            PhotonNetwork.Instantiate("Network Gold Right Half", transform.position, Quaternion.identity);
            PhotonNetwork.Destroy(gameObject);
        }
    }
    
    
    [PunRPC]
    public void ChangeStateBuildWall()
    {
        //Debug.Log("current zone is now buildwall");
        currentZone = BuildWallZone;
    }
    [PunRPC]
    public void ChangeStateToHold()
    {
        currentZone = holdGold;
    }
    [PunRPC]
    public void ChangeStateNoZone()
    {
        currentZone = NoZone;
    }
    [PunRPC]
    public void ChangeStateToDelete()
    {
        currentZone = deleteZone;
    }


    protected override void OnSelectEntered(XRBaseInteractor interactor)
    {
        if (GameManager.instance.holdingGoldHalf)
        {
            Debug.Log("cant pick anything up");
        }
        else
        if (currentZone == BuildWallZone)
        {
            collider.isTrigger = true;
            PlayerGrab();
            PV.RPC("removeCube", RpcTarget.AllBuffered, index.x, index.y);

        }
        else
        {
            PV.RPC("IncreaseGoldCubeNetworkVar", RpcTarget.AllBuffered);
            if (playersHoldingCube == 2)
            {
                
                if (interactor.transform.parent.parent.gameObject.tag == "P1")
                {
                    PV.RequestOwnership();
                    Debug.Log("spawning left half");
                    //gameObject.transform.parent.GetComponent<GoldParent>().leftHalf.SetActive(true);
                    //gameObject.transform.parent.GetComponent<GoldParent>().rightHalf.SetActive(true);
                    //gameObject.SetActive(false);
                    GameObject cube = PhotonNetwork.Instantiate("Network Gold Left Half", transform.position, Quaternion.identity);
                    GameObject cube1 = PhotonNetwork.Instantiate("Network Gold Right Half", transform.position, Quaternion.identity);
                    //Analytics.instance.writeEvent("Player 1 grabs the gold cube", 3);
                    Analytics.instance.writeEvent("Player 1 finishes the gold cube", 3);
                    PhotonNetwork.Destroy(this.gameObject);
                    //PV.RPC("DestoryThisObjectOnNetwork", RpcTarget.AllBuffered);
                }
                else if (interactor.transform.parent.parent.gameObject.tag == "P2")
                {
                    //PV.RequestOwnership();

                    Debug.Log("spawning right half");
                    //gameObject.transform.parent.GetComponent<GoldParent>().leftHalf.SetActive(true);
                    //gameObject.transform.parent.GetComponent<GoldParent>().rightHalf.SetActive(true);
                    //gameObject.SetActive(false);

                    GameObject cube = PhotonNetwork.Instantiate("Network Gold Left Half", transform.position, Quaternion.identity);
                    GameObject cube1 = PhotonNetwork.Instantiate("Network Gold Right Half", transform.position, Quaternion.identity);
                    //Analytics.instance.writeEvent("Player 2 grabs the gold cube", 3);
                    //int temp = PhotonNetwork.PlayerList.Length;
                    //Debug.Log("this is the amount of player in tha game" + temp);
                    //Player tempPlayer = PhotonNetwork.PlayerList[temp-1];
                    //PV.TransferOwnership(tempPlayer);
                    //if (PV.IsMine)
                    //{
                    //    PhotonNetwork.Destroy(this.gameObject);
                    //}
                    Analytics.instance.writeEvent("Player 2 finishes the gold cube", 3);

                    PV.RPC("ChangeStateToDelete", RpcTarget.AllBuffered);
                }
            }
            else
            {
                Debug.Log("helllllloooooo");
                PV.RPC("ChangeStateToHold", RpcTarget.AllBuffered);
                if (interactor.transform.parent.parent.gameObject.tag == "P1")
                {
                    Analytics.instance.writeEvent("Player 1 grabs the gold cube", 3);
                }
                else
                {
                    Analytics.instance.writeEvent("Player 2 grabs the gold cube", 3);
                }
                //currentZone = holdGold;
            }
        }
    }

    protected override void OnSelectExited(XRBaseInteractor interactor)
    {
        collider.isTrigger = false;

        if (currentZone == BuildWallZone)
        {
            Debug.Log("In the build wall");
            Analytics.instance.writeEvent("Gold Cube Added", 3);
        }
        else if (currentZone == NoZone || currentZone == holdGold)
        {
            Debug.Log("Delete GOLD CUBES");
            PV.RPC("ChangeStateToDelete", RpcTarget.AllBuffered);
        }
        else
        {
            Debug.Log("destory this cube");
            Analytics.instance.writeEvent("Gold Cube Destroyed", 3);
            //MasterBuildWall.instance.removeCube(index, MasterBuildWall.instance.gameObjectToCubeCode(this.gameObject));
        }


    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "DropZone")
        {
            canDrop = false;
            // currentZone = BuildWallZone;
        }

        if (other.tag == "cube despawner")
        {
            if (GameManager.instance.playerCount == 2)
            {
                if (PV.IsMine)
                {
                    PhotonNetwork.Destroy(this.gameObject);
                }
            }

            // Debug.Log("cube destroyed");
        }
    }
    [PunRPC]
    public void DestoryThisObjectOnNetwork()
    {
        
        PhotonNetwork.Destroy(gameObject);
        
    }

    [PunRPC]
    public void SetMirrorCubeCode(int objID, int mirrorObjID)
    {

        PhotonView temp = PhotonView.Find(objID);
        temp.gameObject.GetComponent<GoldCubeHalf>().mirroredBuildWallCubeID = mirrorObjID;
       
    }
    [PunRPC]
    public void removeCube(int x, int y)
    {

        //MasterBuildWall.instance.GetComponent<PhotonView>().RPC("removeCubeFromMasterWall", RpcTarget.AllBuffered, x, y);
        MasterBuildWall.instance.masterBuildArray[x, y] = null;
        MasterBuildWall.instance.updateMasterArray = true;

        //Debug.Log("Delete this cube: "+ mirroredBuildWallCube.name);

        PhotonView temp = PhotonView.Find(mirroredBuildWallCubeID);

        PhotonNetwork.Destroy(temp.gameObject);
        Debug.Log("WE ARE HERE");

    }




}
