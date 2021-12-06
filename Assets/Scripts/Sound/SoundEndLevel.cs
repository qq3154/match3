using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEndLevel : MonoBehaviour
{
    [SerializeField] AudioCommand AudioWinLevel;
    [SerializeField] AudioCommand AudioLoseLevel;

    // Start is called before the first frame update
    void Start()
    {
        AudioCommand cmdToUse = (PlayerConfig.instance.isWinLevel) ? AudioWinLevel : AudioLoseLevel; 
        cmdToUse.Excute();
    }
    
}
