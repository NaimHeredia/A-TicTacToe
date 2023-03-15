using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static GameBoard;
using System.Linq;
using UnityEngine.SocialPlatforms.Impl;

/*
 * Name: Naim Heredia
 * Date: October 7, 2022 (Submission Date)
 * Purpose: Implement an AI for a tictactoe game using the Mini-Max algorithm 
 * Opinion: Using this code was complicated until I had a better understanding off it, since it is really involved and not that visual
 *          debugging was a key point to understand what was happening and I believe the AI could be better with some changes but I am overall happy with the results
 * 
 * 
*/

//
// This class is created as a "scratch pad" for the AI.  Before deciding its next move, the AI will make a copy of the current board on to 
// its move board. This class has functions to determine the possible moves, based on one move on the board, and it can evaluate each move (to use with minimax) 
//

// SEARCH For TODO s for hints on where you complete you code... 
public class moveBoard {

    private const int NO_SCORE = 0;             //TODO: Determine the values for the evaluate function
    private const int WIN_SCORE = 1000;
    private const int LOSE_SCORE = -1000;
    private const int TIE_SCORE = 0;

    private Move rootMove;
    private GameBoard.PLAYERS_ID maximizingPlayer; // This is the AI player
    private bool isMoveGameFinished;
    
    public GameBoard.PLAYERS_ID MaximizingPlayer  // Keeps track of which player is maximizing (i.e. AI)
    {
        get{ return maximizingPlayer; }
        set{ maximizingPlayer = value; }
    }

    public Move Root // the root node of the game tree (very first move, depth = 0 for top of tree, the PossibleMoves for the root node will contain all possible moves for an empty game board).
    {
        get{ return rootMove; }
        set{ rootMove = value; }
    }

    public Move[,] gameMoveBoard;

    //Constructor for the moveBoard
    public moveBoard()
    {
        rootMove = new Move(-1, -1);
        gameMoveBoard = new Move[GameBoard.MAX_ROWS, GameBoard.MAX_COLUMNS];
        InitMoveBoard();
    }

    // Adds the "nextMove" for "nextMovePlayer" to the move board.  TODO: use this in the minimax function to test a move on the board
    public void makeMove( Move nextMove, GameBoard.PLAYERS_ID nextMovePlayer )
	{
		// Add move to our scratch pad game board.
		AddMovetoGameMoveBoard(nextMove,nextMovePlayer);
		
		nextMove.SetOccupied(nextMovePlayer);

		// After the nextMove is made then create a list of children with the remaining possible moves.
		AddPossibleMovesForNextMove(nextMove);
	} 

    // Clears the "nextMove" from the gameMoveBoard. TODO: use this in the minimax function to remove the move from the board so another move can be tested.
	public void RemoveMoveFromGameMoveBoard(Move nextMove)
	{
		//clear the board with the move by the currentplayer
		gameMoveBoard[nextMove.Row, nextMove.Column].ClearOccupied();
	}
	

	//Initializes the move board ( this is the scratch pad for the AI )
	public void InitMoveBoard()
	{
		rootMove.ClearChildrenList();
		isMoveGameFinished = false;
		maximizingPlayer = GameBoard.PLAYERS_ID.PLAYER_NONE;
		
		for (int i = 0; i < GameBoard.MAX_ROWS; i++)
		{
			for(int j = 0; j < GameBoard.MAX_COLUMNS; j++)
			{
				gameMoveBoard[i,j] = new Move(i,j);  //Inits move for scratch pad gameMoveBoard
				gameMoveBoard[i,j].Score = 0;
			}
		}
	}

	// Copies the actual game board to the scratch pad (gameMoveBoard) so AI can look ahead. 
	public void SetCurrentGameBoardToMoveBoard(GameBoard.PLAYERS_ID initPlayer)
	{
		for (int i = 0; i < GameBoard.MAX_ROWS; i++)
		{
			for(int j = 0; j < GameBoard.MAX_COLUMNS; j++)
			{
				gameMoveBoard[i,j].Occupied = GameBoard.gameBoard[i,j].Occupied;
				gameMoveBoard[i,j].PlayerID = GameBoard.gameBoard[i,j].PlayerID;
			}
		}
		// Set up the root node to match the current game board. 
		MaximizingPlayer 	= initPlayer;
		rootMove.PlayerID 	= initPlayer;
		rootMove.Score 		= NO_SCORE;
		AddPossibleMovesForNextMove(rootMove);
	}


    // The Evaluation function will calculate a score for the board and return it so a decision can be made for the AI for the best next move
    public float EvalBoardForAI()
    {
        float boardScore = TIE_SCORE;

        float enemyScore = TIE_SCORE;

        float AiScore = TIE_SCORE;
        //
        // TODO: Complete this function with your OWN code. 
        // Determine if there is a win, loss or tie and return appropriate score
        // If not a win loss or tie, evaluate the board based on your OWN strategy (how do you play the game?)
        // and return score accordingly
        // 

        // Check if there is a win, lose or tie and setting their scores
            // if there is none of those options we calculate a score for the board based on the previous moves

        if (CheckForWinner(GameBoard.PLAYERS_ID.PLAYER_ONE))
        {
            boardScore = LOSE_SCORE;
        }
        else if (CheckForWinner(GameBoard.PLAYERS_ID.PLAYER_TWO))
        {
            boardScore = WIN_SCORE;
        }
        else if (CheckForTie())
        {
            boardScore = TIE_SCORE;
        }        
        else
        {
            // Calculate the score for both players
            enemyScore = ScoreCurrentBoard(PLAYERS_ID.PLAYER_ONE);
            AiScore = ScoreCurrentBoard(PLAYERS_ID.PLAYER_TWO);

            // Return the best score for the AI (always player 2)
            boardScore = AiScore - enemyScore;
        }        

        return boardScore;
    }

    // Check if the game has come to an end by analizyng the board

    public bool IsGameOver()
    {
        isMoveGameFinished = false;
        //
        // TODO: Determing when the game is over on the AI board (scratch board) gameMoveBoard, will need for MiniMax function in gameboard.cs
        // Will need to use functions that check if there is a win, loss or tie (see TODO CHECK functions at end of file), if there is a win/loss or tie return true
        //

        if(CheckForWinner(GameBoard.PLAYERS_ID.PLAYER_ONE))
        {
            isMoveGameFinished = true;
        }
        else if (CheckForWinner(GameBoard.PLAYERS_ID.PLAYER_TWO))
        {
            isMoveGameFinished = true;
        }
        else if(CheckForTie())
        {
            isMoveGameFinished = true;
        }

        return isMoveGameFinished;
    }

    // Goes through the entire board and sets all the possible moves for "nextMove".
    private void AddPossibleMovesForNextMove(Move nextMove)
    {
        for (int i = 0; i < GameBoard.MAX_ROWS; i++)
        {
            for (int j = 0; j < GameBoard.MAX_COLUMNS; j++)
            {
                if (!gameMoveBoard[i, j].Occupied)
                {
                    Move posMove = new Move();
                    gameMoveBoard[i, j].CopyData(posMove);
                    nextMove.PossibleMoves.Add(posMove);
                }
            }
        }
    }

    // Directly sets the nextMovePlayer to the nextMove position on the gameMoveBoard.
    private void AddMovetoGameMoveBoard(Move nextMove, GameBoard.PLAYERS_ID nextMovePlayer)
    {
        // Update the board with the move by the currentplayer.
        gameMoveBoard[nextMove.Row, nextMove.Column].SetOccupied(nextMovePlayer);        
    }
    
    // Check if the board has a tie
    private bool CheckForTie( )
	{
		bool gameTied = true;        
        
        //Assumes that there is not a win  on the board (assumes other function checks for win).  
        //If there is not a win or loss on the board and the there are no unoccupied spots then there is a tie.
		for (int i = 0; i < GameBoard.MAX_ROWS; i++)
		{
			for(int j = 0; j < GameBoard.MAX_COLUMNS; j++)
			{
				if(!(gameMoveBoard[i,j].Occupied) )
				{
                    return false;                    
				}
			}
		}
		return gameTied;
	}

    //checks the actual game board for a winner
    private bool CheckForWinner(PLAYERS_ID playerTurnID)
    {
        bool bWon = false;
        //PLAYERS_ID playerTurnID = playerGamePiece.PlayerID;      

        for (int i = 0; i < MAX_ROWS; i++)
        {
            for (int j = 0; j < MAX_COLUMNS; j++)
            {
                Move gamePiece = gameMoveBoard[i, j];
                if (playerTurnID == gamePiece.PlayerID)
                {
                    int foundMatchCount = 0;
                    //Check above
                    if (CheckPieces(gamePiece, ref foundMatchCount, playerTurnID, -1, 0))
                    {
                        bWon = true;
                        //found a winner 
                        break;
                    }
                    foundMatchCount = 0;
                    //Check below
                    if (CheckPieces(gamePiece, ref foundMatchCount, playerTurnID, 1, 0))
                    {
                        //found a winner 
                        bWon = true;
                        break;
                    }
                    foundMatchCount = 0;

                    //Check right
                    if (CheckPieces(gamePiece, ref foundMatchCount, playerTurnID, 0, 1))
                    {
                        //found a winner 
                        bWon = true;
                        break;
                    }
                    foundMatchCount = 0;

                    //Check Left
                    if (CheckPieces(gamePiece, ref foundMatchCount, playerTurnID, 0, -1))
                    {
                        //found a winner 
                        bWon = true;
                        break;
                    }
                    foundMatchCount = 0;

                    //check upper right (above, right)
                    if (CheckPieces(gamePiece, ref foundMatchCount, playerTurnID, -1, 1))
                    {
                        //found a winner 
                        bWon = true;
                        break;
                    }

                    foundMatchCount = 0;
                    //check upper left (above, left)
                    if (CheckPieces(gamePiece, ref foundMatchCount, playerTurnID, -1, -1))
                    {
                        //found a winner 
                        bWon = true;
                        break;
                    }

                    foundMatchCount = 0;
                    //check lower right (below, right)
                    if (CheckPieces(gamePiece, ref foundMatchCount, playerTurnID, 1, 1))
                    {
                        //found a winner 
                        bWon = true;
                        break;
                    }

                    foundMatchCount = 0;
                    //check lower left (below, left)
                    if (CheckPieces(gamePiece, ref foundMatchCount, playerTurnID, 1, -1))
                    {
                        //found a winner 
                        bWon = true;                        
                        break;
                    }
                }
            }
            if (bWon)
            {
                break;
            }
        }

        return bWon;
    }
    // Checks for matches for a win for playerTurnID, in the direction of rowOffset, and colOffset
    private bool CheckPieces(Move playerGamePiece, ref int foundMatchCount, PLAYERS_ID playerTurnID, int rowOffset, int colOffset)
    {
        bool bFoundWin = false;
        if (playerGamePiece.PlayerID == playerTurnID)
        {
            foundMatchCount++;
            if (foundMatchCount == CONNECT_NUM)
            {
                bFoundWin = true;
            }
            else if ((playerGamePiece.Row >= 0 && playerGamePiece.Row < MAX_ROWS) && (playerGamePiece.Column >= 0 && playerGamePiece.Column < MAX_COLUMNS)
                    && foundMatchCount < CONNECT_NUM)
            {
                if (((playerGamePiece.Row + rowOffset) >= 0 && (playerGamePiece.Row + rowOffset) < MAX_ROWS) && ((playerGamePiece.Column + colOffset) >= 0 && (playerGamePiece.Column + colOffset) < MAX_COLUMNS))
                    bFoundWin = CheckPieces(gameMoveBoard[playerGamePiece.Row + rowOffset, playerGamePiece.Column + colOffset], ref foundMatchCount, playerTurnID, rowOffset, colOffset);
            }
            if (bFoundWin)
            {
                
            }
        }
        return bFoundWin;
    }

    // Scores the board by counting and adding up the number of moves on the board that benefit the  current player selected
    // The checks are done on all possible wins giving a high score to multiple moves in a possible win scenario (Rows, Columns, and Diagonals)
    private float ScoreCurrentBoard(PLAYERS_ID id)
    {
        /*
         * This function will create an array of scores to give to each row, column and diagonal then add the score and return it, the more moves there is in a single
         * row/column/diagonal the score will increase
         * Both the rows and columns use variables so the function works no matter the size of the board, the diagonals have a set starting point since the diagonals will only
         * increase in depth but never change their position.
        */
        // Variables
        float[] scores = new float[GameBoard.MAX_COLUMNS + GameBoard.MAX_ROWS + 2];

        int scoresIndex = 0;

        int moveCount = 0;

        int row = 0, col = 0;

        // CHECK ROWS
        for (int i = 0; i < GameBoard.MAX_ROWS; i++)
        {
            for (int j = 0; j < GameBoard.MAX_COLUMNS; j++)
            {
                if (gameMoveBoard[i, j].PlayerID == id)
                {
                    if (scores[scoresIndex] == 0)
                    {
                        scores[scoresIndex]++;
                        moveCount++;
                    }
                    else
                    {
                        scores[scoresIndex] += scores[scoresIndex] * ((float)moveCount / 100f);
                        moveCount++;
                    }
                }
            }
            scoresIndex++;
            moveCount = 0;
        }

        // CHECK COLUMNS
        for (int i = 0; i < GameBoard.MAX_ROWS; i++)
        {
            for (int j = 0; j < GameBoard.MAX_COLUMNS; j++)
            {
                if (gameMoveBoard[j, i].PlayerID == id)
                {
                    if (scores[scoresIndex] == 0)
                    {
                        scores[scoresIndex]++;
                        moveCount++;
                    }
                    else
                    {
                        scores[scoresIndex] += scores[scoresIndex] * ((float)moveCount / 100f);
                        moveCount++;
                    }
                }
            }
            scoresIndex++;
            moveCount = 0;
        }

        // CHECK DIAGONALS

        // 1

        while (row < MAX_ROWS && col < MAX_COLUMNS)
        {
            if (gameMoveBoard[row, col].PlayerID == id)
            {

                if (scores[scoresIndex] == 0)
                {
                    scores[scoresIndex]++;
                    moveCount++;
                }
                else
                {
                    scores[scoresIndex] += scores[scoresIndex] * ((float)moveCount / 100f);
                    moveCount++;                    
                }
            }
            row++;
            col++;
        }

        // Reset the row and col variables for the opposite diagonal
        row = 0;
        col = MAX_COLUMNS;

        // 2
        while (row < MAX_ROWS && col <= 0)
        {
            if (gameMoveBoard[row, col].PlayerID == id)
            {
                if (scores[scoresIndex] == 0)
                {
                    scores[scoresIndex]++;
                    moveCount++;
                }
                else
                {
                    scores[scoresIndex] += scores[scoresIndex] * ((float)moveCount / 100f);
                    moveCount++;                    
                }
            }
            row++;
            col--;
        }

        return scores.Sum();
    }
}