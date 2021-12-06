using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayLevelButton : MonoBehaviour
{
    
    /// <summary>
    /// Method is called by UI Button to replay the level
    /// </summary>
    public void Replay()
    {
        PlayerConfig.instance.SetCurrentLevel(PlayerConfig.instance.currentLevel);
    }

}
