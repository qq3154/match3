using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarRender : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;
    [SerializeField] GameObject gameController;

    // Start is called before the first frame update
    void Start()
    {        
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetStar(int CurrentLevel)
    {


        this.GetComponent<Image>().sprite = sprites[GetStarNumber(CurrentLevel)];
    }

    int GetStarNumber(int i)
    {
        int score = PlayerPrefs.GetInt("HighestScoreLevel" + i);
        int target = TargetManager.instance.Targets[i-1].score.scoreToWin;
        if (score < target)
        {
            return 0;
        }
        if (score >= target && score < 2 * target)
        {
            return 1;
        }
        if (score >= 2 * target && score < 3 * target)
        {
            return 2;
        }
        if (score >= 3 * target)
        {
            return 3;
        }
        return 0;        
    }
}
