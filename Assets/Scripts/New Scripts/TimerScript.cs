using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerScript : Singleton<TimerScript>
{
    public float currentTime = 300; // in seconds
    public float startingTime = 300; // in seconds
    public bool record = false;
    bool paused = false;
    //bool timeUp = false;
    public TextMesh txt;
    float textTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        currentTime = startingTime;
        txt = GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        textTime -= Time.deltaTime;
        if (record)
        {

            if (!paused) currentTime -= Time.deltaTime;
            //timeUp = isTimeUp();
            if (textTime <= 0) DisplayTimer();
        }

    }

    void DisplayTimer()
    {
        if (isTimeUp())
        {
            //txt.text = "Time's Up!";
            DisplayText("Time's Up!", 5);
        }
        else
        {
            txt.text = TimeToString();
        }
    }

    public void ResetTime()
    {
        currentTime = startingTime;
    }

    public string TimeToString()
    {
        string mins = (Mathf.Floor(currentTime / 60)).ToString();
        //string secs = (Mathf.Floor(currentTime % 60)).ToString();
        string secs = string.Format("{0:00}", Mathf.Floor(currentTime % 60));
        return (mins + ":" + secs);
    }

    public bool isTimeUp()
    {
        if (currentTime <= 0)
        {
            GameManager.instance.Gameover();
            Debug.Log("Time's up!");
            return true;
        }
        else
        {
            return false;
        }
    }

    public void changeAllottedTime(float newTime)
    {
        startingTime = newTime;
    }

    public void DisplayText(string msg, float time)
    {
        textTime = time;
        txt.text = msg;
    }
}
