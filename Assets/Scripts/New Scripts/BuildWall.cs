using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

public class BuildWall : Singleton<BuildWall>
{
    GameObject[,] buildWallArr;
    public GameObject otherBuildWall;
    int gameDiff, levelSize;
    public GameObject cubeSlot, dropZone;
    int[,] viewWall;

    public GameObject newBox;

    PhotonView PV;


    private void Awake()
    {
        //PlayerPrefs.SetString("gameDifficulty", "easy");
        //PlayerPrefs.SetInt("playerCount", 2);
    }

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        //ConvertGameDiffToInt(PlayerPrefs.GetString("gameDifficulty"));
        //InitiateBuildWall(gameDiff);
        //DisplayBuildWall(gameDiff);
    }

    // Update is called once per frame
    void Update()
    {
        RemovePickedUpCube();
        //DebugPrint2DArray(CubeToInt());
        //DebugPrint2DArray(viewWall);
        //ConsoleCheckBuildWall();
    }

    void ConvertGameDiffToInt(string strDiff)
    {
        switch (strDiff)
        {
            case "easy":
                gameDiff = 1;
                break;
            case "medium":
                gameDiff = 2;
                break;
            case "hard":
                gameDiff = 3;
                break;
        }
    }

    void InitiateBuildWall(int difficulty)
    {
        levelSize = difficulty * 5;

        buildWallArr = new GameObject[levelSize, levelSize];
        ////Debug.Log(buildWallArr.Length);
    }

    void DisplayBuildWall(int difficulty)
    {
        //int viewWallSize = difficulty * 5;
        ////viewWallSize = 15;

        //for (int i = 0; i < viewWallSize; i++)
        //{
        //    Vector3 spawnLocation = transform.position;
        //    spawnLocation += transform.right * -i;
        //    spawnLocation += (viewWallSize) * transform.up;

        //    ////Debug.Log("Attempting to spawn placeholder");

        //    GameObject spawnedCube = Instantiate(dropZone, spawnLocation, transform.rotation);
        //    spawnedCube.GetComponent<DropzoneScript>().SetColumn(i);
        //    spawnedCube.transform.parent = transform;

        //    for (int j = 0; j < viewWallSize; j++)
        //    {
        //        spawnLocation = transform.position;
        //        spawnLocation += transform.right * -i;
        //        spawnLocation += transform.up * j;

        //       // //Debug.Log("Attempting to spawn placeholder");

        //        spawnedCube = Instantiate(cubeSlot, spawnLocation, transform.rotation);
        //        spawnedCube.transform.parent = transform;
        //        //spawnedCube.GetComponent<Cube>().enabled = false;
        //    }
        //}
    }
    bool CheckGoldMatch(GameObject cube1, GameObject cube2)
    {
        if (cube1 != null && cube2 != null) {
            if (cube1.tag == "left gold cube" && cube2.tag == "right gold cube")
            {
                return true;
                //PhotonNetwork.Instantiate("Network Gold Cube", PlaywallDropPoints[spawnPointChoice].transform.position, Quaternion.identity);
                //PhotonNetwork.Destroy()
            }
            if (cube1.tag == "right gold cube" && cube2.tag == "left gold cube")
            {
                return true;
            }
        }

        return false;
    }

    GameObject GetLastDropedCube(int col)
    {
        for (int i = 0; i < buildWallArr.Length; i++)
        {

            //Debug.Log(col + ", " + i + ": " + buildWallArr[col, i]);

            if (buildWallArr[col, i] == null)
            {
                if(i == 0)
                {
                    return null;
                }
                return buildWallArr[col, i-1];
            }
        }
        return null;
    }

    int GetNextFreeRow(int col)
    {
        //Debug.Log(buildWallArr.Length);
        for (int i = 0; i < buildWallArr.Length; i++)
        {
            //Debug.Log(i);
            Debug.Log(col + ", " + i + ": " + buildWallArr[col, i]);

            if (buildWallArr[col, i] == null)
            {
                //Debug.Log("The next free row is: " + i);
                return i;
            }
            else
            {
                Debug.Log("NOT NULL");
            }
        }
        //Debug.Log("There is no free row");
        return -1;
    }

    public void DropBox(GameObject box, int col)
    {
        box.GetComponent<BoxCollider>().isTrigger = false;
        int nextFreeRow = GetNextFreeRow(col);
        GameObject lastRowObj = GetLastDropedCube(col);

        if (nextFreeRow == -1)
        {
            if (PlayerPrefs.GetInt("playerCount") == 1)
            {
                Destroy(box);
            }
            else
            {
                PhotonNetwork.Destroy(box);
            }
        }
        else
        {
            if (PlayerPrefs.GetInt("playerCount") == 1)
            {
                box.transform.rotation = this.transform.rotation;
                //Debug.Log("calculating the build wall pos for the cube");
                box.transform.position = transform.position + (transform.right * -col) + (transform.up * (levelSize + 1));

                Vector3 newLocation = transform.position;
                newLocation += transform.right * -col;
                newLocation += transform.up * nextFreeRow;

                buildWallArr[col, nextFreeRow] = box;
                //PV.RPC("changeState", RpcTarget.AllBuffered);
                //box.transform.position = newLocation;
                box.GetComponent<Cube>().currentZone = "BuildWall";
                box.GetComponent<Cube>().SetZoneToBuild();
                box.GetComponent<Cube>().buildWallTargetPos = newLocation;
                box.GetComponent<Cube>().buildWallTargetRotation = this.transform.rotation;
                //box.GetComponent<BoxCollider>().enabled = false;
            }
            else
            {
                
                        bool isMatch = CheckGoldMatch(box, lastRowObj);
                        if (isMatch)
                        {
                            if (GameManager.instance.host)
                            {
                                Debug.Log("next free row: " + nextFreeRow);
                                Vector3 tempPos = box.transform.position;
                                PhotonNetwork.Destroy(box);
                                PhotonNetwork.Destroy(buildWallArr[col, nextFreeRow - 1]);
                                DeleteMirrodCube(col, nextFreeRow - 1);
                                box = PhotonNetwork.Instantiate("Network Gold Cube", tempPos, Quaternion.identity);
                                //buildWallArr[col, nextFreeRow - 1] = box;
                                nextFreeRow -= 1;
                                Analytics.instance.writeEvent("[Network] Gold Cube Complete", 3);
                            }
                        }

                    box.transform.rotation = this.transform.rotation;
                    //Debug.Log("calculating the build wall pos for the cube");
                    box.transform.position = transform.position + (transform.right * -col) + (transform.up * (levelSize + 1));

                    Vector3 newLocation = transform.position;
                    newLocation += transform.right * -col;
                    newLocation += transform.up * nextFreeRow;
                    
                    Debug.Log("putting box into array: " + box.name);
                    buildWallArr[col, nextFreeRow] = box;
                    PV.RPC("FillArrayOverNetwork", RpcTarget.AllBuffered, col, nextFreeRow);

                    //box.transform.position = newLocation;
                    if (box.tag == "left gold cube" || box.tag == "right gold cube")
                    {
                        box.GetComponent<GoldCubeHalf>().currentZone = "BuildWall";
                        //box.GetComponent<GoldCubeHalf>().SetZoneToBuild();
                        box.GetComponent<GoldCubeHalf>().buildWallTargetPos = newLocation;
                        box.GetComponent<GoldCubeHalf>().buildWallTargetRotation = this.transform.rotation;
                    }
                    else if (box.tag == "gold cube") 
                    {
                        box.GetComponent<GoldCubeWhole>().currentZone = "BuildWall";
                        //box.GetComponent<GoldCubeHalf>().SetZoneToBuild();
                        box.GetComponent<GoldCubeWhole>().buildWallTargetPos = newLocation;
                        box.GetComponent<GoldCubeWhole>().buildWallTargetRotation = this.transform.rotation;
                    }
                    else
                    {
                        box.GetComponent<XRGrabNetworkInteractable>().currentZone = "BuildWall";
                        box.GetComponent<XRGrabNetworkInteractable>().SetZoneToBuild();
                        box.GetComponent<XRGrabNetworkInteractable>().buildWallTargetPos = newLocation;
                        box.GetComponent<XRGrabNetworkInteractable>().buildWallTargetRotation = this.transform.rotation;
                    }
                        //box.GetComponent<BoxCollider>().enabled = false;

                    MirrorBuildWalls(col, nextFreeRow, box);
                }
            
            
        }
    }
    private string tempName;
    public void MirrorBuildWalls(int col, int row, GameObject clonedBox)
    {
        //Debug.Log("# of players = " + PlayerPrefs.GetInt("playerCount"));
        if (PlayerPrefs.GetInt("playerCount") == 2)
        {
            Debug.Log("MIRRORING THE CUBE");
            Vector3 newLocation = otherBuildWall.transform.position;
            newLocation += otherBuildWall.transform.right * -col;
            newLocation += otherBuildWall.transform.up * row;
            Vector3 tempPos = otherBuildWall.transform.position + (otherBuildWall.transform.right * -col) + (otherBuildWall.transform.up * (levelSize + 1));

            
            if(clonedBox.tag== "red cube"){
                tempName = "Network Red Cube";
            }
            if (clonedBox.tag == "blue cube")
            {
                tempName = "Network Blue Cube";
            }
            if (clonedBox.tag == "invis cube")
            {
                tempName = "Network Neutral Cube";
            }
            if (clonedBox.tag == "gold cube")
            {
                tempName = "Network Gold Cube";
            }
            if (clonedBox.tag == "left gold cube")
            {
                tempName = "Network Gold Left Half";
            }
            if (clonedBox.tag == "right gold cube")
            {
                tempName = "Network Gold Right Half";
            }

            Debug.Log("this is being made " + tempName);
            if (GameManager.instance.host)
            {
                newBox = PhotonNetwork.Instantiate(tempName, tempPos, Quaternion.identity);


                if (tempName == "Network Gold Right Half" || tempName == "Network Gold Left Half")
                {
                    //newBox.GetComponent<GoldCubeHalf>().canBeDroped = false;
                    newBox.GetComponent<GoldCubeHalf>().buildWallTargetPos = newLocation;
                    newBox.GetComponent<GoldCubeHalf>().currentZone = newBox.GetComponent<GoldCubeHalf>().BuildWallZone;
                }
                else if (tempName == "Network Gold Cube")
                {
                    //newBox.GetComponent<GoldCubeWhole>().canBeDroped = false;
                    newBox.GetComponent<GoldCubeWhole>().buildWallTargetPos = newLocation;
                    newBox.GetComponent<GoldCubeWhole>().currentZone = newBox.GetComponent<GoldCubeWhole>().BuildWallZone;
                }
                else
                {
                    newBox.GetComponent<XRGrabNetworkInteractable>().buildWallTargetPos = newLocation;
                    newBox.GetComponent<XRGrabNetworkInteractable>().currentZone = newBox.GetComponent<XRGrabNetworkInteractable>().BuildWallZone;
                }
                newBox.GetComponent<BoxCollider>().isTrigger = false;
            }


            //newBox.GetComponent<BoxCollider>().enabled = false;
            //   newBox.transform.position = otherBuildWall.transform.position + (otherBuildWall.transform.right * -col) + (otherBuildWall.transform.up * (levelSize + 1));

            //buildWallArr[col, row] = box;

            //otherBuildWall.GetComponent<BuildWall>().buildWallArr = buildWallArr;   
            otherBuildWall.GetComponent<BuildWall>().buildWallArr[col, row] = newBox;

            //box.transform.position = newLocation;

        }
    }

    public int[,] CubeToInt()
    {
        int[,] intArr = new int[levelSize, levelSize];

        for (int i = levelSize - 1; i >= 0; i--)
        {
            for (int j = 0; j < levelSize; j++)
            {
                if (buildWallArr[i, j] == null)
                {
                    intArr[i, j] = -1;
                }
                else
                {
                    string cubeName = buildWallArr[i, j].tag;

                    switch (cubeName)
                    {
                        case "blue cube":
                            intArr[i, j] = 1;
                            break;
                        case "red cube":
                            intArr[i, j] = 2;
                            break;
                        case "gold cube":
                            intArr[i, j] = 3;
                            break;
                        case "invis cube":
                            intArr[i, j] = 4;
                            break;
                    }
                }
            }
        }

        return intArr;
    }

    void DebugPrint2DArray(int[,] arr)
    {
        //Debug.Log("Start");
        for (int i = 0; i < levelSize; i++)
        {
            string line = "";
            for (int j = 0; j < levelSize; j++)
            {
                line += arr[j,i].ToString();
                
            }
           // Debug.Log(line);
        }
        //Debug.Log("End");
    }

    void ConsoleCheckBuildWall()
    {
        Debug.Log("Does the build wall match the view wall?: " + CheckBuildWall());
    }
    public bool CheckBuildWall()
    {
        int[,] buildWall = CubeToInt();

        for (int i = 0; i < levelSize; i++)
        {
            for (int j = 0; j < levelSize; j++)
            {
                if (buildWall[i, j] != viewWall[i, j]) return false;
            }
        }

        return true;
    }

    public void SetViewWall(int[,] level)
    {
        viewWall = level; 
    }

    void RemovePickedUpCube()
    {
        for (int i = 0; i < levelSize; i++)
        {
            for (int j = 0; j < levelSize; j++)
            {
                if (buildWallArr[i, j] != null)
                {
                    //Debug.Log(buildWallArr[i, j].name);
                    if (buildWallArr[i, j].gameObject.tag == "left gold cube" || buildWallArr[i, j].gameObject.tag == "right gold cube")
                    {
                        if (buildWallArr[i, j] != null && buildWallArr[i, j].GetComponent<GoldCubeHalf>().currentZone != "BuildWall")
                        {
                            //Debug.Log("A cube was removed...");
                            buildWallArr[i, j] = null;
                            //PushCubesDown(i, j);
                            DeleteMirrodCube(i, j);
                        }
                    }
                    else
                    if (buildWallArr[i, j].gameObject.tag == "gold cube")
                    {
                        if (buildWallArr[i, j] != null && buildWallArr[i, j].GetComponent<GoldCubeWhole>().currentZone != "BuildWall")
                        {
                            //Debug.Log("A cube was removed...");
                            buildWallArr[i, j] = null;
                            //PushCubesDown(i, j);
                            DeleteMirrodCube(i, j);
                        }
                    }
                    else
                    if (buildWallArr[i, j].gameObject.tag == "invis cube" || buildWallArr[i, j].gameObject.tag == "red cube" || buildWallArr[i, j].gameObject.tag == "blue cube")
                    {
                        if (buildWallArr[i, j] != null && buildWallArr[i, j].GetComponent<XRGrabNetworkInteractable>().currentZone != "BuildWall")
                        {
                            //Debug.Log("A cube was removed...");
                            buildWallArr[i, j] = null;
                            //PushCubesDown(i, j);
                            DeleteMirrodCube(i, j);
                        }
                    }
                }
            }
        }
    }

    void PushCubesDown(int col, int row)
    {
        for (int i = row; i < levelSize; i++)
        {
            //Debug.Log("Col: " + col + ", Row: " + row + ", i: " + i);
            // Checks if the array is a cube
            if (i > 0 && i < buildWallArr.Length && buildWallArr[col, i] != null)
            {
                if (buildWallArr[col, i].gameObject.tag == "left gold cube" || buildWallArr[col, i].gameObject.tag == "right gold cube")
                {
                    buildWallArr[col, i].GetComponent<GoldCubeHalf>().buildWallTargetPos += buildWallArr[col, i].transform.up * -1;
                }
                if (buildWallArr[col, i].gameObject.tag == "gold cube")
                {
                    buildWallArr[col, i].GetComponent<GoldCubeWhole>().buildWallTargetPos += buildWallArr[col, i].transform.up * -1;
                }
                if (buildWallArr[col, i].gameObject.tag == "invis cube" || buildWallArr[col, i].gameObject.tag == "red cube" || buildWallArr[col, i].gameObject.tag == "blue cube")
                {
                    buildWallArr[col, i].GetComponent<XRGrabNetworkInteractable>().buildWallTargetPos += buildWallArr[col, i].transform.up * -1;
                }
                buildWallArr[col, i].GetComponent<XRGrabNetworkInteractable>().buildWallTargetPos += buildWallArr[col, i].transform.up * -1;
                buildWallArr[col, i - 1] = buildWallArr[col, i];
                buildWallArr[col, i] = null;
                
             }        
        }
    }

    void DeleteMirrodCube(int col, int row)
    {
        PushCubesDown(col, row);
        PhotonNetwork.Destroy(otherBuildWall.GetComponent<BuildWall>().buildWallArr[col, row]);
        otherBuildWall.GetComponent<BuildWall>().buildWallArr[col, row] = null;
        otherBuildWall.GetComponent<BuildWall>().PushCubesDown(col, row);
        Debug.Log("Deleteing a Mirroed Cube");
    }

    [PunRPC]
    public void FillArrayOverNetwork(int photonID, int col, int row)
    {
        GameObject obj =  PhotonNetwork.GetPhotonView(photonID).gameObject;
        buildWallArr[col, row] = obj;
    }
}