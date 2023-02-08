using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Array2DEditor;


public enum State
{
    Play, 
    Build, 
    None, 
    Held,
    Gold
}

public class XRGrabNetworkInteractable : XRGrabInteractable
{

    public Vector3 playWallTargetPos, buildWallTargetPos;
    public Quaternion buildWallTargetRotation;

    public string currentZone;
    public int playersHoldingCube = 0;
    public int mirroredBuildWallCubeID;
    public Vector3 goldCubeHoldPos;
    public State currentState;

    public string playWallZone = "PlayWall";
    public string BuildWallZone = "BuildWall";
    public string NoZone = "No Zone";
    public string holdGold = "Hold Gold";
    public string currentBuildWall;

    public bool isHeld = false;
    public bool canDrop = false;
    public bool updateBuildWallState;

    public Vector2Int index;

    private BoxCollider collider;
    public PhotonView photonView;
    private Transform currentPos;
    //Cube cube;
    private Rigidbody rb;

    private float playZoneFallSpeed = 3f;

    public GameObject rightRay;
    public GameObject leftRay;
    public GameObject mirroredBuildWallCube;
    public GameObject currentDropZone;

    public LineRenderer rightLineRenderer;
    public LineRenderer leftLineRenderer;

    private Vector3[] rightRayPoints = new Vector3[2];
    public Vector3[] leftRayPoints;

 
    void Start()
    {
        if (gameObject.tag == "blue cube")
        {
            if (GameManager.instance.host)
            {
                gameObject.layer = LayerMask.NameToLayer("blue cube");
            }
        }
        //StartCoroutine(CanDropCubeTimer());
        photonView = GetComponent<PhotonView>();
        collider = GetComponent<BoxCollider>();
        rightRay = GameObject.FindGameObjectWithTag("right ray");
        rightLineRenderer = rightRay.GetComponent<LineRenderer>();
        //cube = GetComponent<Cube>();
    }

    void Update()
    {
        if (currentZone == playWallZone)
        {
            MoveCubePlayWall();
        }
        if (updateBuildWallState)
        {
            photonView.RPC("ChangeStateBuildWall", RpcTarget.AllBuffered);
            updateBuildWallState = false;
        }

        if (currentZone == BuildWallZone)
        {
            //Debug.Log("please");
            //photonView.RPC("ChangeStateBuildWall", RpcTarget.AllBuffered);
            if (GameManager.instance.host)
            {
                MoveCubeBuildWall();
            }
        }
        if (currentZone == NoZone)
        {
            if (photonView.IsMine)
            {
                rightLineRenderer.GetPositions(rightRayPoints);
                gameObject.transform.position = rightRayPoints[rightRayPoints.Length - 1];
            }
            else
            {
                //Debug.Log("not yours");
            }
        }
        if (currentZone == holdGold)
        {
            transform.position = goldCubeHoldPos;
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

    public void GoldHold(Vector3 CubePos)
    {
        photonView.RequestOwnership();
        currentZone = holdGold;
        goldCubeHoldPos = CubePos;
    }

    public void PlayerGrab()
    {
        //Analytics.instance.WriteData(gameObject.name + " was picked up", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());
        Analytics.instance.writeEvent(gameObject.name + " was picked up", 3);
        //Analytics.instance.WriteData2(gameObject.name + " was picked up", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());
        //Analytics.instance.WriteData3(gameObject.name + " was picked up", "", "", transform.position.x.ToString(), transform.position.y.ToString(), transform.position.z.ToString());

        photonView.RequestOwnership();
        AudioManager.instance.grabCube.Play();

        currentZone = NoZone;
        Debug.Log("New zone: " + currentZone);
        photonView.RPC("ChangeState", RpcTarget.AllBuffered);
        collider.isTrigger = true;
    }
    public void PlayerGrabGoldHalf()
    {
        photonView.RequestOwnership();
        currentZone = NoZone;
        collider.isTrigger = true;
    }

    IEnumerator CanDropCubeTimer()
    {
        yield return new WaitForSeconds(3);
        canDrop = true;
    }

    protected override void OnSelectEntered(XRBaseInteractor interactor)
    {
        base.OnSelectEntered(interactor);
        Debug.Log("This cube was grabed");
        if (GameManager.instance.holdingGoldHalf)
        {
            Debug.Log("cant pick anything up");
        }
        else
        if(currentZone == BuildWallZone)
        {
            PlayerGrab();
            //photonView.RequestOwnership();
            Debug.Log("trying to remove cube from build walls");
            photonView.RPC("removeCube", RpcTarget.AllBuffered, index.x, index.y);
            //PhotonNetwork.Destroy(gameObject);

        }
        else 
        if (interactor.transform.parent.parent.gameObject.tag == "P1")
        {
            if (gameObject.tag == "gold cube")
            {
                Debug.Log("gold cube was hit");
                photonView.RPC("IncreaseGoldCubeNetworkVar", RpcTarget.AllBuffered);
                if (playersHoldingCube == 2)
                {
                    PhotonNetwork.Instantiate("Network Gold Left Half", transform.position, Quaternion.identity);
                    NetworkManager.Destroy(gameObject);
                }
                else
                {
                    Debug.Log("gold cube zone change");
                    GoldHold(gameObject.transform.position);
                }

            }

            if (gameObject.tag == "left gold cube")
            {
                PlayerGrabGoldHalf();
            }

            Debug.Log("teir 1 pass");
            if (gameObject.tag == "red cube" || gameObject.tag == "invis cube")
            {
                //canDrop = true;
                Debug.Log(gameObject.tag + " was grabbed");
                PlayerGrab();
            }
        }
        else if (interactor.transform.parent.parent.gameObject.tag == "P2")
        {
            if (gameObject.tag == "gold cube")
            {
                Debug.Log("gold cube was hit");
                photonView.RPC("IncreaseGoldCubeNetworkVar", RpcTarget.AllBuffered);
                if (playersHoldingCube == 2)
                {
                    PhotonNetwork.Instantiate("Network Gold Right Half", transform.position, Quaternion.identity);
                    NetworkManager.Destroy(gameObject);
                }
                else
                {
                    Debug.Log("gold cube zone change");
                    GoldHold(gameObject.transform.position);
                }

            }
            if (gameObject.tag == "right gold cube")
            {
                PlayerGrabGoldHalf();
            }

            if (gameObject.tag == "blue cube" || gameObject.tag == "invis cube")
            {
                //canDrop = true;
                Debug.Log(gameObject.tag + " was grabbed");
                PlayerGrab();
            }
        }
    }

    protected override void OnSelectExited(XRBaseInteractor interactor)
    {
        collider.isTrigger = false;

        if (canDrop)
        {
            Debug.Log("We ARE dropping");
            currentDropZone.GetComponent<MeshRenderer>().material = currentDropZone.GetComponent<DropzoneScript>().normalColor;
            currentDropZone.GetComponent<DropzoneScript>().RemoveArrow();
            MasterBuildWall.instance.dropZoneHit(currentDropZone.GetComponent<DropzoneScript>().index, currentDropZone.GetComponent<DropzoneScript>().direction, gameObject);
        }
        else if (currentZone == NoZone)
        {
            PhotonNetwork.Destroy(this.gameObject);
        } 
        else{
            Debug.Log("destory this cube");
            //MasterBuildWall.instance.removeCube(index, MasterBuildWall.instance.gameObjectToCubeCode(this.gameObject));
        }

        
    }

    protected override void OnHoverEntered(XRBaseInteractor interactor)
    {
        Debug.Log(gameObject.name + " has been hovered");
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "DropZone")
        {
            //canDrop = false;
            if (currentZone != BuildWallZone)
            {
                //int col;
                //BuildWall.instance.DropBox(gameObject, other.GetComponent<DropzoneScript>().column);
                //currentZone = BuildWallZone;
            }
        }
        if (other.tag == "cube despawner")
        {
            if (GameManager.instance.playerCount == 2)
            {
                if (photonView.IsMine)
                {
                    PhotonNetwork.Destroy(this.gameObject);
                }
            }
            else
            {
                Destroy(this.gameObject);

            }

            // Debug.Log("cube destroyed");
        }
    }

    public void ChangeStateToBuildWallCall()
    {
        photonView.RPC("ChangeStateBuildWall", RpcTarget.AllBuffered);
    }
    
    [PunRPC]
    public void ChangeStateBuildWall()
    {
        Debug.Log("current zone is now buildwall");
        currentZone = "BuildWall";
    }
    
    [PunRPC]
    public void ChangeState()
    {
        PhotonView temp = PhotonView.Find(photonView.ViewID);
        temp.gameObject.GetComponent<XRGrabNetworkInteractable>().currentZone = NoZone;
        currentZone = NoZone;
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

    }
}
