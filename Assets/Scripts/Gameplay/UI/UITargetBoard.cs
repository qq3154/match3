using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Observer;

public class UITargetBoard : MonoBehaviour
{
    [SerializeField] GameObject itemPrefab;
    [SerializeField] GameObject scorePrefab;
   
    // Start is called before the first frame update
    void Start()
    {
        GameObject targetprefab = Instantiate(itemPrefab, this.transform, false);
        targetprefab.SetActive(false);

        //Target
        if (PlayerConfig.instance.target.id == TargetConfig.TargetId.ScoreOnly)
        {
            GameObject Score = Instantiate(scorePrefab, this.transform, false);
            Score.GetComponent<Text>().text = "Get " + PlayerConfig.instance.target.score.scoreToWin.ToString() + " points";
        }
        else
        {            
            int length = PlayerConfig.instance.targetObjectTotal * 60;
            int distance = 0;
            int start = 0;                       
            if (PlayerConfig.instance.targetObjectTotal != 1)
            {
                distance = length / (PlayerConfig.instance.targetObjectTotal - 1);
                start = -(length / 2);
            }
            
            for (int i =0; i < PlayerConfig.instance.targetObjectTotal; i++)
            {
                GameObject target = Instantiate(itemPrefab, this.transform, false);
                target.GetComponent<RectTransform>().anchoredPosition = new Vector3(start + distance * i, -40, 0);
                switch (PlayerConfig.instance.target.id)
                {
                    case TargetConfig.TargetId.Item:
                        target.GetComponent<UITarget>().TargetItem(PlayerConfig.instance.target.item.itemList[i].id, i);
                        break;
                    case TargetConfig.TargetId.Bomb:
                        target.GetComponent<UITarget>().TargetBomb(PlayerConfig.instance.target.bomb.bombList[i].id, i);
                        break;
                    case TargetConfig.TargetId.Combo:
                        target.GetComponent<UITarget>().TargetCombo(PlayerConfig.instance.target.combo.comboList[i].id, i);
                        break;
                }
                
            }            
        }        
    }  

}
