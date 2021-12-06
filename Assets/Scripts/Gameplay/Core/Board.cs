using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Observer;
using UnityEngine.Events;

/// <summary>
/// This class is used to simulate the logic of the board
/// </summary>
public class Board : MonoBehaviour
{

    #region Init

    public int Width;
    public int Height;
    public int TotalDot = 6;
    public DotId[,] allDots;

    public enum DotId
    {
        Blank = 0,
        Blue = 1,
        Pink = 2,
        Red = 3,
        Orange = 4,
        Green = 5,
        Special5 = 6
    }

    public enum BombId
    {
        Blank = 0,
        Bomb4Horizontal = 1,
        Bomb4Vertical = 2,
        Bomb3x3 = 3,
        Bomb5 = 4
    }

    public enum ComboId
    {
        none = 0,
        bomb4_bomb4 = 1,
        bomb4_bomb3x3 = 2,
        bomb3x3_bomb3x3 = 3,
        bomb5_normal = 4,
        bomb5_bomb4 = 5,
        bomb5_bomb3x3 = 6,
        bomb5_bomb5 = 7,
    }

    BombId[,] _allDotsBomb;
    BombId[,] _allDotsBombAlreadyExist;
    bool[,] _allDotsDontDestroy;
    bool _isBombAlreadyExist = false;
    Action _action;
    #endregion

    #region Setup board when start level

    private void Awake()
    {
        this.RegisterListener(EventID.OnBoardSwap, (param) => Swap((Position[])param));
        this.RegisterListener(EventID.OnBoardFindMatchWhenSwap, (param) => BoardFindMatchWhenSwap((Position[])param));
        this.RegisterListener(EventID.OnBoardContinueFindMatches, (param) => BoardContinueFindMatches());
    }

    // Start is called before the first frame update    
    void Start()
    {
        ResetBoard();        
        SetUp();
        this.PostEvent(EventID.SetupBoard);
    }

    void ResetBoard()
    {
        allDots = new Board.DotId[Width, Height];
        _allDotsBomb = new Board.BombId[Width, Height];
        _action = new Action();
    }

    /// <summary>
    /// Create a new board include random dots
    /// Make sure there are no match at the beginning
    /// </summary>
    void SetUp()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {                
                int rnd = Random.Range(1, TotalDot);
                while (AvoidMatchWhenSetUp(i, j, rnd))
                {
                    rnd = Random.Range(1, TotalDot);
                }
                allDots[i, j] = (DotId)rnd;
            }
        }
        //allDots[3, 3] = DotId.Red;
        //allDots[3, 4] = DotId.Green;
        //_allDotsBomb[3, 3] = BombId.Bomb4Horizontal;
        //_allDotsBomb[3, 4] = BombId.Bomb3x3;
    }

    bool AvoidMatchWhenSetUp(int x, int y, int rnd)
    {
        if (x > 1 && y > 1)
        {
            if (allDots[x - 1, y] == (DotId)rnd && allDots[x - 2, y] == (DotId)rnd)
            {
                return true;
            }
            if (allDots[x, y - 1] == (DotId)rnd && allDots[x, y - 2] == (DotId)rnd)
            {
                return true;
            }
        }
        else
        {
            if (x > 1)
            {
                if (allDots[x - 1, y] == (DotId)rnd
                && allDots[x - 2, y] == (DotId)rnd)
                {
                    return true;
                }
            }
            if (y > 1)
            {
                if (allDots[x, y - 1] == (DotId)rnd
                && allDots[x, y - 2] == (DotId)rnd)
                {
                    return true;
                }
            }
        }
        return false;
    }
    #endregion

    #region Board Simulate

    /// <summary>
    /// Swap 2 selected dots
    /// </summary>
    /// <param name="swapPositions"> the positions get from SwipeController</param>
    /// <param name="isSwapback">true when the swap cant find a match</param>
    void Swap(Position[] swapPositions, bool isSwapback = false)
    {
        DotId temp = allDots[swapPositions[0].x, swapPositions[0].y];
        allDots[swapPositions[0].x, swapPositions[0].y] = allDots[swapPositions[1].x, swapPositions[1].y];
        allDots[swapPositions[1].x, swapPositions[1].y] = temp;
        BombId bombtemp = _allDotsBomb[swapPositions[0].x, swapPositions[0].y];
        _allDotsBomb[swapPositions[0].x, swapPositions[0].y] = _allDotsBomb[swapPositions[1].x, swapPositions[1].y];
        _allDotsBomb[swapPositions[1].x, swapPositions[1].y] = bombtemp;

        if (isSwapback)
        {
            this.PostEvent(EventID.OnViewSwapBack, swapPositions);
        }
        else
        {
            this.PostEvent(EventID.OnViewSwap, swapPositions);
        }
    }

    /// <summary>
    /// Check if there are any matches or combo at the swap positions 
    /// If cant find any match => swapBack
    /// </summary>
    /// <param name="swapPosition"></param>
    void BoardFindMatchWhenSwap(Position[] swapPosition)
    {
        _allDotsBombAlreadyExist = new BombId[Width, Height];
        _allDotsDontDestroy = new bool[Width, Height];
        _isBombAlreadyExist = false;

        if (IsMatchWhenSwap(swapPosition) || IsCombo(swapPosition))
        {
            if (IsCombo(swapPosition))
            {
                ClearCombo(swapPosition);
            }
            else
            {                
                // Clear matches
                ClearMatchAtSwapPositions(swapPosition[0].x, swapPosition[0].y);
                ClearMatchAtSwapPositions(swapPosition[1].x, swapPosition[1].y);                                  
            }

            if (IsBombExplode())
            {
                ClearBomb();
            }

            if (_isBombAlreadyExist)
            {
                ClearExistBomb();
            }

            RefillBoard();

            this.PostEvent(EventID.OnViewSimulate, _action);
            this.PostEvent(EventID.OnMoveSuccessful);
        }
        else
        {
            Swap(swapPosition, true);
        }
    }

    /// <summary>
    /// After the matches is destroy and refill, continue find new matches
    /// Repeat until cant find any new match.  
    /// </summary>
    void BoardContinueFindMatches()
    {
        if (IsMatch())
        {
            _allDotsBombAlreadyExist = new BombId[Width, Height];
            _allDotsDontDestroy = new bool[Width, Height];
            _isBombAlreadyExist = false;

            ClearMatch();

            if (IsBombExplode())
            {

                ClearBomb();

            }

            if (_isBombAlreadyExist)
            {
                ClearExistBomb();
            }

            RefillBoard();

            this.PostEvent(EventID.OnViewSimulate, _action);
        }
        else
        {
            SwipeController.s_swipeAble = true;
            if (CheckEndLevel())
            {
                this.PostEvent(EventID.OnEndLevel);
            }
        }

    }

    /// <summary>
    /// Pulldown and refill with new dots after destroy match positions
    /// </summary>
    void RefillBoard()
    {
        //pull down
        int[] a = new int[Width];
        for (int x = 0; x < Width; x++)
        {
            a[x] = 0;
            for (int y = 0; y < Height; y++)
            {
                if (allDots[x, y] == 0)
                {
                    a[x]++;
                }
                else
                {
                    if (a[x] > 0)
                    {
                        allDots[x, y - a[x]] = allDots[x, y];
                        _allDotsBomb[x, y - a[x]] = _allDotsBomb[x, y];
                        _allDotsBomb[x, y] = 0;

                        _action.pulldownPositions.AddLast(new Position(x, y, x, a[x]));
                    }
                }
            }
        }

        //refill
        for (int x = 0; x < Width; x++)
        {
            if (a[x] != 0)
            {
                for (int y = Height - a[x]; y < Height; y++)
                {
                    allDots[x, y] = (DotId)Random.Range(1, TotalDot);
                    _action.refillPositions.AddLast(new Position(x, y, allDots[x, y]));
                }
            }
        }
    }
    #endregion

    #region Private sub-method

    bool IsCombo(Position[] swapPosition)
    {
        if (_allDotsBomb[swapPosition[0].x, swapPosition[0].y] == BombId.Bomb5 || _allDotsBomb[swapPosition[1].x, swapPosition[1].y] == BombId.Bomb5)
        {
            return true;
        }
        if (_allDotsBomb[swapPosition[0].x, swapPosition[0].y] != 0 && _allDotsBomb[swapPosition[1].x, swapPosition[1].y] != 0)
        {
            return true;
        }
        return false;
    }

    void ClearCombo(Position[] swapPosition)
    {
        // special bomb5 * special bomb5
        if (allDots[swapPosition[0].x, swapPosition[0].y] == DotId.Special5 && allDots[swapPosition[1].x, swapPosition[1].y] == DotId.Special5)
        {
            _action.comboPositions.AddLast(new Position(swapPosition[1].x, swapPosition[1].y, ComboId.bomb5_bomb5));

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    DestroyAt(x, y);
                    _allDotsBomb[x, y] = 0;
                }
            }
            return;
        }

        // speical bomb5 * bomb4
        if (allDots[swapPosition[0].x, swapPosition[0].y] == DotId.Special5
            && (_allDotsBomb[swapPosition[1].x, swapPosition[1].y] == BombId.Bomb4Horizontal || _allDotsBomb[swapPosition[1].x, swapPosition[1].y] == BombId.Bomb4Vertical))
        {
            _action.comboPositions.AddLast(new Position(swapPosition[1].x, swapPosition[1].y, ComboId.bomb5_bomb4));

            DotId temp = allDots[swapPosition[1].x, swapPosition[1].y];
            _allDotsBomb[swapPosition[0].x, swapPosition[0].y] = 0;
            DestroyAt(swapPosition[0].x, swapPosition[0].y);
            DestroyAt(swapPosition[1].x, swapPosition[1].y);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (allDots[x, y] == temp && _allDotsBomb[x, y] == 0)
                    {
                        CreateBombAt(x, y, allDots[x, y], (BombId)Random.Range(1, 3));
                        DestroyAt(x, y);
                    }
                }
            }

            return;
        }

        // speical bomb4 * bomb5
        if (allDots[swapPosition[1].x, swapPosition[1].y] == DotId.Special5
            && (_allDotsBomb[swapPosition[0].x, swapPosition[0].y] == BombId.Bomb4Horizontal || _allDotsBomb[swapPosition[0].x, swapPosition[0].y] == BombId.Bomb4Vertical))
        {
            _action.comboPositions.AddLast(new Position(swapPosition[1].x, swapPosition[1].y, ComboId.bomb5_bomb4));

            DotId temp = allDots[swapPosition[0].x, swapPosition[0].y];
            _allDotsBomb[swapPosition[1].x, swapPosition[1].y] = 0;
            DestroyAt(swapPosition[1].x, swapPosition[1].y);
            DestroyAt(swapPosition[0].x, swapPosition[0].y);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (allDots[x, y] == temp && _allDotsBomb[x, y] == 0)
                    {
                        CreateBombAt(x, y, allDots[x, y], (BombId)Random.Range(1, 3));
                        DestroyAt(x, y);
                    }
                }
            }
            return;
        }

        // speical bomb5 * bomb3x3
        if (allDots[swapPosition[0].x, swapPosition[0].y] == DotId.Special5
            && _allDotsBomb[swapPosition[1].x, swapPosition[1].y] == BombId.Bomb3x3)
        {
            _action.comboPositions.AddLast(new Position(swapPosition[1].x, swapPosition[1].y, ComboId.bomb5_bomb3x3));

            DotId temp = allDots[swapPosition[1].x, swapPosition[1].y];
            _allDotsBomb[swapPosition[0].x, swapPosition[0].y] = 0;
            DestroyAt(swapPosition[0].x, swapPosition[0].y);
            DestroyAt(swapPosition[1].x, swapPosition[1].y);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (allDots[x, y] == temp && _allDotsBomb[x, y] == 0)
                    {
                        CreateBombAt(x, y, allDots[x, y], BombId.Bomb3x3);
                        DestroyAt(x, y);
                    }
                }
            }
            return;
        }

        // speical bomb3x3 * bomb5
        if (allDots[swapPosition[1].x, swapPosition[1].y] == DotId.Special5
            && _allDotsBomb[swapPosition[0].x, swapPosition[0].y] == BombId.Bomb3x3)
        {
            _action.comboPositions.AddLast(new Position(swapPosition[1].x, swapPosition[1].y, ComboId.bomb5_bomb3x3));

            DotId temp = allDots[swapPosition[0].x, swapPosition[0].y];
            _allDotsBomb[swapPosition[1].x, swapPosition[1].y] = 0;
            DestroyAt(swapPosition[1].x, swapPosition[1].y);
            DestroyAt(swapPosition[0].x, swapPosition[0].y);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (allDots[x, y] == temp && _allDotsBomb[x, y] == 0)
                    {
                        CreateBombAt(x, y, allDots[x, y], BombId.Bomb3x3);
                        DestroyAt(x, y);
                    }
                }
            }
            return;
        }

        // speical bomb5 * normal
        if (allDots[swapPosition[0].x, swapPosition[0].y] == DotId.Special5 ^ allDots[swapPosition[1].x, swapPosition[1].y] == DotId.Special5)
        {
            _action.comboPositions.AddLast(new Position(swapPosition[1].x, swapPosition[1].y, ComboId.bomb5_normal));

            DotId value = 0;
            if (allDots[swapPosition[0].x, swapPosition[0].y] == DotId.Special5)
            {
                value = allDots[swapPosition[1].x, swapPosition[1].y];
                DestroyAt(swapPosition[0].x, swapPosition[0].y);
                _allDotsBomb[swapPosition[0].x, swapPosition[0].y] = 0;

            }
            else
            {
                value = allDots[swapPosition[0].x, swapPosition[0].y];
                DestroyAt(swapPosition[1].x, swapPosition[1].y);
                _allDotsBomb[swapPosition[1].x, swapPosition[1].y] = 0;
            }
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (allDots[x, y] == value)
                    {
                        DestroyAt(x, y);
                    }
                }
            }
            return;
        }

        // bomb4 * bomb4
        if ((_allDotsBomb[swapPosition[0].x, swapPosition[0].y] == BombId.Bomb4Horizontal || _allDotsBomb[swapPosition[0].x, swapPosition[0].y] == BombId.Bomb4Vertical)
            && (_allDotsBomb[swapPosition[1].x, swapPosition[1].y] == BombId.Bomb4Horizontal || _allDotsBomb[swapPosition[1].x, swapPosition[1].y] == BombId.Bomb4Vertical))
        {
            _action.comboPositions.AddLast(new Position(swapPosition[1].x, swapPosition[1].y, ComboId.bomb4_bomb4));

            _allDotsBomb[swapPosition[0].x, swapPosition[0].y] = 0;
            _allDotsBomb[swapPosition[1].x, swapPosition[1].y] = 0;

            for (int x = 0; x < Width; x++)
            {
                if (allDots[x, swapPosition[1].y] != 0)
                {
                    DestroyAt(x, swapPosition[1].y);
                }

            }
            for (int y = 0; y < Height; y++)
            {
                if (allDots[swapPosition[1].x, y] != 0)
                {
                    DestroyAt(swapPosition[1].x, y);
                }

            }
        }

        // bomb4 * bomb3x3
        if (((_allDotsBomb[swapPosition[0].x, swapPosition[0].y] == BombId.Bomb4Horizontal || _allDotsBomb[swapPosition[0].x, swapPosition[0].y] == BombId.Bomb4Vertical)
                    && (_allDotsBomb[swapPosition[1].x, swapPosition[1].y] == BombId.Bomb3x3))
            ||
            ((_allDotsBomb[swapPosition[0].x, swapPosition[0].y] == BombId.Bomb3x3)
                    && (_allDotsBomb[swapPosition[1].x, swapPosition[1].y] == BombId.Bomb4Horizontal || _allDotsBomb[swapPosition[1].x, swapPosition[1].y] == BombId.Bomb4Vertical)))
        {
            _action.comboPositions.AddLast(new Position(swapPosition[1].x, swapPosition[1].y, ComboId.bomb4_bomb3x3));

            _allDotsBomb[swapPosition[0].x, swapPosition[0].y] = 0;
            _allDotsBomb[swapPosition[1].x, swapPosition[1].y] = 0;

            for (int x = swapPosition[1].x - 1; x < swapPosition[1].x + 2; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (x >= 0 && x < Width && allDots[x, y] != 0)
                    {
                        DestroyAt(x, y);
                    }
                }
            }
            for (int y = swapPosition[1].y - 1; y < swapPosition[1].y + 2; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (y >= 0 && y < Height && allDots[x, y] != 0)
                    {
                        DestroyAt(x, y);
                    }
                }
            }
        }

        // bomb3x3 * bomb3x3
        if (_allDotsBomb[swapPosition[0].x, swapPosition[0].y] == BombId.Bomb3x3 && _allDotsBomb[swapPosition[1].x, swapPosition[1].y] == BombId.Bomb3x3)
        {
            _action.comboPositions.AddLast(new Position(swapPosition[1].x, swapPosition[1].y, ComboId.bomb3x3_bomb3x3));

            _allDotsBomb[swapPosition[0].x, swapPosition[0].y] = 0;
            _allDotsBomb[swapPosition[1].x, swapPosition[1].y] = 0;

            for (int x = swapPosition[1].x - 2; x < swapPosition[1].x + 3; x++)
            {
                for (int y = swapPosition[1].y - 2; y < swapPosition[1].y + 3; y++)
                {
                    if (x >= 0 && x < Width && y >= 0 && y < Height && allDots[x, y] != 0)
                    {
                        DestroyAt(x, y);
                    }

                }
            }
        }
    }

    bool IsMatchWhenSwap(Position[] swapPosition)
    {
        //bool ans = false;
        if (IsMatchAtSwapPositions(swapPosition[0].x, swapPosition[0].y))
        {
            return true;
        }

        if (IsMatchAtSwapPositions(swapPosition[1].x, swapPosition[1].y))
        {
            return true;
        }
        return false;
    }

    bool IsMatchAtSwapPositions(int x, int y)
    {
        int xCount = 1;
        int yCount = 1;
        int x_temp = x + 1;
        while (x_temp < Width && allDots[x, y] == allDots[x_temp, y])
        {
            xCount++;
            x_temp++;
        }
        x_temp = x - 1;
        while (x_temp >= 0 && allDots[x, y] == allDots[x_temp, y])
        {
            xCount++;
            x_temp--;
        }
        int y_temp = y + 1;
        while (y_temp < Height && allDots[x, y] == allDots[x, y_temp])
        {
            yCount++;
            y_temp++;
        }
        y_temp = y - 1;
        while (y_temp >= 0 && allDots[x, y] == allDots[x, y_temp])
        {
            yCount++;
            y_temp--;
        }

        if (xCount > 2 || yCount > 2)
        {
            return true;
        }

        return false;

    }

    void ClearMatchAtSwapPositions(int x, int y)
    {
        int xCount = 1;
        int yCount = 1;
        int x_temp = x + 1;
        while (x_temp < Width && allDots[x, y] == allDots[x_temp, y])
        {
            xCount++;
            x_temp++;
        }
        x_temp = x - 1;
        while (x_temp >= 0 && allDots[x, y] == allDots[x_temp, y])
        {
            xCount++;
            x_temp--;
        }

        int y_temp = y + 1;
        while (y_temp < Height && allDots[x, y] == allDots[x, y_temp])
        {
            yCount++;
            y_temp++;
        }
        y_temp = y - 1;
        while (y_temp >= 0 && allDots[x, y] == allDots[x, y_temp])
        {
            yCount++;
            y_temp--;
        }

        if (IsCreateBombAt(x, y, xCount, yCount))
        {
            _allDotsDontDestroy[x, y] = true;
        }
        else
        {
            if (xCount == 3)
            {
                DestroyNext(x, y, 1, 0);
            }
            if (yCount == 3)
            {
                DestroyNext(x, y, 1, 1);
            }
        }
    }

    bool IsCreateBombAt(int x, int y, int xCount, int yCount)
    {
        bool ans = false;
        if (xCount > 4)
        {
            if (_allDotsBomb[x, y] != 0)
            {
                _allDotsBombAlreadyExist[x, y] = _allDotsBomb[x, y];
                _isBombAlreadyExist = true;
            }
            DestroyNextWhenCreateBomb(x, y, 1);
            CreateBombAt(x, y, allDots[x, y], BombId.Bomb5);

            return true;
        }
        if (yCount > 4)
        {
            if (_allDotsBomb[x, y] != 0)
            {
                _allDotsBombAlreadyExist[x, y] = _allDotsBomb[x, y];
                _isBombAlreadyExist = true;
            }
            DestroyNextWhenCreateBomb(x, y, 2);
            CreateBombAt(x, y, allDots[x, y], BombId.Bomb5);

            return true;
        }

        if (xCount > 2 && yCount > 2)
        {
            if (_allDotsBomb[x, y] != 0)
            {
                _allDotsBombAlreadyExist[x, y] = _allDotsBomb[x, y];
                _isBombAlreadyExist = true;
            }
            CreateBombAt(x, y, allDots[x, y], BombId.Bomb3x3);
            DestroyNextWhenCreateBomb(x, y, 3);
            return true;
        }

        if (xCount >= 4)
        {
            if (_allDotsBomb[x, y] != 0)
            {
                _allDotsBombAlreadyExist[x, y] = _allDotsBomb[x, y];
                _isBombAlreadyExist = true;
            }
            CreateBombAt(x, y, allDots[x, y], BombId.Bomb4Vertical);
            //DestroyNext(x, y, 0, 0);
            DestroyNextWhenCreateBomb(x, y, 1);
            //ans = true;
            return true;
        }

        if (yCount >= 4)
        {
            if (_allDotsBomb[x, y] != 0)
            {
                _allDotsBombAlreadyExist[x, y] = _allDotsBomb[x, y];
                _isBombAlreadyExist = true;
            }
            CreateBombAt(x, y, allDots[x, y], BombId.Bomb4Horizontal);

            DestroyNextWhenCreateBomb(x, y, 2);

            return true;
        }
        return ans;
    }

    void ClearExistBomb()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (_allDotsBombAlreadyExist[x, y] != 0)
                {
                    ClearBombAt(x, y, _allDotsBombAlreadyExist[x, y]);
                }
            }
        }
    }

    bool IsBombExplode()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (allDots[x, y] == 0 && _allDotsBomb[x, y] != 0)
                    return true;
            }
        }
        return false;
    }

    void ClearBomb()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (allDots[x, y] == 0 && _allDotsBomb[x, y] != 0)
                {
                    ClearBombAt(x, y, _allDotsBomb[x, y]);

                }
            }
        }
    }

    void ClearBombAt(int x, int y, Board.BombId bombId)
    {
        _action.explodePositions.AddLast(new Position(x, y, allDots[x, y], bombId));

        if (_allDotsDontDestroy[x, y] == false)
        {
            _allDotsBomb[x, y] = 0;
        }

        if (bombId == BombId.Bomb4Horizontal)
        {
            for (int x_temp = 0; x_temp < Width; x_temp++)
            {
                if (allDots[x_temp, y] != 0 && _allDotsDontDestroy[x_temp, y] == false)
                {
                    DestroyAt(x_temp, y);
                    if (_allDotsBomb[x_temp, y] != 0)
                    {
                        ClearBombAt(x_temp, y, _allDotsBomb[x_temp, y]);
                    }

                }
            }
        }

        if (bombId == BombId.Bomb4Vertical)
        {
            for (int y_temp = 0; y_temp < Height; y_temp++)
            {
                if (allDots[x, y_temp] != 0 && _allDotsDontDestroy[x, y_temp] == false)
                {
                    DestroyAt(x, y_temp);
                    if (_allDotsBomb[x, y_temp] != 0)
                    {
                        ClearBombAt(x, y_temp, _allDotsBomb[x, y_temp]);
                    }

                }
            }
        }

        if (bombId == BombId.Bomb3x3)
        {
            for (int x_temp = x - 1; x_temp < x + 2; x_temp++)
            {
                for (int y_temp = y - 1; y_temp < y + 2; y_temp++)
                {
                    
                    if (x_temp >= 0 && x_temp < Width && y_temp >= 0 && y_temp < Height)
                    {
                        if (allDots[x_temp, y_temp] != 0 && _allDotsDontDestroy[x_temp, y_temp] == false)
                        {
                            DestroyAt(x_temp, y_temp);
                            if (_allDotsBomb[x_temp, y_temp] != 0)
                            {
                                ClearBombAt(x_temp, y_temp, _allDotsBomb[x_temp, y_temp]);
                            }
                        }
                    }
                    
                }
            }
        }

        if (bombId == BombId.Bomb5)
        {
            int rnd = Random.Range(1, TotalDot);
            for (int x_temp = 0; x_temp < Width; x_temp++)
            {
                for (int y_temp = 0; y_temp < Height; y_temp++)
                {
                    if ((int)allDots[x_temp, y_temp] == rnd)
                    {
                        DestroyAt(x_temp, y_temp);
                        if (_allDotsBomb[x_temp, y_temp] != 0)
                        {
                            ClearBombAt(x_temp, y_temp, _allDotsBomb[x_temp, y_temp]);
                        }
                    }
                }
            }
        }
    }

    bool IsMatch()
    {
        int[,] xCount = new int[6, 7];
        int[,] yCount = new int[6, 7];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (allDots[x, y] != 0)
                {

                    if (x > 0 && allDots[x, y] == allDots[x - 1, y])
                    {
                        xCount[x, y] = xCount[x - 1, y];

                    }
                    else
                    {
                        int x_temp = x;
                        if (x < Width)
                        {
                            while (x_temp < Width && allDots[x, y] == allDots[x_temp, y])
                            {
                                x_temp++;
                                xCount[x, y]++;
                            }

                        }
                    }

                    if (y > 0 && allDots[x, y] == allDots[x, y - 1])
                    {
                        yCount[x, y] = yCount[x, y - 1];

                    }
                    else
                    {
                        int y_temp = y;
                        if (y < Height)
                        {
                            while (y_temp < Height && allDots[x, y] == allDots[x, y_temp])
                            {
                                y_temp++;
                                yCount[x, y]++;
                            }
                        }
                    }
                    if (xCount[x, y] > 2 || yCount[x, y] > 2)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    void ClearMatch()
    {
        int[,] xCount = new int[6, 7];
        int[,] yCount = new int[6, 7];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (allDots[x, y] != 0)
                {

                    if (x > 0 && allDots[x, y] == allDots[x - 1, y])
                    {
                        xCount[x, y] = xCount[x - 1, y];

                    }
                    else
                    {
                        int x_temp = x;
                        if (x < Width)
                        {
                            while (x_temp < Width && allDots[x, y] == allDots[x_temp, y])
                            {
                                x_temp++;
                                xCount[x, y]++;
                            }

                        }
                    }

                    if (y > 0 && allDots[x, y] == allDots[x, y - 1])
                    {
                        yCount[x, y] = yCount[x, y - 1];

                    }
                    else
                    {
                        int y_temp = y;
                        if (y < Height)
                        {
                            while (y_temp < Height && allDots[x, y] == allDots[x, y_temp])
                            {
                                y_temp++;
                                yCount[x, y]++;
                            }
                        }
                    }
                }
            }
        }

        //Find bomb 
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (allDots[x, y] != 0)
                {
                    if (IsCreateBombAt(x, y, xCount[x, y], yCount[x, y]))
                    {
                        _allDotsDontDestroy[x, y] = true;
                        ClearMatch();
                        return;
                    }
                }

            }
        }

        //Destroy matches
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (allDots[x, y] != 0)
                {

                    if (xCount[x, y] == 3 ^ yCount[x, y] == 3)
                    {
                        DestroyAt(x, y);
                    }
                }

            }
        }
    }
    #endregion

    #region Set action
    void CreateBombAt(int x, int y, DotId id, Board.BombId bombId)
    {
        _action.bombPositions.AddLast(new Position(x, y, allDots[x, y], bombId));

        _allDotsBomb[x, y] = bombId;
        if (bombId == Board.BombId.Bomb5)
        {
            allDots[x, y] = DotId.Special5;
        }
    }

    void DestroyAt(int x, int y)
    {
        if (allDots[x, y] != 0)
        {
            _action.destroyPositions.AddLast(new Position(x, y));
            allDots[x, y] = 0;
        }
        else
        {
            Debug.LogWarning("destroy fail at" + x + y);
        }

    }

    void DestroyNext(int x, int y, int destroyitself, int direction)
    {
        if (direction == 0)
        {
            int x_temp = x + 1;
            while (x_temp < Width && allDots[x, y] == allDots[x_temp, y])
            {
                DestroyAt(x_temp, y);
                x_temp++;
            }
            x_temp = x - 1;
            while (x_temp >= 0 && allDots[x, y] == allDots[x_temp, y])
            {
                DestroyAt(x_temp, y);
                x_temp--;
            }
        }
        else
        {
            int y_temp = y + 1;
            while (y_temp < Height && allDots[x, y] == allDots[x, y_temp])
            {
                DestroyAt(x, y_temp);
                y_temp++;
            }
            y_temp = y - 1;
            while (y_temp >= 0 && allDots[x, y] == allDots[x, y_temp])
            {
                DestroyAt(x, y_temp);
                y_temp--;
            }
        }
        if (destroyitself == 1)
        {
            DestroyAt(x, y);
        }
    }

    void DestroyNextWhenCreateBomb(int x, int y, int bombId)
    {
        if (bombId != 2)
        {
            int x_temp = x + 1;
            while (x_temp < Width && allDots[x, y] == allDots[x_temp, y])
            {
                _action.destroyWhenCreateBombPositions.AddLast(new Position(x_temp, y, x, y));
                allDots[x_temp, y] = 0;
                x_temp++;
            }
            x_temp = x - 1;
            while (x_temp >= 0 && allDots[x, y] == allDots[x_temp, y])
            {
                _action.destroyWhenCreateBombPositions.AddLast(new Position(x_temp, y, x, y));
                allDots[x_temp, y] = 0;
                x_temp--;
            }
        }
        if (bombId != 1)
        {
            int y_temp = y + 1;
            while (y_temp < Height && allDots[x, y] == allDots[x, y_temp])
            {
                _action.destroyWhenCreateBombPositions.AddLast(new Position(x, y_temp, x, y));
                allDots[x, y_temp] = 0;
                y_temp++;
            }
            y_temp = y - 1;
            while (y_temp >= 0 && allDots[x, y] == allDots[x, y_temp])
            {
                _action.destroyWhenCreateBombPositions.AddLast(new Position(x, y_temp, x, y));
                allDots[x, y_temp] = 0;
                y_temp--;
            }
        }
    }
    #endregion

    /// <summary>
    /// Called after player make a legit move
    /// If player clear all ther target or run out of move => Stop the level
    /// </summary>
    /// <returns></returns>
    public bool CheckEndLevel()
    {
        if(PlayerConfig.instance.moveCount == 0)
        {
            return true;
        }

        //Target
        if (PlayerConfig.instance.target.id != TargetConfig.TargetId.ScoreOnly)
        {            

            for(int i =0; i < PlayerConfig.instance.targetObjectTotal; i++)
            {
                if (PlayerConfig.instance.targetObjectCount[i] != 0) return false;
            }
            return true;
        }

        return false;
    }
}
