using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// /// Config the end level board when player win/lose the level
/// </summary>
public class EndLevelBoard : MonoBehaviour
{
    [SerializeField] GameObject levelHeader;
    [SerializeField] Sprite spriteLevelComlete;
    [SerializeField] Sprite spriteLevelFail;

    [SerializeField] GameObject starComplete;
    [SerializeField] GameObject starFail;

    [SerializeField] Image score;
    [SerializeField] Text scoreText;

    [SerializeField] Sprite spriteScoreComplete;
    [SerializeField] Sprite spriteScoreFail;

    [SerializeField] Button nextLevelButton;
   
    bool _isWinLevel;

    private void Awake()
    {
        starComplete.SetActive(false);
        starFail.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        _isWinLevel = PlayerConfig.instance.isWinLevel;        

        LevelTextConfig();       //Show the text win or lose the level
        SetStar();              //Show the stars player get
        SetScore();             //Show the score
        ActiveNextLevelButton();//Config NextLevel Button only available when player pass the level 

        //Set the highest score of this level
        //Set the highest level player already win
        if (_isWinLevel)
        {                     
            if (PlayerConfig.instance.currentLevel == PlayerPrefs.GetInt("Level") && PlayerConfig.instance.currentLevel < PlayerConfig.instance.maxLevel)
            {
                PlayerPrefs.SetInt("Level", PlayerConfig.instance.currentLevel + 1);
                PlayerPrefs.Save();
            }
            
            if (PlayerConfig.instance.currentScore > PlayerPrefs.GetInt("HighestScoreLevel" + PlayerConfig.instance.currentLevel))
            {
                PlayerPrefs.SetInt("HighestScoreLevel" + PlayerConfig.instance.currentLevel, PlayerConfig.instance.currentScore);
                PlayerPrefs.Save();
            }
        }
        
    }


    #region Private Methods
   
    void LevelTextConfig()
    {
        if (_isWinLevel)
        {
            levelHeader.GetComponent<Image>().sprite = spriteLevelComlete;

        }
        else
        {
            levelHeader.GetComponent<Image>().sprite = spriteLevelFail;
        }
    }
    
    void SetStar()
    {
        if (_isWinLevel)
        {
            starComplete.SetActive(true);

        }
        else
        {
            starFail.SetActive(true);
        }
    }
    
    void SetScore()
    {
        scoreText.text = "Score: " + PlayerConfig.instance.currentScore.ToString();
        if (_isWinLevel)
        {
            score.GetComponent<Image>().sprite = spriteScoreComplete;

        }
        else
        {
            score.GetComponent<Image>().sprite = spriteScoreFail;
        }
    }
    
    void ActiveNextLevelButton()
    {
        if (_isWinLevel)
        {
            if (PlayerConfig.instance.currentLevel < PlayerConfig.instance.maxLevel)
            {
                nextLevelButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                nextLevelButton.GetComponent<Button>().interactable = false;
            }

        }
        else
        {
            if (PlayerConfig.instance.currentLevel == PlayerPrefs.GetInt("Level"))
            {
                nextLevelButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                nextLevelButton.GetComponent<Button>().interactable = true;
            }
        }
    }
    #endregion

}
