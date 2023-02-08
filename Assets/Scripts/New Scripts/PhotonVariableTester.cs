using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonVariableTester : MonoBehaviour
{
    private PhotonView PV;
    private CNPcontroller cont;
    public GameObject myChar;
    public int charValue;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        cont = GetComponent<CNPcontroller>();
        //if (PV.IsMine)
        //{
        //    PV.RPC("RPC_TestFunction", RpcTarget.AllBuffered, CubeNetworkProporties.CNP.mySelectedCube);
        //}
    }

    [PunRPC]
    void RPC_TestFunction(int num)
    {
        charValue = num;
        myChar = Instantiate(CubeNetworkProporties.CNP.allCharecters[num], transform.position, transform.rotation, transform);
    }

    [PunRPC]
    public void ChangeVar()
    {
        if (cont.isOn)
        {
            cont.isOn = false;
        }
        else
        {
            cont.isOn = true;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            PV.RPC("ChangeVar", RpcTarget.AllBuffered);
            //ChangeVar();
            Debug.Log("space bar was pressed");
        }
    }

}
