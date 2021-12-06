using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    /// <summary>
    /// This method is called by UI Button to load scene
    /// </summary>
    /// <param name="sceneName">Name of the scene in Build Setting</param>
    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName); 
    }

    
}
