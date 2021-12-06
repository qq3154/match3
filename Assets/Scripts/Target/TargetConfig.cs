using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetConfig", menuName = "GameConfiguration/Target/TargetConfig")]
public class TargetConfig : ScriptableObject
{
    public enum TargetId
    {
        ScoreOnly,
        Item,        
        Bomb,        
        Combo,        
    }

    public TargetId id;
    public int move;

    public ScoreType score;    
    public ItemType item;
    public BombType bomb;
    public ComboType combo;
    
    #region Public - Getting
    public TargetId GetId()
	{
		return this.id;
	}
    #endregion

    #region Validate
    private void OnValidate()
    {        
        if(move <= 0)
        {
            Debug.Log("Set Move kia");
        }
        if(score.scoreToWin <=0)
        {
            Debug.Log("Quen set Score kia!!!");
        }

              
        if (this.id != TargetId.ScoreOnly)
        {
            score.active = false;
        }
        else
        {
            score.active = true;
        }

        if(this.id != TargetId.Item)
        {
            item.active = false;
            item.count = 0;
            item.itemList = new ItemId[0];
        }
        else
        {
            item.active = true;
            if (item.itemList.Length != item.count)
            {
                item.itemList = new ItemId[item.count];
            }
        }

        if (this.id != TargetId.Bomb)
        {
            bomb.active = false;
            bomb.count = 0;
            bomb.bombList = new BombId[0];
        }
        else
        {
            bomb.active = true;
            if (bomb.bombList.Length != bomb.count)
            {
                bomb.bombList = new BombId[bomb.count];
            }
           
        }

        if (this.id != TargetId.Combo)
        {
            combo.active = false;
            combo.count = 0;
            combo.comboList = new ComboId[0];
        }
        else
        {
            combo.active = true;
            if (combo.comboList.Length != combo.count)
            {
                combo.comboList = new ComboId[combo.count];
            }
            
        }


    }
    #endregion
   
    #region Custom class
    [System.Serializable]
    public struct ScoreType
    {
        public bool active;
        public int scoreToWin;
    }
    
    /// Item   
    [System.Serializable]
    public struct ItemType
    {
        public bool active;
        public int count;
        public ItemId[] itemList;
    }
    [System.Serializable]
    public struct ItemId
    {        
        public Board.DotId id;
        public int count;
    }
    
    ///Bomb
    
    [System.Serializable]
    public struct BombType
    {
        public bool active;
        public int count;
        public BombId[] bombList;
    }
    [System.Serializable]
    public struct BombId
    {                
        public Board.BombId id;
        public int count;
    }

    //COmbo
    [System.Serializable]
    public struct ComboType
    {
        public bool active;
        public int count;
        public ComboId[] comboList;
        
    }
    [System.Serializable]
    public struct ComboId
    {        
        public Board.ComboId id;
        public int count;
    }

    #endregion
}
