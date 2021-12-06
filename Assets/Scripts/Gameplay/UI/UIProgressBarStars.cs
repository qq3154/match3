using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIProgressBarStars : MonoBehaviour
{
    [SerializeField] Image mask;
    [SerializeField] Animator[] stars;
    [SerializeField] Animator star2;
    [SerializeField] Animator star3;

    bool[] _isPlayYet = new bool[3];    

    // Start is called before the first frame update
    void Start()
    {
        _isPlayYet[0] = false;
        _isPlayYet[1] = false;
        _isPlayYet[2] = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckStar(0);
        CheckStar(1);
        CheckStar(2);

        
    }

    void CheckStar(int i)
    {
        if (mask.fillAmount >= (float)(i+1) / (float)3 && _isPlayYet[i] ==false)
        {
            stars[i].Play("Star");
            _isPlayYet[i] = true;
            
        }   
    }
}
