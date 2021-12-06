using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Observer;
using System.Linq;
using DG.Tweening;

public class View : MonoBehaviour
{
    [SerializeField] Board board;
    [SerializeField] GameObject[] dots;
    [SerializeField] GameObject bombExplodeVFX;

    [Header("Sound Effect")]
    [SerializeField] AudioCommand SFXCantMove;
    [SerializeField] AudioCommand SFXMatch;
    [SerializeField] AudioCommand SFXPulldown;
    [SerializeField] AudioCommand SFXCreateBomb4;
    [SerializeField] AudioCommand SFXCreateBomb3x3;
    [SerializeField] AudioCommand SFXCreateBomb5;
    [SerializeField] AudioCommand SFXExplodeBomb4;
    [SerializeField] AudioCommand SFXExplodeBomb3x3;
    [SerializeField] AudioCommand SFXExplodeBomb5;
    [SerializeField] AudioCommand SFXCombo4_3x3;
    [SerializeField] AudioCommand SFXCombo3x3_3x3;

    private void Awake()
    {
        this.RegisterListener(EventID.SetupBoard, (param) => SetupBoard());        
        this.RegisterListener(EventID.OnViewSwap, (param) => ViewSwap((Position[])param,false));
        this.RegisterListener(EventID.OnViewSwapBack, (param) => ViewSwap((Position[])param,true));
        this.RegisterListener(EventID.OnViewSimulate, (param) => ViewSimulate((Action)param));
    }

    void SetupBoard()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        for (int x = 0; x < board.Width; x++)
        {
            for (int y = 0; y < board.Height; y++)
            {
                Vector3 tempPos = new Vector2(x + 0.5f, y + 0.5f);
                GameObject dot = Instantiate(dots[(int)board.allDots[x, y]], tempPos, Quaternion.identity);
                dot.transform.parent = this.transform;
            }
        }
    }

    void ViewSwap(Position[] swapPosition,bool isSwapBack)
    {
        //Audio
        if(isSwapBack) SFXCantMove.Excute();
        StartCoroutine(IE_Swap(swapPosition,isSwapBack));
    }

    IEnumerator IE_Swap(Position[] swapPosition,bool isSwapBack)
    {
        GameObject item1 = GetItemAtThisPosition(swapPosition[0]);
        GameObject item2 = GetItemAtThisPosition(swapPosition[1]);
        Vector2 temp = item1.transform.position;

        item1.GetComponent<Dot>().Move(item2.transform.position, 10f);
        item2.GetComponent<Dot>().Move(temp, 10f);

        

        yield return new WaitForSeconds(0.5f);
        if (isSwapBack)
        {
            SwipeController.s_swipeAble = true;            
        }
        else
        {
            this.PostEvent(EventID.OnBoardFindMatchWhenSwap, swapPosition);
        }
        

    }

    void ViewSimulate(Action action)
    {
       
        StartCoroutine(IE_Combo(action));
    }

    IEnumerator IE_Combo(Action action)
    {
        float timeDelay =0;        
              
        //Combo
        while (action.comboPositions.Count != 0)
        {
            Position pos = action.comboPositions.First.Value;
            action.comboPositions.RemoveFirst();
            GameObject item = GetItemAtThisPosition(pos);
            Vector2 comboPos = new Vector2(pos.x + 0.5f, pos.y + 0.5f);

            //post event when combo
            this.PostEvent(EventID.OnCombo, pos.comboId); 

            //bomb5
            if (pos.comboId == Board.ComboId.bomb5_normal || pos.comboId == Board.ComboId.bomb5_bomb3x3 || pos.comboId == Board.ComboId.bomb5_bomb4 ||pos.comboId == Board.ComboId.bomb5_bomb5)
            {                
                if (action.bombPositions.Count != 0)
                {
                    StartCoroutine(IE_CreateBomb(action));
                    timeDelay = 1f;
                }
                //Audio
                SFXExplodeBomb5.Excute();
            }
            //bomb4 * bomb4
            if (pos.comboId == Board.ComboId.bomb4_bomb4)
            {                               
                GameObject vfxX = Instantiate(bombExplodeVFX);
                GameObject vfxY = Instantiate(bombExplodeVFX);                
                vfxX.GetComponent<BombExplodeVFX>().DestroyTile(comboPos, item.GetComponent<Dot>().id, Board.BombId.Bomb4Horizontal);                
                vfxY.GetComponent<BombExplodeVFX>().DestroyTile(comboPos, item.GetComponent<Dot>().id, Board.BombId.Bomb4Vertical);
                //Audio
                SFXExplodeBomb4.Excute();
                SFXExplodeBomb4.Excute();
            }
            //bomb4 * bomb3x3
            if (pos.comboId == Board.ComboId.bomb4_bomb3x3)
            {                
                GameObject vfxX = Instantiate(bombExplodeVFX);                
                vfxX.GetComponent<BombExplodeVFX>().DestroyTile3X(comboPos, item.GetComponent<Dot>().id, Board.BombId.Bomb4Horizontal);
                GameObject vfxY = Instantiate(bombExplodeVFX);                
                vfxY.GetComponent<BombExplodeVFX>().DestroyTile3X(comboPos, item.GetComponent<Dot>().id, Board.BombId.Bomb4Vertical);
                //Audio
                SFXCombo4_3x3.Excute();
            }
            //bomb3x3 * bomb3x3
            if (pos.comboId == Board.ComboId.bomb3x3_bomb3x3)
            {                
                GameObject vfx = Instantiate(bombExplodeVFX);                
                vfx.GetComponent<BombExplodeVFX>().Destroy5x5(comboPos, item.GetComponent<Dot>().id, Board.BombId.Bomb4Horizontal);
                //Audio
                SFXCombo3x3_3x3.Excute();
            }
        }
        
        yield return new WaitForSeconds(timeDelay);

        StartCoroutine(IE_DestroyMatch(action));
    }

    IEnumerator IE_CreateBomb(Action action)
    {
        //Create Bomb      
        while (action.bombPositions.Count != 0)
        {
            Position pos = action.bombPositions.First.Value;
            action.bombPositions.RemoveFirst();
            GameObject item = GetItemAtThisPosition(pos);
            if (item != null)
            {
                item.GetComponent<Dot>().ChangeSprite(pos.bombId);
                if (pos.bombId == Board.BombId.Bomb5)
                {
                    item.GetComponent<Dot>().id = Board.DotId.Special5;
                }
                
                //Audio
                switch (pos.bombId)
                {
                    case Board.BombId.Bomb4Horizontal:                        
                        SFXCreateBomb4.Excute();
                        break;

                    case Board.BombId.Bomb4Vertical:                     
                        SFXCreateBomb4.Excute();
                        break;

                    case Board.BombId.Bomb3x3:                     
                        SFXCreateBomb3x3.Excute();
                        break;

                    case Board.BombId.Bomb5:                       
                        SFXCreateBomb5.Excute();
                        break;

                    //default:
                    //    break;
                }
                yield return new WaitForSeconds(0.05f);  

            }
            else
            {
                Debug.LogWarning("null at" + pos.x + pos.y);
            }

        }
        //Destroy next dots to create bomb
        while (action.destroyWhenCreateBombPositions.Count != 0)
        {
            Position pos = action.destroyWhenCreateBombPositions.First.Value;
            action.destroyWhenCreateBombPositions.RemoveFirst();

            GameObject item = GetItemAtThisPosition(pos);
            if (item != null)
            {
                item.GetComponent<Dot>().MoveWhenCreateBomb(pos.targetX, pos.targetY);
            }

        }
        
    }

    IEnumerator IE_DestroyMatch(Action action)
    {       
        float timeDelay = (action.explodePositions.Count != 0 || action.destroyPositions.Count != 0 || action.bombPositions.Count != 0) ? 0.8f : 0;

        //Explode      
        while (action.explodePositions.Count != 0)
        {            
            Position pos = action.explodePositions.First.Value;
            action.explodePositions.RemoveFirst();
            //post event when bomb explode
            this.PostEvent(EventID.OnBombExplode, pos.bombId);

            GameObject item = GetItemAtThisPosition(pos);
            pos.id = item.GetComponent<Dot>().id;
            if(pos.bombId == Board.BombId.Bomb3x3)
            {
                Vector2 explodePos = new Vector2(pos.x + 0.5f, pos.y + 0.5f);
                GameObject vfx = Instantiate(bombExplodeVFX);                
                vfx.GetComponent<BombExplodeVFX>().Destroy3x3(explodePos, pos.id, pos.bombId);               
                //Audio
                SFXExplodeBomb3x3.Excute();
            }
            if (pos.bombId == Board.BombId.Bomb4Horizontal || pos.bombId == Board.BombId.Bomb4Vertical)
            {
                Vector2 explodePos = new Vector2(pos.x + 0.5f, pos.y + 0.5f);                
                GameObject vfx = Instantiate(bombExplodeVFX);                
                vfx.GetComponent<BombExplodeVFX>().DestroyTile(explodePos, pos.id, pos.bombId);
                //Audio
                SFXExplodeBomb4.Excute();
            }
            yield return new WaitForSeconds(0.05f);
        }

        // Destroy matches
        //Audio
        if(action.destroyPositions.Count != 0)
        {
            SFXMatch.Excute();
        }
        while (action.destroyPositions.Count != 0)
        {

            Position pos = action.destroyPositions.First.Value;
            action.destroyPositions.RemoveFirst();
            GameObject item = GetItemAtThisPosition(pos);
            item.GetComponent<Dot>().DestroyThisDot();
            yield return new WaitForSeconds(0);
        }

        //Create Bomb
        StartCoroutine( IE_CreateBomb(action));

        yield return new WaitForSeconds(timeDelay);
                                               
        StartCoroutine(IE_RefillBoard(action));
    }

    IEnumerator IE_RefillBoard(Action action)
    {        
        float timeDelay = 0.5f;
        //Action action = actionList.Action[count];
       

        //pull down
        //Audio
        if(action.pulldownPositions.Count != 0) SFXPulldown.Excute();
        while (action.pulldownPositions.Count !=0)
        {
            //foreach(var pos in action.PulldownPositions2)
            {
                Position pos = action.pulldownPositions.First.Value;
                action.pulldownPositions.RemoveFirst();
                GameObject item = GetItemAtThisPosition(pos);
                Vector2 destination = new Vector2(item.transform.position.x, item.transform.position.y - pos.targetY);
                item.GetComponent<Dot>().Move(destination, 20);
            }
            
        }

        //Refill
        int[] row = new int[7];
        while (action.refillPositions.Count != 0)
        {                        
            Position pos = action.refillPositions.First.Value;
            action.refillPositions.RemoveFirst();
            row[pos.x]++;            
            Vector3 tempPos = new Vector2(pos.x + 0.5f, row[pos.x] + 6.3f);
            Vector3 destination = new Vector2(pos.x + 0.5f, pos.y + 0.5f);
            GameObject dot = Instantiate(dots[(int)pos.id], tempPos, Quaternion.identity);
            dot.GetComponent<Dot>().Move(destination, 20);            
            dot.transform.parent = this.transform;
        }
        
        yield return new WaitForSeconds(timeDelay);
        
        this.PostEvent(EventID.OnBoardContinueFindMatches);
        
    }
    
    GameObject GetItemAtThisPosition(Position pos)
    {        
        Vector2 item = new Vector2(pos.x + 0.5f, pos.y + 0.5f);
        RaycastHit2D hit = new RaycastHit2D();
        if (!Physics2D.Raycast(item, Vector2.up, 0.5f))
        {
            Debug.LogWarning("null at " + pos.x + pos.y);
                        
        }               
            
        hit = Physics2D.Raycast(item, Vector2.up, 0.5f);
        
        return hit.collider.gameObject;               
    }

}
