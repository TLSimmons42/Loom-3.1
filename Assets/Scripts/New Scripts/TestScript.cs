using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class TestScript : MonoBehaviour
{

    private bool shoulLerp = false;

    public float timeStartedLearping;
    public float lerpTime;

    public Vector3 startPos;
    public Vector3 endPos;

    public float startNewLerpTime = 0;

    PhotonView photonView;
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        StartCoroutine(StartDelay());
    }

    // Update is called once per frame
    void Update()
    {
            if (shoulLerp)
            {
            //Analytics.instance.WriteData("Photon Test", "", Time.time.ToString(), gameObject.transform.position.x.ToString(), gameObject.transform.position.y.ToString(), gameObject.transform.position.z.ToString());
            Analytics.instance.writeEvent("Photon Test", 3);
            if (GameManager.instance.gameObject.tag == "host")
                {
                    photonView.RequestOwnership();
                    transform.position = Lerp(startPos, endPos, timeStartedLearping, lerpTime);
                    if (transform.position == endPos)
                    {
                        StartLerping();
                    }
                }
            }
        
    }

    private void StartLerping()
    {
        //Analytics.instance.WriteData("Cube start", "", Time.time.ToString(), gameObject.transform.position.x.ToString(), gameObject.transform.position.y.ToString(), gameObject.transform.position.z.ToString());
        Analytics.instance.writeEvent("Cube start", 3);
        Debug.Log("start lerp");
        int temp = Random.Range(1, 4);

        startPos = Vector3.zero;
        endPos = startPos;
        endPos.y = startPos.y + temp;
        shoulLerp = true;
        timeStartedLearping = Time.time;

    }

    public Vector3 Lerp(Vector3 start, Vector3 end, float timeStartedLerping, float lerpTime = 1)
    {
        float timeSinceStarted = Time.time - timeStartedLearping;

        float percentageComplete = timeSinceStarted / lerpTime;

        var result = Vector3.Lerp(start, end, percentageComplete);

        return result;
    }

    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(6);
        StartLerping();
    }
}
