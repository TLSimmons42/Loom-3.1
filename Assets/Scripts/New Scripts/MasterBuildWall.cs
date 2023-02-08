using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Array2DEditor;
using Photon.Pun;

public class MasterBuildWall : Singleton<MasterBuildWall>
{
    [SerializeField]
    private Array2DString editorBuildMasterArray, editorTargetWall;

    private string[,] levelImport;
    public string[,] targetWall;
    public string[,] masterBuildArray = new string[5, 5];
    private GameObject[,] hostSpotHolderArray = new GameObject[5, 5];
    private GameObject[,] clientSpotHolderArray = new GameObject[5, 5];
    private int[,] hostWallArrayIDs = new int[5, 5];
    private int[,] clientWallArrayIDs = new int[5, 5];
    public int buildWallLength;
    public int colCounter = 0;

    public GameObject hostBuildWallLocation, clientBuildWallLocation;

    public GameObject dropZone;
    public GameObject spotPlaceHolder;

    int index;

    public GameObject redCube, blueCube, invsCube, goldCube, leftGoldCube, rightGoldCube;

    PhotonView PV;

    public bool updateMasterArray = false;


    public void Start()
    {
        buildWallLength = masterBuildArray.GetLength(0);
        Debug.Log(buildWallLength);

        PV = GetComponent<PhotonView>();

    }
    private void Update()
    {
        if (updateMasterArray)
        {
            PV.RPC("UpdateMasterArray", RpcTarget.AllBuffered);
            updateMasterArray = false;
            Debug.Log("Updating the master ARrfay");
        }
        if (Input.GetKeyDown("space"))
        {
            displayEditorMasterArray();
        }
    }

    public void importLevel()
    {
        Debug.Log("Is host: " + GameManager.instance.host);


        if (GameManager.instance.playerCount == 2)
        {
            if (GameManager.instance.host)
            {
                Debug.Log("Difficulty: " + PlayerPrefs.GetString("gameDifficulty"));

                Debug.Log("Setting level import...");
                index = Levels.instance.getRandomIndex(PlayerPrefs.GetString("gameDifficulty"));
                
                Debug.Log("this is the index: "+index);

                    PV.RPC("sendIndex", RpcTarget.AllBuffered, index);
                    Debug.Log("Making build walls...");
                    PV.RPC("initializeBuildWalls", RpcTarget.AllBuffered);
                    Debug.Log("Making view walls...");
                    PV.RPC("buildViewWall", RpcTarget.AllBuffered);
            }
           
        }
        else
        {
            Debug.Log("single player setup");
            index = Levels.instance.getRandomIndex(PlayerPrefs.GetString("gameDifficulty"));
            sendIndex(index);
            initializeBuildWalls();
            buildViewWall();
        }
    }

    [PunRPC]
    public void buildViewWall()
    {
        //setTargetWall();
        GameManager.instance.targetWall = targetWall;
        GameManager.instance.BuildViewWall();
    }

    [PunRPC]
    public void sendIndex(int index)
    {
        Debug.Log("SEND INDEX");
        levelImport = Levels.instance.getLevelFromIndex(PlayerPrefs.GetString("gameDifficulty"), index);
        //levelImport = Levels.instance.getLevelFromIndex("medium", index);

        setTargetWall();
    }


    public void DebugLog2DArray(string[,] array)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                Debug.Log(array[i, j]);
            }
        }
    }

    [PunRPC]
    public void setLevelImport(string[,] level)
    {
        levelImport = level;
    }

    // Instaniates the drop zones
    public void initalizeDropZones()
    {
        for (int i = 0; i < levelImport.GetLength(0); i++)
        {
            for (int j = 0; j < levelImport.GetLength(1); j++)
            {
                //Debug.Log("Current indices: (" + i + ", " + j + ")");
                if (i == 0 || i == levelImport.GetLength(0) - 1 || j == 0 || j == levelImport.GetLength(1) - 1)
                {
                    if (!levelImport[i, j].Equals(""))
                    {
                       // Debug.Log("Spawning cube at: (" + i + ", " + j + ")");
                        GameObject hostDropZone = Instantiate(dropZone, hostBuildWallLocation.transform);
                        hostDropZone.GetComponent<DropzoneScript>().buildWallNumber = "h";

                        hostDropZone.GetComponent<DropzoneScript>().direction = indicesToDirection(j, i, levelImport);
                        hostDropZone.GetComponent<DropzoneScript>().index = new Vector2Int(j, i);

                        hostDropZone.transform.position = hostBuildWallLocation.transform.position;
                        hostDropZone.transform.position += hostBuildWallLocation.transform.right * -j;
                        hostDropZone.transform.position += hostBuildWallLocation.transform.up * -i;

                        hostDropZone.transform.parent = hostBuildWallLocation.transform;

                        GameObject clientDropZone = Instantiate(dropZone, clientBuildWallLocation.transform);
                        clientDropZone.GetComponent<DropzoneScript>().buildWallNumber = "c";
                        clientDropZone.GetComponent<DropzoneScript>().direction = indicesToDirection(j, i, levelImport);
                        clientDropZone.GetComponent<DropzoneScript>().index = new Vector2Int(j, i);

                        clientDropZone.transform.position = clientBuildWallLocation.transform.position;
                        clientDropZone.transform.position += clientBuildWallLocation.transform.right * -j;
                        clientDropZone.transform.position += clientBuildWallLocation.transform.up * -i;

                        clientDropZone.transform.parent = clientBuildWallLocation.transform;
                    }
                }
            }
        }
    }

    // Converts given indices and returns the direction is should face
    public string indicesToDirection(int x, int y, string[,] array)
    {
        if (x == 0) {
            return "right";
        } else if (x == array.GetLength(0) - 1) {
            return "left";
        } else if (y == 0) {
            return "down";
        } else if (y == array.GetLength(1) - 1) {
            return "up";
        } else {
            return "invalid";
        }
    }

    [PunRPC]
    // Grabs the inside of the imported wall (ie no dropzone notation) and sets it as the target
    public void setTargetWall()
    {
        targetWall = new string[levelImport.GetLength(0) - 2, levelImport.GetLength(1) - 2];

        for (int i = 1; i < levelImport.GetLength(0) - 1; i++)
        {
            for (int j = 1; j < levelImport.GetLength(1) - 1; j++)
            {
               // Debug.Log("Setting target (" + (i - 1) + ", " + (j - 1) + ") to " + levelImport[i, j]);
                targetWall[j - 1, i - 1] = levelImport[i, j];
            }
        }

        displayEditorTargetWall();
    }

    [PunRPC]
    public void initializeBuildWalls()
    {
        for (int i = 0; i < masterBuildArray.GetLength(0); i++)
        {
            for (int j = 0; j < masterBuildArray.GetLength(1); j++)
            {
                masterBuildArray[i, j] = null;
            }
        }
        hostBuildWallLocation.transform.position += hostBuildWallLocation.transform.up * levelImport.GetLength(0);
        clientBuildWallLocation.transform.position += clientBuildWallLocation.transform.up * levelImport.GetLength(0);
        ConstructBuildWall(hostBuildWallLocation.transform, "host");
        ConstructBuildWall(clientBuildWallLocation.transform, "client");
        initalizeDropZones();
    }

    void ConstructBuildWall(Transform wallLocation, string wall)
    {

        for (int i = 0; i < targetWall.GetLength(0); i++)
        {
            for (int j = 0; j < targetWall.GetLength(0); j++)
            {
                Vector3 spawnLocation = wallLocation.position;
                spawnLocation += wallLocation.right * -(i + 1);
                spawnLocation += wallLocation.up * -(j + 1);

                GameObject placeholder = Instantiate(spotPlaceHolder, spawnLocation, wallLocation.rotation);
                placeholder.transform.parent = wallLocation.transform;
                if (wall == "host")
                {
                    placeholder = hostSpotHolderArray[i, j];
                }
                else if (wall == "client")
                {
                    placeholder = clientSpotHolderArray[i, j];
                }
            }
        }
    }

    private void displayEditorMasterArray()
    {
        for (int x = 0; x < masterBuildArray.GetLength(0); x++)
        {
            for (int y = 0; y < masterBuildArray.GetLength(1); y++)
            {
                //Debug.Log("X:" + x + ", Y:" + y);
                //Debug.Log(masterBuildArray[x, y]);
                editorBuildMasterArray.SetCell(x, y, masterBuildArray[x, y]);
            }
        }
    }
    private void displayEditorTargetWall()
    {
        for (int x = 0; x < targetWall.GetLength(0); x++)
        {
            for (int y = 0; y < targetWall.GetLength(1); y++)
            {
                //Debug.Log("X:" + x + ", Y:" + y);
               // Debug.Log(targetWall[x, y]);
                editorTargetWall.SetCell(x, y, targetWall[x, y]);
            }
        }
    }

    public GameObject cubeCodeToGameObject(string cubeCode)
    {
        switch (cubeCode)
        {
            case "R":
                return redCube;
            case "B":
                return blueCube;
            case "I":
                return invsCube;
            case "G":
                return goldCube;
            case "lG":
                return leftGoldCube;
            case "rG":
                return rightGoldCube;
            default:
                Debug.LogError("Invalid cubeCode: " + cubeCode);
                return null;
        }
    }

    public string cubeCodeToNetworked(string cubeCode)
    {
        switch (cubeCode)
        {
            case "red cube":
                return "Network Red Cube";
            case "blue cube":
                return "Network Blue Cube";
            case "invis cube":
                return "Network Neutral Cube";
            case "gold cube":
                return "Network Gold Cube";
            case "left gold cube":
                return "Network Gold Left Half";
            case "right gold cube":
                return "Network Gold Right Half";
            case "gold cube whole":
                return "Network Gold Cube Whole";
            default:
                Debug.LogError("Invalid cubeCode: " + cubeCode);
                return null;
        }
    }

    public string gameObjectToCubeCode(GameObject cube)
    {
        switch (cube.tag)
        {
            case "red cube":
                return "R";
            case "blue cube":
                return "B";
            case "invis cube":
                return "I";
            case "gold cube":
                return "G";
            case "left gold cube":
                return "lG";
            case "right gold cube":
                return "rG";
            default:
                Debug.LogError("Invalid cube: " + cube.tag);
                return null;
        }
    }

    public string cubeCodeToTag(string cubeCode)
    {
        switch (cubeCode)
        {
            case "R":
                return "red cube";
            case "B":
                return "blue cube";
            case "I":
                return "invis cube";
            case "G":
                return "gold cube";
            case "lG":
                return "left gold cube";
            case "rG":
                return "right gold cube";
            default:
                Debug.LogError("Invalid code: " + cubeCode);
                return null;
        }
    }


    public void dropZoneHit(Vector2Int dropIndex, string direction, GameObject cube)
    {
        Vector2Int startPos, targetPos;
        bool canDrop = true;
        Debug.Log("Direction: " + direction);
        switch (direction)
        {
            case "right":
                for (int i = masterBuildArray.GetLength(0) - 1; i >= 0; i--)
                {
                    if ((masterBuildArray[i, dropIndex.y] == null || masterBuildArray[dropIndex.x, i].Equals("")) && canDrop)
                    {
                        canDrop = false;
                        startPos = dropIndex;
                        targetPos = new Vector2Int(i, dropIndex.y);
                        Debug.Log("Start: " + startPos.ToString());
                        Debug.Log("Target: " + targetPos.ToString());
                        PV.RPC("addCube", RpcTarget.AllBuffered, startPos.x, startPos.y, targetPos.x, targetPos.y, cube.tag);
                        PhotonNetwork.Destroy(cube);
                    }
                    else
                    {
                        Debug.Log(new Vector2Int(i, dropIndex.y) + " is not null; actual: " + masterBuildArray[i, dropIndex.y]);
                    }
                }
                break;
            case "left":
                for (int i = 0; i < masterBuildArray.GetLength(0); i++)
                {
                    if ((masterBuildArray[i, dropIndex.y] == null || masterBuildArray[dropIndex.x, i].Equals("")) && canDrop)
                    {
                        canDrop = false;
                        startPos = dropIndex;
                        targetPos = new Vector2Int(i, dropIndex.y);
                        PV.RPC("addCube", RpcTarget.AllBuffered, startPos.x, startPos.y, targetPos.x, targetPos.y, cube.tag);
                        PhotonNetwork.Destroy(cube);
                    }
                    else
                    {
                        Debug.Log(new Vector2Int(i, dropIndex.y) + " is not null; actual: " + masterBuildArray[i, dropIndex.y]);
                    }
                }

                break;
            case "up":
                for (int i = 0; i < masterBuildArray.GetLength(0); i++)
                {
                    if ((masterBuildArray[dropIndex.x - 1, i] == null || masterBuildArray[dropIndex.x - 1, i].Equals("")) && canDrop)
                    {
                        canDrop = false;
                        startPos = dropIndex;
                        targetPos = new Vector2Int(dropIndex.x - 1, i);
                        Debug.Log("Start: " + startPos.ToString());
                        Debug.Log("Target: " + targetPos.ToString());
                        PV.RPC("addCube", RpcTarget.AllBuffered, startPos.x, startPos.y, targetPos.x, targetPos.y, cube.tag);
                        PhotonNetwork.Destroy(cube);
                    }
                    else
                    {
                        Debug.Log(new Vector2Int(dropIndex.x - 1, i) + " is not null; actual: " + masterBuildArray[dropIndex.x - 1, i]);
                    }
                }
                break;
            case "down":
                AudioManager.instance.dropCubeSound.Play();

                for (int i = masterBuildArray.GetLength(0) - 1; i >= 0; i--)
                {
                    //if(cube.tag == "right gold cube"  ||  cube.tag == "left gold cube")
                    if ((masterBuildArray[dropIndex.x - 1, i] == "left gold cube" && cube.tag == "right gold cube") || (masterBuildArray[dropIndex.x - 1, i] == "right gold cube" && cube.tag == "left gold cube"))
                    {
                        canDrop = false;
                        startPos = dropIndex;
                        targetPos = new Vector2Int(dropIndex.x - 1, i);
                        Debug.Log("MAKE A NEW GOLD WHOLE");


                        PV.RPC("removeHalfCube", RpcTarget.AllBuffered, targetPos.x, targetPos.y, cube.tag);
                        //removeHalfCube(targetPos.x, targetPos.y, cube.tag);
                      
                        PV.RPC("addCube", RpcTarget.AllBuffered, startPos.x, startPos.y, targetPos.x, targetPos.y, "gold cube whole");
                        PhotonNetwork.Destroy(cube);



                        //PhotonNetwork.Destroy(cube);
                        //removeHalfCube(targetPos.x, targetPos.y, cube.tag);


                    }
                    else if ((masterBuildArray[dropIndex.x - 1, i] == null || masterBuildArray[dropIndex.x - 1, i].Equals("")) && canDrop)
                    {
                        canDrop = false;
                        startPos = dropIndex;
                        targetPos = new Vector2Int(dropIndex.x - 1, i);
                       // Debug.Log("Start: " + startPos.ToString());
                        //Debug.Log("Target: " + targetPos.ToString());

                        if (GameManager.instance.playerCount == 2)
                        {
                            PV.RPC("addCube", RpcTarget.AllBuffered, startPos.x, startPos.y, targetPos.x, targetPos.y, cube.tag);
                            PhotonNetwork.Destroy(cube);
                        }
                        else
                        {
                            addCube(startPos.x, startPos.y, targetPos.x, targetPos.y, cube.tag);
                            Destroy(cube);
                        }
                    }
                    else
                    {
                        colCounter++;
                        Debug.Log("coCounter: "+ colCounter);
                        if (colCounter == buildWallLength)
                        {
                            if (GameManager.instance.playerCount == 2)
                            {
                                PhotonNetwork.Destroy(cube);
                            }
                            else
                            {
                                Destroy(cube);
                            }
                        }
                        //Debug.Log(new Vector2Int(dropIndex.x - 1, i) + " is not null; actual: " + masterBuildArray[dropIndex.x - 1, i]);
                    }
                }
                break;
            default:
                Debug.LogError("Trying to drop at zone without valid direction)");
                break;
        }
        colCounter = 0;
    }

    [PunRPC]
    public void addCube(int startX, int startY, int targetX, int targetY, string cubeCode)
    {
        Debug.Log("adding cube");
        Vector2Int start = new Vector2Int(startX, startY);
        Vector2Int target = new Vector2Int(targetX, targetY);
        //Debug.Log(start + "  " + target + "  " + cubeCode);

        //Debug.Log("Add Target: " + target.ToString());
        masterBuildArray[target.x, target.y] = cubeCode;

        if (GameManager.instance.playerCount == 2)
        {

            if (GameManager.instance.host)
            {
                Vector3 hostSpawnLocation = hostBuildWallLocation.transform.position;
                hostSpawnLocation += hostBuildWallLocation.transform.right * -(start.x);
                hostSpawnLocation += hostBuildWallLocation.transform.up * -(start.y + 1);

                Vector3 clientSpawnLocation = clientBuildWallLocation.transform.position;
                clientSpawnLocation += clientBuildWallLocation.transform.right * -(start.x);
                clientSpawnLocation += clientBuildWallLocation.transform.up * -(start.y + 1);

                string networkedCode = cubeCodeToNetworked(cubeCode);

                GameObject hostCube = PhotonNetwork.Instantiate(networkedCode, hostSpawnLocation, hostBuildWallLocation.transform.rotation);
                GameObject clientCube = PhotonNetwork.Instantiate(networkedCode, clientSpawnLocation, clientBuildWallLocation.transform.rotation);
                PV.RPC("SetCubeIndex", RpcTarget.AllBuffered, cubeCode, hostCube.GetComponent<PhotonView>().ViewID, clientCube.GetComponent<PhotonView>().ViewID, target.x, target.y);
                PV.RPC("SetCubeIndex", RpcTarget.AllBuffered, cubeCode, clientCube.GetComponent<PhotonView>().ViewID, hostCube.GetComponent<PhotonView>().ViewID, target.x, target.y);


                if (cubeCode == "left gold cube" || cubeCode == "right gold cube")
                {
                    addToBuildWall(hostCube.GetComponent<GoldCubeHalf>(), target, "host");
                    addToBuildWall(clientCube.GetComponent<GoldCubeHalf>(), target, "client");
                    hostCube.GetComponent<GoldCubeHalf>().mirroredBuildWallCube = clientCube;
                    clientCube.GetComponent<GoldCubeHalf>().mirroredBuildWallCube = hostCube;
                }
                //else if (cubeCode == "gold cube")
                //{
                //    addToBuildWall(hostCube.GetComponent<GoldCubeWhole>(), target, "host");
                //    addToBuildWall(clientCube.GetComponent<GoldCubeWhole>(), target, "client");
                //    hostCube.GetComponent<GoldCubeWhole>().mirroredBuildWallCube = clientCube;
                //    clientCube.GetComponent<GoldCubeWhole>().mirroredBuildWallCube = hostCube;
                //}
                else
                {
                    addToBuildWall(hostCube.GetComponent<XRGrabNetworkInteractable>(), target, "host");
                    addToBuildWall(clientCube.GetComponent<XRGrabNetworkInteractable>(), target, "client");
                    hostCube.GetComponent<XRGrabNetworkInteractable>().mirroredBuildWallCube = clientCube;
                    clientCube.GetComponent<XRGrabNetworkInteractable>().mirroredBuildWallCube = hostCube;
                    //hostWallArrayIDs[target.x, target.y] = hostCube.GetComponent<PhotonView>().ViewID;
                    //clientWallArrayIDs[target.x, target.y] = clientCube.GetComponent<PhotonView>().ViewID;
                }
            }
        }
        else
        {
            Debug.Log("1 player cube droping");
            Vector3 hostSpawnLocation = hostBuildWallLocation.transform.position;
            hostSpawnLocation += hostBuildWallLocation.transform.right * -(start.x);
            hostSpawnLocation += hostBuildWallLocation.transform.up * -(start.y + 1);

            GameObject normalCube = cubeCodeToGameObject(cubeCode);

            GameObject newCube = Instantiate(normalCube, hostSpawnLocation, hostBuildWallLocation.transform.rotation);
            newCube.GetComponent<Cube>().currentZone = newCube.GetComponent<Cube>().BuildWallZone;
            newCube.GetComponent<Cube>().index = target;
            newCube.GetComponent<Cube>().buildWallTargetPos = new Vector3((target.x + 1), -(target.y + 1), 0) + hostBuildWallLocation.transform.position;


        }
    }

    private void addToBuildWall(XRGrabNetworkInteractable script, Vector2Int target, string buildWall)
    {
        Debug.Log("changing the zone to buildwall");
        //script.canBeDroped = false;
        script.updateBuildWallState = true;
        script.index = target;
        if (buildWall == "host")
        {
            script.buildWallTargetPos = new Vector3((target.x + 1), -(target.y + 1), 0) + hostBuildWallLocation.transform.position;
            script.currentBuildWall = "host";
        }
        else
        {
            script.buildWallTargetPos = new Vector3(-(target.x + 1), -(target.y + 1), 0) + clientBuildWallLocation.transform.position;
            script.currentBuildWall = "client";
        }
    }

    private void addToBuildWall(GoldCubeHalf script, Vector2Int target, string buildWall)
    {
        //script.canBeDroped = false;
        script.updateBuildWallState = true;
        script.index = target;
        if (buildWall == "host")
        {
            script.buildWallTargetPos = new Vector3((target.x + 1), -(target.y + 1), 0) + hostBuildWallLocation.transform.position;
            script.currentBuildWall = "host";
        }
        else
        {
            
            script.buildWallTargetPos = new Vector3(-(target.x + 1), -(target.y + 1), 0) + clientBuildWallLocation.transform.position;
            script.currentBuildWall = "client";
        }
    }

    private void addToBuildWall(GoldCubeWhole script, Vector2Int target, string buildWall)
    {
        //script.canBeDroped = false;
        script.updateBuildWallState = true;
        script.index = target;
       if (buildWall == "host")
       {
            script.buildWallTargetPos = new Vector3((target.x + 1), -(target.y + 1), 0) + hostBuildWallLocation.transform.position;
            script.currentBuildWall = "host";
        }
        else
       {
            script.currentBuildWall = "client";
            script.buildWallTargetPos = new Vector3(-(target.x + 1), -(target.y + 1), 0) + clientBuildWallLocation.transform.position;
       }
    }

    [PunRPC]
    public void removeHalfCube(int index_x, int index_y, string cubeCode)
    {
        Debug.Log("this the cubeCode: " + cubeCode);
        GameObject[] cubesForDeletion;
        if(cubeCode == "left gold cube")
        {
            cubesForDeletion = GameObject.FindGameObjectsWithTag("right gold cube");
        }
        else
        {
            cubesForDeletion = GameObject.FindGameObjectsWithTag("left gold cube");
        }

        foreach (GameObject cube in cubesForDeletion)
        {
            Debug.Log("found a half cube");
            Debug.Log("this the index for remove: " + index_x + "this the index for remove: " + index_y);
            if (cube.GetComponent<GoldCubeHalf>().index.x == index_x && cube.GetComponent<GoldCubeHalf>().index.y == index_y)
            {
                cube.GetComponent<GoldCubeHalf>().destroyCube = true;
                Debug.Log("deleting a half cube now");
                //PhotonNetwork.Destroy(cube);
  
            }
        }
    }
    [PunRPC]
    public void SetCubeIndex(string cubeCode, int objID, int mirrorObjID, int index_x, int index_y) 
    { 
        if(cubeCode == "blue cube" || cubeCode == "red cube" || cubeCode == "invis cube" || cubeCode == "gold cube whole")
        {
            PhotonView temp = PhotonView.Find(objID);
            temp.gameObject.GetComponent<XRGrabNetworkInteractable>().mirroredBuildWallCubeID = mirrorObjID;
            temp.gameObject.GetComponent<XRGrabNetworkInteractable>().index.x = index_x;
            temp.gameObject.GetComponent<XRGrabNetworkInteractable>().index.y = index_y;
        }
        if (cubeCode == "left gold cube" || cubeCode == "right gold cube")
        {
            PhotonView temp = PhotonView.Find(objID);
            temp.gameObject.GetComponent<GoldCubeHalf>().mirroredBuildWallCubeID = mirrorObjID;
            temp.gameObject.GetComponent<GoldCubeHalf>().index.x = index_x;
            temp.gameObject.GetComponent<GoldCubeHalf>().index.y = index_y;
        }
        if (cubeCode == "gold cube")
        {
            PhotonView temp = PhotonView.Find(objID);
            temp.gameObject.GetComponent<GoldCubeWhole>().mirroredBuildWallCubeID = mirrorObjID;
            temp.gameObject.GetComponent<GoldCubeWhole>().index.x = index_x;
            temp.gameObject.GetComponent<GoldCubeWhole>().index.y = index_y;
        }

    }
    

    [PunRPC]
    public void removeCubeFromMasterWall(int x, int y)
    {
        Debug.Log("removing cube from the master array");
        masterBuildArray[x, y] = null;
    }
    [PunRPC]
    public void UpdateMasterArray()
    {
        masterBuildArray = masterBuildArray;
    }
}