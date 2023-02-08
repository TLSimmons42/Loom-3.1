using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void LaunchGame()
    {
        Debug.Log("Attemping to launch game");
        SceneManager.LoadScene("Integration Main");
    }
    public void ChangeToOnePlayer()
    {
        Debug.Log("Attemping to change players to one");
        PlayerPrefs.SetInt("playerCount", 1);
        //ChangeToPlayerOneChoice();
    }

    public void ChangeToTwoPlayer()
    {
        Debug.Log("Attemping to change players to two");
        PlayerPrefs.SetInt("playerCount", 2);
    }

    public void ChangeToEasy()
    {
        Debug.Log("Attemping to change difficulty to easy");
        PlayerPrefs.SetString("gameDifficulty", "easy");
    }
    public void ChangeToMedium()
    {
        Debug.Log("Attemping to change difficulty to medium");
        PlayerPrefs.SetString("gameDifficulty", "medium");
    }
    public void ChangeToHard()
    {
        Debug.Log("Attemping to change difficulty to hard");
        PlayerPrefs.SetString("gameDifficulty", "hard");
    }

    public void ChangeToPlayerOneChoice()
    {
        Debug.Log("Attemping to player choice to P1");
        PlayerPrefs.SetString("playerChoice", "P1");
    }

    public void ChangeToPlayerTwoChoice()
    {
        Debug.Log("Attemping to player choice to P2");
        PlayerPrefs.SetString("playerChoice", "P2");
    }

    public void ChangeToMouse()
    {
        Debug.Log("Attemping to head choice to mouse");
        PlayerPrefs.SetString("player head", "mouse");
    }

    public void ChangeToHuman()
    {
        Debug.Log("Attemping to head choice to human");
        PlayerPrefs.SetString("player head", "human");
    }

    public void ChangeToCat()
    {
        Debug.Log("Attemping to head choice to cat");
        PlayerPrefs.SetString("player head", "cat");
    }
}
