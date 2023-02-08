using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

public class MyRayInteractor : XRRayInteractor
{
    public bool couldDelete = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CheckDelete(GameObject obj)
    {
        if (couldDelete)
        {
            PhotonNetwork.Destroy(obj);
            couldDelete = false;
        }
    }

    [PunRPC]

    public void DeleteObjsOverNetwork(GameObject obj1, GameObject obj2)
    {
        PhotonNetwork.Destroy(obj1);
        PhotonNetwork.Destroy(obj2);
    }

    protected override void OnSelectExited(XRBaseInteractable interactor)
    {
       // Debug.Log("ray select exited from " + interactor.gameObject.name);

        //if (GameManager.instance.host)
        //{
        //    Debug.Log("the ray is about to delete the cube");
        //    PhotonNetwork.Destroy(gameObject);
        //}
    }

    protected override void OnHoverExited(XRBaseInteractable obj)
    {

    }
    protected override void OnHoverEntered(XRBaseInteractable obj)
    {
        //Debug.Log(obj.gameObject.name);
    }
}
