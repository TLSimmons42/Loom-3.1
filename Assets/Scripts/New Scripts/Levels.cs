using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Array2DEditor;

public class Levels : Singleton<Levels>
{
    List<Array2DString> easyLevels = new List<Array2DString>();
    [Header("Easy Levels")]

    [SerializeField]
    public Array2DString easy1;
    [SerializeField]
    public Array2DString easy2;
    [SerializeField]
    public Array2DString easy3;
    [SerializeField]
    public Array2DString easy4;
    [SerializeField]
    public Array2DString easy5;
    [SerializeField]
    public Array2DString easy6;



    List<Array2DString> mediumLevels = new List<Array2DString>();
    [Header("Medium Levels")]

    [SerializeField]
    public Array2DString medium1;
    [SerializeField]
    public Array2DString medium2;
    [SerializeField]
    public Array2DString medium3;
    [SerializeField]
    public Array2DString medium4;
    [SerializeField]
    public Array2DString medium5;
    [SerializeField]
    public Array2DString medium6;

    List<Array2DString> hardLevels = new List<Array2DString>();
    [Header("Hard Levels")]

    [SerializeField]
    public Array2DString hard1;
    [SerializeField]
    public Array2DString hard2;
    [SerializeField]
    public Array2DString hard3;
    [SerializeField]
    public Array2DString hard4;
    [SerializeField]
    public Array2DString hard5;
    [SerializeField]
    public Array2DString hard6;

    public void Awake()
    {
        initializeLevels();
    }

    public void initializeLevels()
    {
        //easyLevels.Add(easy1);
        easyLevels.Add(easy2);
        easyLevels.Add(easy3);
        easyLevels.Add(easy4);
        easyLevels.Add(easy5);

        mediumLevels.Add(medium1);
        mediumLevels.Add(medium2);
        mediumLevels.Add(medium3);
        mediumLevels.Add(medium4);
        mediumLevels.Add(medium5);
        mediumLevels.Add(medium6);

        hardLevels.Add(hard1);
        hardLevels.Add(hard2);
        hardLevels.Add(hard3);
        hardLevels.Add(hard4);
        hardLevels.Add(hard5);
        hardLevels.Add(hard6);


    }

    public string[,] getRandomLevel(string difficulty)
    {
        //initializeLevels();
        switch (difficulty) 
        {
            case "easy":
                return easyLevels[Random.Range(0, easyLevels.Count)].GetCells();
            case "medium":
                return mediumLevels[Random.Range(0, mediumLevels.Count)].GetCells();
            case "hard":
                return hardLevels[Random.Range(0, hardLevels.Count)].GetCells();
            default:
                return easyLevels[Random.Range(0, easyLevels.Count)].GetCells();
        }
    }

    public int getRandomIndex(string difficulty)
    {
        switch (difficulty)
        {
            case "easy":
                return Random.Range(0, easyLevels.Count);
            case "medium":
                return Random.Range(0, mediumLevels.Count);
            case "hard":
                return Random.Range(0, hardLevels.Count);
            default:
                return Random.Range(0, easyLevels.Count);
        }
    }

    public string[,] getLevelFromIndex(string difficulty, int index)
    {
        switch (difficulty)
        {
            case "easy":
                return easyLevels[index].GetCells();
            case "medium":
                return mediumLevels[index].GetCells();
            case "hard":
                return hardLevels[index].GetCells();
            default:
                return easyLevels[index].GetCells();
        }
    }
}
