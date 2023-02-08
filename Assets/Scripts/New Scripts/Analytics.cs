using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using System.Diagnostics.Eventing.Reader;

public class Analytics : Singleton<Analytics>
{
    string savePath;
    string filePath;
    string csvPath;
    string filePath2;
    string csvPath2;
    string filePath3;
    string csvPath3;
    string filePath4;
    string csvPath4;

    public InputField partID;
    public InputField partAge;
    public InputField partGender;
    public InputField condition;
    public InputField trial;
    public InputField group;
    public string sessionTime;
    public GameObject VRcamHeadPos;
    public GameObject rightHand;

    GameObject eyeTracker;



    // Start is called before the first frame update
    void Start()
    {
        sessionTime = TimerScript.instance.currentTime.ToString();

        eyeTracker = GameObject.FindGameObjectWithTag("eye tracker");



        savePath = Application.dataPath + "/Analytics";
        filePath = Application.dataPath + "/Analytics/analytics.json";
        csvPath = Application.dataPath + "/Analytics/analytics.csv";
        filePath2 = Application.dataPath + "/Analytics/analytics2.json";
        csvPath2 = Application.dataPath + "/Analytics/analytics2.csv";
        filePath3 = Application.dataPath + "/Analytics/analytics3.json";
        csvPath3 = Application.dataPath + "/Analytics/analytics3.csv";
        filePath4 = Application.dataPath + "/Analytics/analytics4.json";
        csvPath4 = Application.dataPath + "/Analytics/analytics4.csv";

        if (!File.Exists(savePath))
        {
            Debug.Log("making new analytics forlder");
            Directory.CreateDirectory(savePath);
        }
        try 
        {
            if(!File.Exists(csvPath))
            File.WriteAllText(csvPath, "TimeStamp,participant,Condition,Tiral,Age,Gender,SessionTime,Event, eyePosX, eyePosY, eyePosZ, headPosX, HeadPosY, HeadPosZ, HeadRotX, HeadRotY, HeadRotZ, HandPosX, HandPosY, HandPosZ, HandRotX, HandRotY, HandRotZ, currentGazeTarget, EnvironmentGazeTarget, Handedness, RightPupil, LeftPupil, Group");
        } catch (Exception e)
        {
            Debug.LogException(e);
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (GameManager.instance.eyeTracking)
        {
            DataPoint data = new DataPoint();
            data.timestamp = DateTime.Now.Ticks.ToString();
            data.participant = PlayerPrefs.GetString("ParticipantID");
            data.task = PlayerPrefs.GetString("ParticipantCondition");
            data.trial = PlayerPrefs.GetString("trial");
            data.age = PlayerPrefs.GetString("ParticipantAge").ToString();
            data.gender = PlayerPrefs.GetString("ParticipantGender");
            data.group = PlayerPrefs.GetString("group");
            // Handedness is Right by default
            data.handedness = "Right";
            /*data.headPosX = */
            try
            {
                data.eyePosX = eyeTracker.GetComponent<EyeTracker>().hit2.point.x.ToString();
                data.eyePosY = eyeTracker.GetComponent<EyeTracker>().hit2.point.y.ToString();
                data.eyePosZ = eyeTracker.GetComponent<EyeTracker>().hit2.point.z.ToString();
                data.headPosX = VRcamHeadPos.transform.position.x.ToString();
                data.headPosY = VRcamHeadPos.transform.position.y.ToString();
                data.headPosZ = VRcamHeadPos.transform.position.z.ToString();
                data.headRotX = VRcamHeadPos.transform.rotation.x.ToString();
                data.headRotY = VRcamHeadPos.transform.rotation.y.ToString();
                data.headRotZ = VRcamHeadPos.transform.rotation.z.ToString();
                data.handPosX = rightHand.transform.position.x.ToString();
                data.handPosY = rightHand.transform.position.y.ToString();
                data.handPosZ = rightHand.transform.position.z.ToString();
                data.handRotX = rightHand.transform.rotation.x.ToString();
                data.handRotY = rightHand.transform.rotation.y.ToString();
                data.handRotZ = rightHand.transform.rotation.z.ToString();
                data.rigthPupil = eyeTracker.GetComponent<EyeTracker>().rightEyePupil_diameter.ToString();
                data.leftPupil = eyeTracker.GetComponent<EyeTracker>().leftEyePupil_diameter.ToString();
                string csvstring = String.Join(",", GetDataArray(data));
                File.AppendAllText(csvPath, "\n");
                File.AppendAllText(csvPath, csvstring);
            } catch
            {

            }
        }
    }

    public void writeEvent(string eventString, int hit_wall_1=1)
    {
        DataPoint data = new DataPoint();
        data.timestamp = DateTime.Now.Ticks.ToString();
        data.participant = PlayerPrefs.GetString("ParticipantID");
        data.task = PlayerPrefs.GetString("ParticipantCondition");
        data.trial = PlayerPrefs.GetString("trial");
        data.age = PlayerPrefs.GetString("ParticipantAge").ToString();
        data.gender = PlayerPrefs.GetString("ParticipantGender");
        data.group = PlayerPrefs.GetString("group");
        // Handedness is Right by default
        data.handedness = "Right";
        /*data.headPosX = */
        if(hit_wall_1 == 1)
        {
            
            try
            {
                data.eyePosX = eyeTracker.GetComponent<EyeTracker>().hit1.point.x.ToString();
                data.eyePosY = eyeTracker.GetComponent<EyeTracker>().hit1.point.y.ToString();
                data.eyePosZ = eyeTracker.GetComponent<EyeTracker>().hit1.point.z.ToString();
                data.currentGazeTraget = eventString;
            } catch(Exception e)
            {
                data.currentGazeTraget = "";
            }
        } else if (hit_wall_1 == 2)
        {
            
            try
            {
                data.eyePosX = eyeTracker.GetComponent<EyeTracker>().hit2.point.x.ToString();
                data.eyePosY = eyeTracker.GetComponent<EyeTracker>().hit2.point.y.ToString();
                data.eyePosZ = eyeTracker.GetComponent<EyeTracker>().hit2.point.z.ToString();
                data.environmentGazeTarget = eventString;
            }
            catch (Exception e)
            {
                data.environmentGazeTarget = "";
            }
        } else
        {
            
            try
            {
                data.eyePosX = eyeTracker.GetComponent<EyeTracker>().hit2.point.x.ToString();
                data.eyePosY = eyeTracker.GetComponent<EyeTracker>().hit2.point.y.ToString();
                data.eyePosZ = eyeTracker.GetComponent<EyeTracker>().hit2.point.z.ToString();
                data.eventName = eventString;
            }
            catch (Exception e)
            {
                data.eventName = "";
            }
        }
        try
        {
            data.headPosX = VRcamHeadPos.transform.position.x.ToString();
            data.headPosY = VRcamHeadPos.transform.position.y.ToString();
            data.headPosZ = VRcamHeadPos.transform.position.z.ToString();
            data.headRotX = VRcamHeadPos.transform.rotation.x.ToString();
            data.headRotY = VRcamHeadPos.transform.rotation.y.ToString();
            data.headRotZ = VRcamHeadPos.transform.rotation.z.ToString();
            data.handPosX = rightHand.transform.position.x.ToString();
            data.handPosY = rightHand.transform.position.y.ToString();
            data.handPosZ = rightHand.transform.position.z.ToString();
            data.handRotX = rightHand.transform.rotation.x.ToString();
            data.handRotY = rightHand.transform.rotation.y.ToString();
            data.handRotZ = rightHand.transform.rotation.z.ToString();
            data.rigthPupil = eyeTracker.GetComponent<EyeTracker>().rightEyePupil_diameter.ToString();
            data.leftPupil = eyeTracker.GetComponent<EyeTracker>().leftEyePupil_diameter.ToString();
        } catch
        {

        }

        string csvstring = String.Join(",", GetDataArray(data));
        File.AppendAllText(csvPath, "\n");
        File.AppendAllText(csvPath, csvstring);
    }

    public void WriteData(string eventString, string participant, string sessionTime, string testX, string testY, string testZ)
    {
        DataPoint data = new DataPoint();
        data.timestamp = DateTime.Now.Ticks.ToString();
        data.participant = PlayerPrefs.GetString("ParticipantID");
        data.task = PlayerPrefs.GetString("ParticipantCondition");
        data.trial = PlayerPrefs.GetString("trial");
        data.age = PlayerPrefs.GetString("ParticipantAge").ToString();
        data.gender = PlayerPrefs.GetString("ParticipantGender");
        data.sessionTime = sessionTime;
        data.eventName = eventString;
/*        data.testX = testX;
        data.testY = testY;
        data.testZ = testZ;*/
        data.group = PlayerPrefs.GetString("group");


        string jsonString = JsonUtility.ToJson(data);
        string csvstring = String.Join(",", GetDataArray(data));
        //File.AppendAllText(filePath, "\n");
        if (!File.Exists(csvPath))
        {
            File.WriteAllText(csvPath, "TimeStamp,participant,Condition,Tiral,Age,Gender,SessionTime,Event, eyePosX, eyePosY, eyePosZ, Group");
        }
        File.AppendAllText(csvPath, "\n");
        File.AppendAllText(filePath, jsonString);
        File.AppendAllText(csvPath, csvstring);
    }
    public void WriteData2(string eventString, string participant, string sessionTime, string testX, string testY, string testZ)
    {
        DataPoint data = new DataPoint();
        data.timestamp = DateTime.Now.Ticks.ToString();
        data.participant = PlayerPrefs.GetString("ParticipantID");
        data.task = PlayerPrefs.GetString("ParticipantCondition");
        data.trial = PlayerPrefs.GetString("trial");
        data.age = PlayerPrefs.GetString("ParticipantAge").ToString();
        data.gender = PlayerPrefs.GetString("ParticipantGender");
        data.sessionTime = sessionTime;
        data.eventName = eventString;
/*        data.testX = testX;
        data.testY = testY;
        data.testZ = testZ;*/
        data.group = PlayerPrefs.GetString("group");



        string jsonString = JsonUtility.ToJson(data);
        string csvstring = String.Join(",", GetDataArray(data));
        //File.AppendAllText(filePath, "\n");
        if (!File.Exists(csvPath2))
        {
            File.WriteAllText(csvPath2, "TimeStamp,participant,Condition,Tiral,Age,Gender,SessionTime,Event, xPos, yPos, zPos, Group");
        }
        File.AppendAllText(csvPath2, "\n");
        File.AppendAllText(filePath2, jsonString);
        File.AppendAllText(csvPath2, csvstring);
    }

    public void WriteData3(string eventString, string participant, string sessionTime, string testX, string testY, string testZ)
    {
        DataPoint data = new DataPoint();
        data.timestamp = DateTime.Now.Ticks.ToString();
        data.participant = PlayerPrefs.GetString("ParticipantID");
        data.task = PlayerPrefs.GetString("ParticipantCondition");
        data.trial = PlayerPrefs.GetString("trial");
        data.age = PlayerPrefs.GetString("ParticipantAge").ToString();
        data.gender = PlayerPrefs.GetString("ParticipantGender");
        data.sessionTime = sessionTime;
        data.eventName = eventString;
        /*data.testX = testX;
        data.testY = testY;
        data.testZ = testZ;*/
        data.group = PlayerPrefs.GetString("group");



        string jsonString = JsonUtility.ToJson(data);
        string csvstring = String.Join(",", GetDataArray(data));
        //File.AppendAllText(filePath, "\n");
        if (!File.Exists(csvPath3))
        {
            File.WriteAllText(csvPath3, "TimeStamp,participant,Condition,Tiral,Age,Gender,SessionTime,Event, rightEye, leftEye, zPos, Group");
        }
        File.AppendAllText(csvPath3, "\n");
        File.AppendAllText(filePath3, jsonString);
        File.AppendAllText(csvPath3, csvstring);
    }
    public void WriteData4(string eventString, string participant, string sessionTime, string xRow, string yRow, string zRow, string xPos, string yPos, string zPos)
    {
        DataPoint data = new DataPoint();
        data.timestamp = DateTime.Now.Ticks.ToString();
        data.participant = PlayerPrefs.GetString("ParticipantID");
        data.task = PlayerPrefs.GetString("ParticipantCondition");
        data.trial = PlayerPrefs.GetString("trial");
        data.age = PlayerPrefs.GetString("ParticipantAge").ToString();
        data.gender = PlayerPrefs.GetString("ParticipantGender");
        data.sessionTime = sessionTime;
        data.eventName = eventString;
        /*data.testX = xRow;
        data.testY = yRow;
        data.testZ = zRow;
        data.x = xPos;
        data.y = yPos;
        data.z = zPos;*/
        data.group = PlayerPrefs.GetString("group");


        string jsonString = JsonUtility.ToJson(data);
        string csvstring = String.Join(",", GetDataArray(data));
        //File.AppendAllText(filePath, "\n");
        if (!File.Exists(csvPath4))
        {
            File.WriteAllText(csvPath4, "TimeStamp,participant,Condition,Tiral,Age,Gender,SessionTime,Event, xRotation, yRotation, zRotation, xPos, yPos, Zpos, Group");
        }
        File.AppendAllText(csvPath4, "\n");
        File.AppendAllText(filePath4, jsonString);
        File.AppendAllText(csvPath4, csvstring);
    }

    string[] GetDataArray(DataPoint data)
    {
        List<string> stringlist = new List<string>();
        stringlist.Add(data.timestamp);
        stringlist.Add(data.participant);
        stringlist.Add(data.task);
        stringlist.Add(data.trial);
        stringlist.Add(data.age);
        stringlist.Add(data.gender);
        stringlist.Add(data.sessionTime);
        stringlist.Add(data.eventName);
        stringlist.Add(data.eyePosX);
        stringlist.Add(data.eyePosY);
        stringlist.Add(data.eyePosZ);
        stringlist.Add(data.headPosX);
        stringlist.Add(data.headPosY);
        stringlist.Add(data.headPosZ);
        stringlist.Add(data.headRotX);
        stringlist.Add(data.headRotY);
        stringlist.Add(data.headRotZ);
        stringlist.Add(data.handPosX);
        stringlist.Add(data.handPosY);
        stringlist.Add(data.handPosZ);
        stringlist.Add(data.handRotX);
        stringlist.Add(data.handRotY);
        stringlist.Add(data.handRotZ);
        stringlist.Add(data.currentGazeTraget);
        stringlist.Add(data.environmentGazeTarget);
        stringlist.Add(data.handedness);
        stringlist.Add(data.rigthPupil);
        stringlist.Add(data.leftPupil);
        stringlist.Add(data.group);

        return stringlist.ToArray();
    }

    public void SetParticipantID(string id)
    {
        PlayerPrefs.SetString("ParticipantID", id);
    }
    public void SetParticipantAge(string age)
    {
        PlayerPrefs.SetString("ParticipantAge", age);
    }
    public void SetParticipantCondition(string con)
    {
        PlayerPrefs.SetString("ParticipantCondition", con);
    }
    public void Settrial(string trial)
    {
        PlayerPrefs.SetString("trial", trial);
    }
    public void SetParticipantGender(string gen)
    {
        PlayerPrefs.SetString("ParticipantGender", gen);
    }
    public void SetParticipantGroup(string group)
    {
        PlayerPrefs.SetString("group", group);
    }


    public void SetPartVariables()
    {
        SetParticipantID(partID.text);
        SetParticipantAge(partAge.text);
        SetParticipantCondition(condition.text);
        SetParticipantGender(partGender.text);
        Settrial(trial.text);
        SetParticipantGroup(group.text);
    }


    public class DataPoint
    {
        public string timestamp;
        public string participant;
        public string task;
        public string trial;
        public string age;
        public string gender;
        public string sessionTime;
        public string eventName;
        /*public string testX;
        public string testY;
        public string testZ;*/
        public string eyePosX;
        public string eyePosY;
        public string eyePosZ;
        public string headPosX;
        public string headPosY;
        public string headPosZ;
        public string headRotX;
        public string headRotY;
        public string headRotZ;
        public string handPosX;
        public string handPosY;
        public string handPosZ;
        public string handRotX;
        public string handRotY;
        public string handRotZ;
        /*public string x;
        public string y;
        public string z;*/
        public string currentGazeTraget;
        public string environmentGazeTarget;
        public string handedness;
        public string rigthPupil;
        public string leftPupil;
        public string group;
    }
}
