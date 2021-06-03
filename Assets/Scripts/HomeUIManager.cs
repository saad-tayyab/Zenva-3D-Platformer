using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeUIManager : MonoBehaviour
{
    // Start the game
    public void StartGame()
    {
        // Load the level 1
        SceneManager.LoadScene("Level1");
    }
}
