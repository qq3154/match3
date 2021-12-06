using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Observer;

public class UITarget : MonoBehaviour
{
    [SerializeField] Sprite[] spriteItems;
    [SerializeField] Sprite[] spriteBombs;
    [SerializeField] Sprite[] spriteCombos;
    [SerializeField] Image image;
    [SerializeField] Text number;

    #region Private Field
    Board.DotId _itemId;
    Board.BombId _bombId;
    Board.ComboId _comboId;
    int n;
    #endregion

    private void Awake()
    {
        //resgiter event when dot destroyed
        this.RegisterListener(EventID.OnDestroyDot, (param) => ItemTargetUpdate((Board.DotId)param));

        //resgiter event when bomb explode
        this.RegisterListener(EventID.OnBombExplode, (param) => BombTargetUpdate((Board.BombId)param));

        //register event when combo
        this.RegisterListener(EventID.OnCombo, (param) => ComboTargetUpdate((Board.ComboId)param));
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        number.text = PlayerConfig.instance.targetObjectCount[n].ToString();
    }

    #region Public Get Target Type
    public void TargetItem(Board.DotId dotId, int n)
    {
        this.n = n;
        _itemId = dotId;
        image.sprite = spriteItems[(int)dotId];

    }

    public void TargetBomb(Board.BombId bombId, int n)
    {
        this.n = n;
        this._bombId = bombId;
        image.sprite = spriteBombs[(int)bombId];
    }
    public void TargetCombo(Board.ComboId comboId, int n)
    {
        this.n = n;
        this._comboId = comboId;
        image.sprite = spriteCombos[(int)comboId];
    }
    #endregion

    #region Private Udapte Target
    void BombTargetUpdate(Board.BombId bombId)
    {
        if(this._bombId == bombId)
        {
            if(PlayerConfig.instance.targetObjectCount[n] != 0)
                PlayerConfig.instance.targetObjectCount[n]--;
        }
        else
        {
            if(this._bombId == Board.BombId.Bomb4Horizontal && bombId == Board.BombId.Bomb4Vertical)
            {
                if (PlayerConfig.instance.targetObjectCount[n] != 0)
                    PlayerConfig.instance.targetObjectCount[n]--;
            }
        }
    }

    void ItemTargetUpdate(Board.DotId dotId)
    {
        if(dotId != Board.DotId.Special5)
        {
            if (this._itemId == dotId && PlayerConfig.instance.targetObjectCount[n] != 0)
            {
                PlayerConfig.instance.targetObjectCount[n]--;
            }
        }
                
    }
    void ComboTargetUpdate(Board.ComboId comboId)
    {
        if(this._comboId == comboId)
        {
            PlayerConfig.instance.targetObjectCount[n]--;
            
        }
        switch (comboId)
        {
            case Board.ComboId.bomb4_bomb4:
                BombTargetUpdate(Board.BombId.Bomb4Horizontal);
                BombTargetUpdate(Board.BombId.Bomb4Horizontal);
                break;
            case Board.ComboId.bomb4_bomb3x3:
                BombTargetUpdate(Board.BombId.Bomb4Horizontal);
                BombTargetUpdate(Board.BombId.Bomb3x3);
                break;
            case Board.ComboId.bomb3x3_bomb3x3:
                BombTargetUpdate(Board.BombId.Bomb3x3);
                BombTargetUpdate(Board.BombId.Bomb3x3);
                break;
            case Board.ComboId.bomb5_normal:
                BombTargetUpdate(Board.BombId.Bomb5);
                break;
            case Board.ComboId.bomb5_bomb4:
                BombTargetUpdate(Board.BombId.Bomb5);
                BombTargetUpdate(Board.BombId.Bomb4Horizontal);
                break;
            case Board.ComboId.bomb5_bomb3x3:
                BombTargetUpdate(Board.BombId.Bomb5);
                BombTargetUpdate(Board.BombId.Bomb3x3);
                break;
            case Board.ComboId.bomb5_bomb5:
                BombTargetUpdate(Board.BombId.Bomb5);
                BombTargetUpdate(Board.BombId.Bomb5);
                break;
        }
    }
    #endregion
}
