using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeNetworkProporties : MonoBehaviour
{


    public static CubeNetworkProporties CNP;

    public int mySelectedCube;


    public GameObject[] allCharecters;

    private void OnEnable()
    {
        if(CubeNetworkProporties.CNP == null)
        {
            CubeNetworkProporties.CNP = this;
        }
        else
        {
            if(CubeNetworkProporties.CNP != this)
            {
                Destroy(CubeNetworkProporties.CNP.gameObject);
                CubeNetworkProporties.CNP = this;
            }
        }

        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("MyCharacter"))
        {
            mySelectedCube = PlayerPrefs.GetInt("MyCharacter", mySelectedCube);
        }
        else
        {
            mySelectedCube = 0;
            PlayerPrefs.SetInt("MyCharacter", mySelectedCube);
        }
    }

}
