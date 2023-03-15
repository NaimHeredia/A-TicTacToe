using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// This class represents a move on the "gamemoveboard" (moveBoard class).
public class Move {
	
	private float score;              // Score for this move (if board is evaluated at this move, this will contain the result of the evaluate)
	private int rowPos;             // Row on move game board
	private int colPos;             // Column on move game board
	private bool isOccupied;        // True if a player has made a move in this spot
	private int currentDepth;       // Current Depth of move to be filled in if/when this move is evaluated.

	private GameBoard.PLAYERS_ID occupiedByPlayer;  // Contains the ID of player that occupies spot
	private List<Move> children;                    // Contains the list of possible moves on board AFTER the move has been made.

	public int Depth
	{
		get{ return currentDepth; }
		set{ currentDepth = value; }
	}

	public float Score
	{
		get{ return score; }
		set{ score = value; }
	}

	public int Row
	{
		get{ return rowPos; }
		set{ rowPos = value; }
	}

	public int Column
	{
		get{ return colPos; }
		set{ colPos = value; }
	}

	public bool Occupied
	{
		get{ return isOccupied; }
		set{ isOccupied = value; }
	}

	public List<Move> PossibleMoves
	{
		get { return children; }
	}
	
	public GameBoard.PLAYERS_ID PlayerID 
	{
		get{ return occupiedByPlayer; }
		set{ occupiedByPlayer = value; }
	}
	
	public void CopyData( Move copyMove )
	{
		copyMove.Score = Score;
		copyMove.Row = Row;
		copyMove.Column = Column;
		copyMove.PlayerID = PlayerID;
		copyMove.Occupied = Occupied;
		copyMove.currentDepth= currentDepth;
		// note: children NOT copied 
	}
	
	public void ClearChildrenList()
	{
		children.Clear();
	}
	
	public Move( int row, int column )
	{
		children = new List<Move>();
		InitMove(row,column);
	}
	
	public Move()
	{
		children = new List<Move>();
		InitMove(-1,-1);
	}
	
	public void InitMove(int row, int column)
	{
		score = 0;
		rowPos = row;
		colPos = column;
		isOccupied = false;
		children.Clear();
		occupiedByPlayer = GameBoard.PLAYERS_ID.PLAYER_NONE;
		currentDepth = 0;
	}
	
	public void SetOccupied(GameBoard.PLAYERS_ID occupiedBy)
	{
		isOccupied = true;
		occupiedByPlayer = occupiedBy;
	}

	public void ClearOccupied()
	{
		isOccupied = false;
		occupiedByPlayer = GameBoard.PLAYERS_ID.PLAYER_NONE;
        children.Clear();
    }
}
