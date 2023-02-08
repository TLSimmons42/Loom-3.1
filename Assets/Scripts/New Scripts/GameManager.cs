using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Doozy.Engine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;


public class GameManager : Singleton<GameManager>
{

    private float cubeDropTimer = 1f;

    public GameObject[] PlaywallDropPoints;
    public GameObject[] PlaywallEndPoints;

    public GameObject VRrig;
    public GameObject playerPos1;
    public GameObject playerPos2;
    public GameObject VRcamHeadPos;
    public GameObject eyeTracker;


    public GameObject BlueCube;
    public GameObject RedCube;
    public GameObject GoldCube;
    public GameObject NeutralCube;

    public GameObject NetworkBlueCube;
    public GameObject NetworkRedCube;
    public GameObject NetworkGoldCube;
    public GameObject NetworkNeutralCube;

    public GameObject buildWall1;
    public GameObject buildWall2;
    public GameObject ViewWall1;
    public GameObject ViewWall2;

    public UIView gameOverScreen;

    public UIView startView;
    public UIView waitingView;
    public UIView gameOverView;
    public UIView gameOverView2;


    public bool dropCubes = false;
    public bool dropNetworkCubes = false;
    public bool host = false;
    public bool allPlayersConnected = false;
    public bool holdingGoldHalf = false;
    public bool gameStarted = true;
    public bool gameOn = false;
    public bool canPickUpOffBuildWall = false;
    public bool eyeTracking = false;


    public TextAsset[] easyLevels;
    public TextAsset[] mediumLevels;
    public TextAsset[] hardLevels;

    private GameObject cube; // spawned cube

    List<GameObject> Players = new List<GameObject>();

    int gameDiff;
    string strGameDiff;
    public int playerCount;
    int playerDesignation;
    int numOfPlayersInGame;


    public string[,] targetWall;


    PhotonView PV;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        strGameDiff = PlayerPrefs.GetString("gameDifficulty");
        playerCount = PlayerPrefs.GetInt("playerCount");
        Debug.Log("the player count is: " + playerCount);

        ConvertGameDiffToInt(strGameDiff); //gets game difficulty 
        //MakeViewWall(); // detects # of players and spawns appropriate view walls
        if (playerCount == 2)
        {
            StartCoroutine(AssignHostAndPlayerPos()); // this will set the host and give access to the players gameobjects
        }
        canPickUpOffBuildWall = true;


    }
    
    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.Return) && gameStarted)
        {
            StartTheGame();
            gameStarted = false;
        }
        if (dropCubes)
        {
            StartCoroutine(BuildNewCube());
        }
        if (dropNetworkCubes)
        {
            StartCoroutine(BuildNewNetworkCube());
        }
        if (gameOn)
        {
            //Analytics.instance.WriteData4("HeadPos", "", "", VRcamHeadPos.transform.rotation.x.ToString(), VRcamHeadPos.transform.rotation.y.ToString(), VRcamHeadPos.transform.rotation.z.ToString(), VRcamHeadPos.transform.position.x.ToString(), VRcamHeadPos.transform.position.y.ToString(), VRcamHeadPos.transform.position.z.ToString());
        }

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


    // this will be called at the start of the game to build a the view wall for the player
    public void BuildViewWall()
    {
        GameObject[] spawnLocations = { ViewWall1, ViewWall2 };

        // Making the view wall depending on the difficulty
        for (int l = 0; l < spawnLocations.Length; l++)
        {
            for (int i = 0; i < targetWall.GetLength(0); i++)
            {
                for (int j = 0; j < targetWall.GetLength(1); j++)
                {
                   // Debug.Log("X:" + i + ", Y:" + j);
                   // Debug.Log(targetWall[i, j]);

                    Vector3 spawnLocation = spawnLocations[l].transform.position;
                    spawnLocation += spawnLocations[l].transform.right * -i;
                    spawnLocation += spawnLocations[l].transform.up * j;
                    GameObject cube = MasterBuildWall.instance.cubeCodeToGameObject(targetWall[i, j]);
                    GameObject spawnedCube = Instantiate(cube, spawnLocation, spawnLocations[l].transform.rotation);
                    //Destroy(spawnedCube.GetComponent<Rigidbody>());
                    spawnedCube.GetComponent<Cube>().enabled = false;
                    spawnedCube.GetComponent<BoxCollider>().enabled = false;

                    if (l == 0)
                    {
                        spawnedCube.transform.parent = ViewWall1.transform;
                    }
                    else
                    {
                        spawnedCube.transform.parent = ViewWall2.transform;
                    }
                }
            }
        }
    }

    // this will be called durring the game in order to build a new cube on the Play Wall
    public IEnumerator BuildNewCube()
    {
        if (dropCubes)
        {
            dropCubes = false;
            int spawnPointChoice = Random.Range(0, PlaywallDropPoints.Length);
            int cubeChoice = Random.Range(0, 4); // 4 = the number of cube choices


            if (cubeChoice == 0)
            {
                cube = Instantiate(RedCube, PlaywallDropPoints[spawnPointChoice].transform.position, Quaternion.identity);
            }
            if (cubeChoice == 1)
            {
                cube = Instantiate(BlueCube, PlaywallDropPoints[spawnPointChoice].transform.position, Quaternion.identity);
            }
            if (cubeChoice == 2)
            {
                cube = Instantiate(GoldCube, PlaywallDropPoints[spawnPointChoice].transform.position, Quaternion.identity);
            }
            if (cubeChoice == 3)
            {
                cube = Instantiate(NeutralCube, PlaywallDropPoints[spawnPointChoice].transform.position, Quaternion.identity);
            }
            cube.gameObject.GetComponent<Cube>().playWallTargetPos = PlaywallEndPoints[spawnPointChoice].transform.position;
            cube.GetComponent<Cube>().SetZoneToPlay();
            //for change
        }
        yield return new WaitForSeconds(cubeDropTimer);
        dropCubes = true;
    }

    public IEnumerator BuildNewNetworkCube()
    {
        if (host)
        {
            if (dropNetworkCubes)
            {
                dropNetworkCubes = false;
                int spawnPointChoice = Random.Range(0, PlaywallDropPoints.Length);
                int cubeChoice = Random.Range(0, 4); // 4 = the number of cube choices


                if (cubeChoice == 0)
                {
                    cube = PhotonNetwork.Instantiate("Network Red Cube", PlaywallDropPoints[spawnPointChoice].transform.position, Quaternion.identity);
                }
                if (cubeChoice == 1)
                {
                    cube = PhotonNetwork.Instantiate("Network Blue Cube", PlaywallDropPoints[spawnPointChoice].transform.position, Quaternion.identity);
                }
                if (cubeChoice == 2)
                {
                    cube = PhotonNetwork.Instantiate("Network Gold Cube", PlaywallDropPoints[spawnPointChoice].transform.position, Quaternion.identity);
                    //cube = PhotonNetwork.Instantiate("Gold Parent", PlaywallDropPoints[spawnPointChoice].transform.position, Quaternion.identity);
                    //cube.transform.GetChild(0).GetComponent<GoldCubeWhole>().currentZone = cube.transform.GetChild(0).GetComponent<GoldCubeWhole>().playWallZone;

                }
                if (cubeChoice == 3)
                {
                    cube = PhotonNetwork.Instantiate("Network Neutral Cube", PlaywallDropPoints[spawnPointChoice].transform.position, Quaternion.identity);
                }

                if (cube.gameObject.tag == "gold cube")
                {
                    cube.gameObject.GetComponent<GoldCubeWhole>().playWallTargetPos = PlaywallEndPoints[spawnPointChoice].transform.position;
                    cube.GetComponent<GoldCubeWhole>().currentZone = cube.GetComponent<GoldCubeWhole>().playWallZone;
                }
                else
                {
                    cube.gameObject.GetComponent<XRGrabNetworkInteractable>().playWallTargetPos = PlaywallEndPoints[spawnPointChoice].transform.position;
                    cube.GetComponent<XRGrabNetworkInteractable>().SetZoneToPlay();
                }
            }
            yield return new WaitForSeconds(cubeDropTimer);
            dropNetworkCubes = true;
        }
    }


    //This will detect the number of players and start the game accordingly
    public void StartTheGame()
    {
        eyeTracking = true;
        gameOn = true;
        //GetComponent<Analytics>().sessionTime = TimerScript.instance.currentTime.ToString();
        //Analytics.instance.WriteData("Game Start", "placeholder", TimerScript.instance.currentTime.ToString(),"","","");'
        Analytics.instance.writeEvent("Game Start", 3);
        //Analytics.instance.WriteData2("Game Start", "placeholder", TimerScript.instance.currentTime.ToString(), "", "", "");
        //Analytics.instance.WriteData3("Game Start", "placeholder", TimerScript.instance.currentTime.ToString(), "", "", "");

        TimerScript.instance.record = true;
        if (playerCount == 2)
        {
            MultiplayerStart();
        }
        else
        {
            SinglePlayerStart();
        }
    }

    public void SinglePlayerStart()
    {
        Debug.Log("single player start");
        buildWall2.SetActive(false);
        ViewWall1.SetActive(false);
        MasterBuildWall.instance.importLevel();
        dropCubes = true;
    }

    //This will be the Game Start Function for Multiplayer Player Mode
    public void MultiplayerStart()
    {
        Debug.Log("multiplayer start");
        MasterBuildWall.instance.importLevel();
        dropNetworkCubes = true;
    }
    public IEnumerator AssignHostAndPlayerPos()
    {
        yield return new WaitForSeconds(3);

        Debug.Log("detecting the number of players");
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Network Player"))
        {

            if (obj.name == "Network Player(Clone)")
            {
                Players.Add(obj);
                if (Players.Count == 1)
                {
                    host = true;
                    this.tag = "host";
                    Players[0].tag = "P1";
                    VRrig.tag = "P1";
                    VRrig.transform.position = playerPos2.transform.position;
                    Debug.Log("the host has been set");
                }
                else if (Players.Count == 2)
                {
                    host = false;
                    this.tag = "cliant";
                    Players[1].tag = "P2";
                    VRrig.tag = "P2";
                    VRrig.transform.position = playerPos1.transform.position;
                    Debug.Log("a client has been set");
                }
            }
        }
    }

    public void Gameover()
    {
        eyeTracking = false;
        gameOn = false;
        //Analytics.instance.WriteData("Game Start", "placeholder", TimerScript.instance.currentTime.ToString());
        TimerScript.instance.record = false;
        //Analytics.instance.WriteData("Game Over", "", "", "","","");
        Analytics.instance.writeEvent("Game Over", 3);
        //Analytics.instance.WriteData2("Game Over", "", "", "", "", "");
        //Analytics.instance.WriteData3("Game Over", "", "", "", "", "");
        //if (playerCount == 2)
        //{
        //    gameOverView.Show();
        //    gameOverView2.Show();
        //}
        //else
        //{
        //    gameOverView.Show();
        //}


        dropCubes = false;
        dropNetworkCubes = false;
        /*GoToMainMenu();*/
        //gameOverScreen.SetVisibility(true);
    }

    //tags the player gameobjects so that the cubes can identify them
    public void AssignPlayerTags()
    {
        Players[0].tag = "P1";
        Players[1].tag = "P2";
        VRrig.tag = "P1";
    }

    public void SubmitButton()
    {
        AudioManager.instance.Victory.Play();

        bool allCorrect = true;
        if(playerCount == 2)
        {
            MasterBuildWall.instance.updateMasterArray = true;
        }
        if (playerCount == 1  || playerCount == 2)
        {
            for (int x = 0; x < MasterBuildWall.instance.masterBuildArray.GetLength(0); x++)
            {
                for (int y = 0; y < MasterBuildWall.instance.masterBuildArray.GetLength(1); y++)
                {
                    //Debug.Log("master build wall: " + MasterBuildWall.instance.masterBuildArray[x, y]);
                    //Debug.Log("target build wall: " + MasterBuildWall.instance.targetWall[x, y]);
                    if (MasterBuildWall.instance.masterBuildArray[x, y] == MasterBuildWall.instance.targetWall[x, y])
                    {
                        Debug.Log("GOOD match!!");
                    }
                    else
                    {
                        allCorrect = false;
                        Debug.Log("not good match!!");
                    }
                }
            }
        }
        Debug.Log("This puzzle is all correct: " + allCorrect);
        
        Gameover();
    }

    public void CanPickUpOffBuildWall()
    {
        StartCoroutine(CanPickUpOffBuildwall());
    }

    IEnumerator CanPickUpOffBuildwall()
    {
        canPickUpOffBuildWall = false;
        yield return new WaitForSeconds(2);
        canPickUpOffBuildWall = true;

    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        if (playerCount == 2)
        {
            PV.RPC("BothPlayersLeaveRoom", RpcTarget.AllBuffered);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    [PunRPC]
    public void BothPlayersLeaveRoom()
    {
        SceneManager.LoadScene(0);
    }

}
