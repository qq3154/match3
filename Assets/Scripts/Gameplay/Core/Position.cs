using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Position
{
    public int x;
    public int y;   
    public Board.DotId id;
    public Board.BombId bombId;
    public int targetX;
    public int targetY;
    public Board.ComboId comboId;

    public Position(int x, int y) : this()
    {
        this.x = x;
        this.y = y;
    }
   
    public Position(int x, int y, Board.DotId id) : this()
    {
        this.x = x;
        this.y = y;
        this.id = id;
    }

    public Position(int x, int y, Board.DotId id, Board.BombId bombId) : this()
    {
        this.x = x;
        this.y = y;
        this.id = id;
        this.bombId = bombId;
    }

    public Position(int x, int y, int targetX, int targetY) : this()    
    {
        this.x = x;
        this.y = y;
        this.targetX = targetX;
        this.targetY = targetY;
    }

    public Position(int x, int y, Board.ComboId comboId) : this()
    {
        this.x = x;
        this.y = y;
        this.comboId = comboId;
    }
}
