using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Method is called by UI Button to start the level by its number    
    /// </summary>
    public void SetGamePlayLevel()
    {
        string strLevel = this.transform.GetChild(0).gameObject.GetComponent<Text>().text;
        int intLevel = Int32.Parse(strLevel);
                
        PlayerConfig.instance.SetCurrentLevel(intLevel);
    }

    
}
