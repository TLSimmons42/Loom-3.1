using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjSpawner : MonoBehaviour
{


    public Vector3 spawnPos;
    public GameObject spawnCubePrefab;
    // Start is called before the first frame update
    void Start()
    {
        MakeNewCube();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void MakeNewCube()
    {
        Instantiate(spawnCubePrefab, spawnPos, Quaternion.identity);
    }
}
