using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetStar : MonoBehaviour
{
    [SerializeField] Image starYellow;
    [SerializeField] Image starBlank;

    // Start is called before the first frame update
    void Start()
    {
        int starCount = PlayerConfig.instance.currentScore / PlayerConfig.instance.target.score.scoreToWin;
        for(int i =1; i <= 3; i++)
        {
            Image star;
            if(i <= starCount)
            {
                star = Instantiate(starYellow, this.transform, false);
                star.rectTransform.anchoredPosition = new Vector3(-400 + i*200, 0, 0);

            }
            else
            {
                star = Instantiate(starBlank, this.transform, false);
                star.rectTransform.anchoredPosition = new Vector3(-400 + i * 200, 0, 0);
            }
        }
    }

}
