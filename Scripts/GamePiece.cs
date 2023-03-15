using UnityEngine;
using System.Collections;

// This script is attached to each game piece or actual board position (game object).  Together, each game piece makes up the tic tack toe board
// see gameboard in GameBoard script.

public class GamePiece : MonoBehaviour {
	
    // Sets boundary for game board.
	public const float BOUND_MIN_X= -12;
	public const float BOUND_MAX_X= 15;
	public const float BOUND_MIN_Z= -15;
	public const float BOUND_MAX_Z= 10;

	private const float YPos = 2.0f;
	private GameBoard.PLAYERS_ID playerID;          // Stores the player ID of that occupies game piece/spot
	private bool isOccupied = false;                // True if player chooses a this spot
	private Vector3 endPosition = Vector3.zero;     // End position of piece. Allows for movement of board pieces if required
	private int row;                                // Specifies the row and column position on board grid
	private int column;

	public float spaceBetween = 1;

	public GameBoard.PLAYERS_ID PlayerID 
	{
		get{ return playerID; }
		set{ playerID = value; }
	}
	
	public int Row 
	{
		get{ return row; }
		set{ row = value; }
	}
	
	public int Column 
	{
		get{ return column; }
		set{ column = value; }
	}
	
	public bool Occupied 
	{
		get{ return isOccupied; }
		set{ isOccupied = value; }
	}
	
	public void SetEndPosition( Vector3 endPos )
	{
		endPosition = endPos;
	}
	
	public void SetParameters(int boardRow, int boardColumn )
	{
		row =  boardRow;
		column = boardColumn;
	}
    
    public void SetColor(Color newColor)
    {
        GetComponent<Renderer>().material.color = newColor;
    }

    // Assigns the square to be occupied by playerNum.
    public void AddPlayerToSquare(GameBoard.PLAYERS_ID playerNum, int boardRow, int boardColumn)
    {
        isOccupied = true;
        playerID = playerNum;
        row = boardRow;
        column = boardColumn;
    }

    // Initialize the "square" for game board
    public void InitPlayerSquare(int boardRow, int boardColumn)
    {
        isOccupied = false;
        playerID = GameBoard.PLAYERS_ID.PLAYER_NONE;
        row = boardRow;
        column = boardColumn;
        SetGamePiecePositions();
        SetParameters(boardRow, boardColumn);
        GetComponent<Renderer>().material.color = new Color(255, 255, 0);
    }

    void Awake()
    {
        spaceBetween = 4;
        row = -1;
        column = -1;
        isOccupied = false;
    }

	private void OnMouseDown()
	{
		print("game piece " + " row " + row + " column " + column );
		GameBoard.BoardClicked( row, column, isOccupied, GameBoard.PLAYERS_ID.PLAYER_ONE );
	}
	
    // Uses the column, row data to set positions for game piece.
	private void SetGamePiecePositions()
	{
		float endX = GamePiece.BOUND_MIN_X + (column * spaceBetween);
		float endZ = GamePiece.BOUND_MAX_Z - row * spaceBetween;
		
		SetEndPosition(new Vector3(endX,YPos,endZ));
		transform.position = endPosition;
	}
}
