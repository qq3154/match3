using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Observer;

public class UIProgressBar : MonoBehaviour
{    
    public Image mask;

    int _maximum;
    int _current;
    float _fillAmount;

    private void Awake()
    {
        this.RegisterListener(EventID.OnDestroyDot, (param) => UpdateProgressBar());
    }

    // Start is called before the first frame update
    void Start()
    {
        _maximum = PlayerConfig.instance.target.score.scoreToWin * 3;
        _current = PlayerConfig.instance.currentScore;
        _fillAmount = (float)_current / (float)_maximum;
    }

    // Update is called once per frame
    void Update()
    {
        if(mask.fillAmount < _fillAmount)
        {
            mask.fillAmount += 0.2f * Time.deltaTime;
        }
        
      
    }

    void UpdateProgressBar()
    {
        _current = PlayerConfig.instance.currentScore;
        _fillAmount = (float)_current / (float)_maximum;
    }
}
