using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

//  Trent Simmons 10/11/2021
// this script is attached to the prefab which is attached to the left and right hand GameObject in the VR rig.
// This scirpt is will find either the left or the right controllers depending on the "controllerCharacteristics" 
//  set the "controllerCharacteristics" to the side (left/right) of the hand model prefab attached in the inspector
//This script will spawn the speciffied handmodel
// Also updates the Animation of the hand that it is controlling



public class HandPresenceNew : MonoBehaviour
{
    public InputDeviceCharacteristics controllerCharacteristics;
    //public List<GameObject> controllerPrefabs;
    private InputDevice targetDevice;

    //private string rightControlerName = "OpenVR Controller(Vive Controller MV) - Left";
    //private string leftConrolerName = "OpenVR Controller(Vive Controller MV) - Left";

    public GameObject handModelPrefab;
    private GameObject spawnedHandModel;

    private Animator handAnimator;


    void Start()
    {
        TryInitialize();
    }

    void TryInitialize()
    {
        //this will store the controller variable handle in the devices list
        List<InputDevice> devices = new List<InputDevice>();
        //InputDeviceCharacteristics leftControllerCharacteristics = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

        //InputDeviceCharacteristics rightControllerCharacteristics = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        //InputDevices.GetDevicesWithCharacteristics(leftControllerCharacteristics, devices);

        foreach (var item in devices)
        {
            Debug.Log(item.name + item.characteristics);
            //if(item.name == rightControlerName)

        }

        if (devices.Count > 0)
        {
            targetDevice = devices[0];
            //GameObject prefab = controllerPrefabs.Find(controller => controller.name == targetDevice.name);
            spawnedHandModel = Instantiate(handModelPrefab, transform);
            handAnimator = spawnedHandModel.GetComponent<Animator>();
        }
    }

    void UpdateHandAnimation()
    {
        //Debug.Log("hi hi");
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handAnimator.SetFloat("Trigger", triggerValue);
            //Debug.Log("trigger value: " + triggerValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0f);
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0f);
        }
    }

    void Update()
    {
        if (!targetDevice.isValid)
        {
            TryInitialize();
        }
        else
        {
            UpdateHandAnimation();
        }
        //// finds bool for A and X button on cosmos controllers
        //targetDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool primaryButtonValue);
        //if (primaryButtonValue)
        //{
        //    Debug.Log("pressing primary button");
        //}

        //// finds float 0-1 for trigger value on cosmos controllers
        //targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue);
        //if (triggerValue> 0.1f)
        //{
        //    Debug.Log("pressing trigger "+triggerValue);
        //}

        //// finds float 0-1 for trigger value on cosmos controllers
        //targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxisValue);
        //if (primary2DAxisValue != Vector2.zero)
        //{
        //    Debug.Log("Touchpad " + primary2DAxisValue);
        //}



    }
}
