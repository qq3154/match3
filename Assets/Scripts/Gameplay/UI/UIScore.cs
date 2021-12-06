using System.Collections;
using System.Collections.Generic;
using Observer;
using UnityEngine;
using UnityEngine.UI;

public class UIScore : MonoBehaviour
{    
    private void Awake()
    {
        this.RegisterListener(EventID.OnDestroyDot, (param) => UpdateCurrentScore((Board.DotId)param));
    }
    // Start is called before the first frame update
    void Start()
    {        
        this.GetComponent<Text>().text = "0";
    }
   
    void UpdateCurrentScore(Board.DotId id)
    {
        PlayerConfig.instance.currentScore += 10;
        this.GetComponent<Text>().text = PlayerConfig.instance.currentScore.ToString();
    }
}
