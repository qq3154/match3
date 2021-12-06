using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Observer;

/// <summary>
/// Notification pop up when end level
/// Check if player win/lose the level
/// Load the end level scene
/// </summary>
public class EndLevelNotification : MonoBehaviour
{      
    [SerializeField] Vector3 startPosition;
    [SerializeField] Vector3 endPosition;

    private void Awake()
    {
        this.RegisterListener(EventID.OnEndLevel, (param) => ShowEndLevelNotification());
    }    

    void ShowEndLevelNotification()
    {
        if (IsWin())
        {
            PlayerConfig.instance.isWinLevel = true;
            
        }
        else
        {
            PlayerConfig.instance.isWinLevel = false;
            Debug.Log("Loser");
        }
        startPosition = transform.position;
        
        StartCoroutine(IE_MoveTable());
    }
    bool IsWin()
    {       
        //target
        TargetConfig target = PlayerConfig.instance.target;
        if (target.id == TargetConfig.TargetId.ScoreOnly)
        {
            if (PlayerConfig.instance.currentScore >= target.score.scoreToWin)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            for (int i = 0; i < PlayerConfig.instance.targetObjectTotal; i++)
            {
                if (PlayerConfig.instance.targetObjectCount[i] != 0) return false;
            }
            return true;
        }
        
    }

    IEnumerator IE_MoveTable()
    {
        SwipeController.s_swipeAble = false;

        float timeOfTravel = 0.5f; //time after object reach a target place 
        float currentTime = 0; // actual floting time 
        float normalizedValue;
        while (currentTime <= timeOfTravel)
        {
            currentTime += Time.deltaTime;
            normalizedValue = currentTime / timeOfTravel; // we normalize our time 

            transform.position = Vector3.Lerp(startPosition, endPosition, normalizedValue); //normalizedValue);

            yield return null;
        }

        yield return new WaitForSeconds(2f);

        ChangeScene.LoadScene("EndLevel");
        

    }
}
