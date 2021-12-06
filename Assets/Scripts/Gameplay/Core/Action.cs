using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// This class is used to store all actions the board made
/// then the View  will simulate all the actions above
/// </summary>
public class Action
{
    public LinkedList<Position> comboPositions;

    public LinkedList<Position> bombPositions;

    public LinkedList<Position> explodePositions;

    public LinkedList<Position> destroyWhenCreateBombPositions;

    public LinkedList<Position> destroyPositions;

    public LinkedList<Position> refillPositions;

    public LinkedList<Position> pulldownPositions;

    //Constructor
    public Action()
    {
        this.comboPositions = new LinkedList<Position>();
        this.bombPositions = new LinkedList<Position>();
        this.explodePositions = new LinkedList<Position>();
        this.destroyWhenCreateBombPositions = new LinkedList<Position>();
        this.destroyPositions = new LinkedList<Position>();
        this.refillPositions = new LinkedList<Position>();
        this.pulldownPositions = new LinkedList<Position>();
    }
}
