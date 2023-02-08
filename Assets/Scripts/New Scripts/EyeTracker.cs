using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveSR.anipal.Eye;

public class EyeTracker : MonoBehaviour
{
    //public AntiSaccadeGameManager manager;
    //public LaserEyeGameManager laserEyeManager;
    public SRanipal_GazeRaySample gazerayData;
    public ViveSR.anipal.Eye.VerboseData rightEye;
    public ViveSR.anipal.Eye.VerboseData leftEye;
    public ViveSR.anipal.Eye.SingleEyeData rightEyeData;
    public ViveSR.anipal.Eye.SingleEyeData leftEyeData;

    public float currentRightEyePupilSize;
    public float currentLeftEyePupilSize;

    public ViveSR.anipal.Eye.EyeData_v2 eye_data = new ViveSR.anipal.Eye.EyeData_v2();

    //new stuff
    public ViveSR.anipal.Eye.EyeData_v2 eyeData = new ViveSR.anipal.Eye.EyeData_v2();

    public float rightEyePupil_diameter;
    public float leftEyePupil_diameter;


    public VerboseData verboseData;


    private Vector3[] LRpoints = new Vector3[2];
    public GameObject endCollider;
    LineRenderer LR;
    Vector3 LRstartPos;
    Vector3 LRendPos;

    Vector3 SRCombinedPoint;
    Vector3 gazePoint;
    Ray gazeRay;
    public RaycastHit hit2;
    public RaycastHit hit;

    public GameObject gazeTarget;
    public GameObject environmentGazeTarget;
    // Start is called before the first frame update
    void Start()
    {
        
        gazerayData = gazerayData.GetComponent<SRanipal_GazeRaySample>();
        rightEyeData = rightEye.right;
        leftEyeData = leftEye.left;
        LR = gazerayData.GetComponent<LineRenderer>();



        //ViveSR.anipal.Eye.SRanipal_Eye_API.GetEyeData(ref eye_data);

        //ViveSR.anipal.Eye.SRanipal_Eye_v2.GetVerboseData(out vive.VerboseData verbose_data);

        //pupil_diameter_L = verbose_data.left.pupil_diameter_mm;    // read pupil diameter of left eye.

    }
    public RaycastHit hit1;
    
    // Update is called once per frame
    void Update()
    {
        //SRanipal_Eye_API(ref data);
        currentRightEyePupilSize = rightEyeData.pupil_diameter_mm;
        currentLeftEyePupilSize = leftEyeData.pupil_diameter_mm;
        SRanipal_Eye.GetVerboseData(out verboseData);
        leftEyePupil_diameter = verboseData.left.pupil_diameter_mm;
        rightEyePupil_diameter = verboseData.right.pupil_diameter_mm;

        //Analytics.instance.WriteData3("RightPupil", "", "", leftEyePupil_diameter.ToString(), rightEyePupil_diameter.ToString(), "");

        // Debug.Log("Left eye size: " + pupil_diameter);

        //LR.GetPositions(LRpoints);
        //LRstartPos = LRpoints[LRpoints.Length - 2];
        //LRendPos = LRpoints[LRpoints.Length - 1];
        //Debug.Log("start: " + LRstartPos + "   " + "end: " + LRendPos);
        //gazeRay = new Ray(LRstartPos, LRendPos);
        //Debug.DrawRay(LRstartPos, LRendPos, Color.yellow);


        SRCombinedPoint = Camera.main.transform.position - Camera.main.transform.up * 0.05f;
        gazePoint = gazerayData.GazeDirectionCombined * gazerayData.LengthOfRay;
        gazeRay = new Ray(SRCombinedPoint, gazePoint);
        Ray gazeRay2 = new Ray(SRCombinedPoint, gazePoint);
        int layerMask = ~LayerMask.GetMask("walls");
        //Debug.Log("start: " + SRCombinedPoint + "   " + "end: " + gazerayData.GazeDirectionCombined * gazerayData.LengthOfRay);
        Debug.DrawRay(SRCombinedPoint, gazePoint, Color.yellow);


        if (GameManager.instance.eyeTracking)
        {
            if (Physics.Raycast(gazeRay, out hit1, 10000, layerMask))
            {
                //Debug.Log(hit1.transform.name);
                if (hit1.transform.GetComponent<GazeTarget>() != null) //need to have the target class
                {
                    string type = hit1.transform.GetComponent<GazeTarget>().targetType;
                    Debug.Log(type);
                    //EventManager.instance.FixationOnObject.Invoke(type);
                    if (type == "blue cube")
                    {
                        Debug.Log("hit a blue cube gen");
                        if (GameManager.instance.playerCount == 2)
                        {
                            if (hit1.transform.GetComponent<XRGrabNetworkInteractable>().currentZone == hit1.transform.GetComponent<XRGrabNetworkInteractable>().playWallZone)
                            {
                                //Analytics.instance.WriteData("looking at blue cube", "", "", hit1.point.x.ToString(), hit1.point.y.ToString(), hit1.point.z.ToString());
                                Analytics.instance.writeEvent("looking at blue cube");
                                Debug.Log("looking at: " + type.ToString());
                            }
                        }
                        else
                        {
                            Debug.Log("looking at blue cube before play wall");
                            if (hit1.transform.GetComponent<Cube>().currentZone == hit1.transform.GetComponent<Cube>().playWallZone)
                            {
                                //Analytics.instance.WriteData("looking at blue cube", "", "", hit1.point.x.ToString(), hit1.point.y.ToString(), hit1.point.z.ToString());
                                Analytics.instance.writeEvent("looking at blue cube");
                                Debug.Log("looking at: " + type.ToString());
                            }
                        }
                    }
                    if (type == "red cube")
                    {
                        if (GameManager.instance.playerCount == 2)
                        {
                            if (hit1.transform.GetComponent<XRGrabNetworkInteractable>().currentZone == hit1.transform.GetComponent<XRGrabNetworkInteractable>().playWallZone)
                            {
                                //Analytics.instance.WriteData("looking at red cube", "", "", hit1.point.x.ToString(), hit1.point.y.ToString(), hit1.point.z.ToString());
                                Analytics.instance.writeEvent("looking at red cube");
                                Debug.Log("looking at: " + type.ToString());
                            }
                        }
                        else
                        {
                            if (hit1.transform.GetComponent<Cube>().currentZone == hit1.transform.GetComponent<Cube>().playWallZone)
                            {
                                //Analytics.instance.WriteData("looking at red cube", "", "", hit1.point.x.ToString(), hit1.point.y.ToString(), hit1.point.z.ToString());
                                Analytics.instance.writeEvent("looking at red cube");
                                Debug.Log("looking at: " + type.ToString());
                            }
                        }
                    }
                    if (type == "invis cube")
                    {
                        if (GameManager.instance.playerCount == 2)
                        {
                            if (hit1.transform.GetComponent<XRGrabNetworkInteractable>().currentZone == hit1.transform.GetComponent<XRGrabNetworkInteractable>().playWallZone)
                            {
                                //Analytics.instance.WriteData("looking at invis cube", "", "", hit1.point.x.ToString(), hit1.point.y.ToString(), hit1.point.z.ToString());
                                Analytics.instance.writeEvent("looking at invis cube");
                                Debug.Log("looking at: " + type.ToString());
                            }
                        }
                        else
                        {
                            if (hit1.transform.GetComponent<Cube>().currentZone == hit1.transform.GetComponent<Cube>().playWallZone)
                            {
                                //Analytics.instance.WriteData("looking at invis cube", "", "", hit1.point.x.ToString(), hit1.point.y.ToString(), hit1.point.z.ToString());
                                Analytics.instance.writeEvent("looking at invis cube");
                                Debug.Log("looking at: " + type.ToString());
                            }
                        }
                    }
                    if (type == "gold cube")
                    {
                        if (GameManager.instance.playerCount == 2)
                        {
                            if (hit1.transform.GetComponent<GoldCubeWhole>().currentZone == hit1.transform.GetComponent<GoldCubeWhole>().playWallZone)
                            {
                                //Analytics.instance.WriteData("looking at gold cube", "", "", hit1.point.x.ToString(), hit1.point.y.ToString(), hit1.point.z.ToString());
                                Analytics.instance.writeEvent("looking at gold cube");
                                Debug.Log("looking at: " + type.ToString());
                            }
                        }
                        else
                        {
                            if (hit1.transform.GetComponent<Cube>().currentZone == hit1.transform.GetComponent<Cube>().playWallZone)
                            {
                                //Analytics.instance.WriteData("looking at gold cube", "", "", hit1.point.x.ToString(), hit1.point.y.ToString(), hit1.point.z.ToString());
                                Analytics.instance.writeEvent("looking at gold cube");
                                Debug.Log("looking at: " + type.ToString());
                            }
                        }
                    }
                    if (type == "DropZoneEye")
                    {

                        //Analytics.instance.WriteData("looking at Drop Zone", "", "", hit1.point.x.ToString(), hit1.point.y.ToString(), hit1.point.z.ToString());
                        Analytics.instance.writeEvent("looking at Drop Zone");
                        Debug.Log("looking at: " + type.ToString());

                    }
                    //if(type == "View Wall")
                    //{
                    //    Analytics.instance.WriteData2("looking at View wall", "", "", hit1.transform.position.x.ToString(), hit1.transform.position.y.ToString(), hit1.transform.position.z.ToString());
                    //    Debug.Log("looking at: " + type.ToString());
                    //}
                    //if (type == "Play Wall")
                    //{
                    //    Analytics.instance.WriteData2("looking at Play wall", "", "", hit1.transform.position.x.ToString(), hit1.transform.position.y.ToString(), hit1.transform.position.z.ToString());
                    //    Debug.Log("looking at: " + type.ToString());
                    //}
                    //if (type == "Build Wall")
                    //{
                    //    Analytics.instance.WriteData2("looking at Build wall", "", "", hit1.transform.position.x.ToString(), hit1.transform.position.y.ToString(), hit1.transform.position.z.ToString());
                    //    Debug.Log("looking at: " + type.ToString());
                    //}
                }
            }
        }
        if (Physics.Raycast(gazeRay2, out hit2, 10000))
        {
            if (GameManager.instance.eyeTracking)
            {
                if (hit2.transform.GetComponent<GazeTarget>() != null) //need to have the target class
                {
                    string type = hit2.transform.GetComponent<GazeTarget>().targetType;
                    if (type == "View Wall")
                    {
                        //Analytics.instance.WriteData2("looking at View wall", "", "", hit2.point.x.ToString(), hit2.point.y.ToString(), hit2.point.z.ToString());
                        Analytics.instance.writeEvent("looking at View wall");
                        //Debug.Log("looking at: " + type.ToString());
                    }
                    if (type == "Play Wall")
                    {
                        //Analytics.instance.WriteData2("looking at Play wall", "", "", hit2.point.x.ToString(), hit2.point.y.ToString(), hit2.point.z.ToString());
                        Analytics.instance.writeEvent("looking at Play wall");
                        //Debug.Log("looking at: " + type.ToString());
                    }
                    if (type == "Build Wall")
                    {
                        //Analytics.instance.WriteData2("looking at Build wall", "", "", hit2.point.x.ToString(), hit2.point.y.ToString(), hit2.point.z.ToString());
                        Analytics.instance.writeEvent("looking at Build wall");
                        //Debug.Log("looking at: " + type.ToString());
                    }
                    if (type == "Background Wall")
                    {
                        //Analytics.instance.WriteData2("looking at Background wall", "", "", hit2.point.x.ToString(), hit2.point.y.ToString(), hit2.point.z.ToString());
                        Analytics.instance.writeEvent("looking at Background wall");
                        //Debug.Log("looking at: " + type.ToString());
                    }
                }
            }
        }
        
        


    }
}
