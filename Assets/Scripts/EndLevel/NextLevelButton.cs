using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelButton : MonoBehaviour
{    
    /// <summary>
    /// Method is called by UI Button to start the next level
    /// Only available when player pass the current level
    /// </summary>
    public void NextLevel()
    {       
        PlayerConfig.instance.SetCurrentLevel(PlayerConfig.instance.currentLevel + 1);

    }
}
