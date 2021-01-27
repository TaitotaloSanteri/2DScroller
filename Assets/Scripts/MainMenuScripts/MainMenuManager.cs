using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void OnNewGameButtonPressed()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void OnQuitGameButtonPressed()
    {
        Application.Quit();
    }
}
