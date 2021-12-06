using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConfig : MonoSingleton<PlayerConfig>
{
    public int maxLevel;
    public int currentScore;
    public int currentLevel;
    public int moveCount;    
    public bool isWinLevel;

    //Target
    public TargetConfig target;
    public int targetObjectTotal;
    public int[] targetObjectCount;

    /// <summary>
    /// Get the target from TargetConfig before start that level
    /// </summary>
    /// <param name="CurrentLevel"></param>
    public void SetCurrentLevel(int CurrentLevel)
    {
        this.currentLevel = CurrentLevel;
        this.currentScore = 0;
        this.moveCount = TargetManager.instance.GetLevel(CurrentLevel).move;
        this.target = TargetManager.instance.GetLevel(CurrentLevel);

        //if Target = Item
        if(this.target.id == TargetConfig.TargetId.Item)
        {
            this.targetObjectTotal = this.target.item.count;
            targetObjectCount = new int[this.targetObjectTotal];
            
            for (int i =0; i < this.target.item.count; i++)
            {
                targetObjectCount[i] = this.target.item.itemList[i].count;
            }
        }
        //If target = Bomb
        if (this.target.id == TargetConfig.TargetId.Bomb)
        {
            this.targetObjectTotal = this.target.bomb.count;
            targetObjectCount = new int[this.targetObjectTotal];

            for (int i = 0; i < this.target.bomb.count; i++)
            {
                targetObjectCount[i] = this.target.bomb.bombList[i].count;
            }
        }
        //If target = Combo
        if (this.target.id == TargetConfig.TargetId.Combo)
        {
            this.targetObjectTotal = this.target.combo.count;
            targetObjectCount = new int[this.targetObjectTotal];

            for (int i = 0; i < this.target.combo.count; i++)
            {
                targetObjectCount[i] = this.target.combo.comboList[i].count;
            }
        }


    }

    protected override void DoOnAwake()
    {
        maxLevel = TargetManager.instance.MaxLevel();
    }


    //PlayerPrefs.SetInt("CurrentLevel", 1);
    //PlayerPrefs.Save();
    //for (int i = 1; i <= 16; i++)
    //{
    //    PlayerPrefs.SetInt("HighestCurrentScoreLevel" + i, 0);

    //}
}
