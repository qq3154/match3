using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Observer;

public class SwipeController : MonoBehaviour
{
    /// <summary>
    /// When player are able to swipe => start calculate the direction to get 2 dot positions
    /// Post an event for the Board to start the Swap function
    /// </summary>
    public static bool s_swipeAble = true;

    [SerializeField] Board board;

    Vector2 _firstTouchPos;
    Vector2 _finalTouchPos;

    float _swipeAngle;
    float _swipeResist = 0.5f; // the swipe length must > _swipeResist

    private void Awake()
    {
       
    }

    private void Start()
    {
       
        s_swipeAble = true;
    }

    private void OnMouseDown()
    {
        _firstTouchPos = Camera.main.ScreenToWorldPoint(position: Input.mousePosition); //Get the first touch position
    }

    private void OnMouseUp()
    {
        _finalTouchPos = Camera.main.ScreenToWorldPoint(position: Input.mousePosition); //Get the end touch position
      
        if (s_swipeAble)
        {
            CalculateAngle();
        }                       
    }

    #region Private - Get swap positions
    void CalculateAngle()
    {
        if (Mathf.Abs(_firstTouchPos.y - _finalTouchPos.y) > _swipeResist || Mathf.Abs(_firstTouchPos.x - _finalTouchPos.x) > _swipeResist)
        {
            s_swipeAble = false;
            _swipeAngle = Mathf.Atan2(_finalTouchPos.y - _firstTouchPos.y, _finalTouchPos.x - _firstTouchPos.x) * 180 / Mathf.PI;
            MovePieces((int)_firstTouchPos.x, (int)_firstTouchPos.y);
        }
    }
    void MovePieces(int x1, int y1)
    {        
        int x2 = x1;
        int y2 = y1;
        //right Swipe
        if (_swipeAngle > -45 && _swipeAngle <= 45 && x1 < board.Width - 1)
        {
            x2++;
        }
        //up Swipe
        if (_swipeAngle > 45 && _swipeAngle <= 135 && y1 < board.Height - 1)
        {
            y2++;
        }
        //left Swipe
        if ((_swipeAngle > 135 || _swipeAngle <= -135) && x1 > 0)
        {
            x2--;
        }
        //down Swipe
        if (_swipeAngle < -45 && _swipeAngle > -135 && y1 > 0)
        {
            y2--;
        }
        Position[] swapPositions = new Position[2];
        swapPositions[0] = new Position(x1, y1);
        swapPositions[1] = new Position(x2, y2);

        this.PostEvent(EventID.OnBoardSwap, swapPositions);
    }
    #endregion
}
