using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This class maintains the game using the ACTUAL game board for TicTacToe. It allows each player to take a turn and determines
// whether there is a win/loss or tie. It also allows for restarting new games.   It contains the minimax function for the AI. 
// You will need to complete the minimx funtion. 

public class GameBoard : MonoBehaviour {
	
	public enum PLAYERS_ID { PLAYER_NONE = 0, PLAYER_ONE ,PLAYER_TWO };  // player one is the user, player two is the AI
	
	public const int MAX_COLUMNS = 3; 	// Max width of game board  (set to 4 for 4x4 grid)
	public const int MAX_ROWS = 3;      // Max height of game board (set to 4 for 4x4 grid)
    public const int CONNECT_NUM = 3;   // Number of squares to connect in row in order to win (set to 4 for 4x4 grid)
    public const int MAX_DEPTH = 5;     // max depth level for miniMax algorithm
										// use this value to set the max search in your tree for the minimax algorithm.  
										// Test your solution by setting this value to 1,3,5,9 for 3x3 game board.  
										// Set this value to 3,4,5 for testing with a 4x4 board. 
										// Higher values will cause SIGNIFICANT slow down on a 4x4 grid.

	public const int INFINITY = int.MaxValue;

	static bool gameFinished = false;											  // true if game is over
	static public GamePiece[,] gameBoard = new GamePiece[MAX_ROWS, MAX_COLUMNS];  // 2D array/grid representing the ACTUAL game board
	static PLAYERS_ID winningPlayer = PLAYERS_ID.PLAYER_NONE;					  // stores the winner of each game played

	public GameObject gamePiecePrefab;                                            // prefab representing one game piece (or board space) 

	// Use this for initialization
	void Start()
	{
		InitGameBoard(true);
	}

	//
	//This function is used to claim a move (by playerID) on the actual game board
	//
	static public void BoardClicked( int clickedRow, int clickedColumn, bool isOccupied, PLAYERS_ID playerID )
	{
        // if the game isn't finished and a valid, unoccupied spot was clicked
		if (!gameFinished && clickedColumn >= 0 && clickedColumn < MAX_COLUMNS && clickedRow >= 0 && clickedRow < MAX_ROWS )
		{
			if ( !isOccupied  )
			{
				Color colourPiece;  //Find the colour for  specific player
				if(playerID == PLAYERS_ID.PLAYER_TWO)
				{
					colourPiece = new Color(0,0,255);
				}
				else
				{
					colourPiece = new Color(255,0,0);
				}
				
                // Add the player to the game board 
				gameBoard[clickedRow,clickedColumn].AddPlayerToSquare(playerID,clickedRow,clickedColumn);
				gameBoard[clickedRow,clickedColumn].SetColor(colourPiece);
				
                // Check for win 
				if ( CheckForWinner(gameBoard[clickedRow,clickedColumn]) )
				{
					print ("Player " + playerID + " has won!!!!") ;
					winningPlayer = playerID;
					gameFinished = true;
				}
				else if( CheckForTie())  
				{
					winningPlayer = PLAYERS_ID.PLAYER_NONE;
					print ("GAME TIED!!!!") ;
					gameFinished = true;
				}
			}
		}
	}
    //
    //This function implements the Minimax algorithm
    //
    //moveboard board   --represents the current game board as a scratch pad to make moves on (before actually making the move)
    //                  --board also holds the actual player's turn for the game (first player in the tree)
    //Move nextMove     --represents the next move to be made on the game board
    //                  --nexMove also holds a score value for the move
    //                  --PossibleMoves in nextMove represents the children of the "nextMove" (if this is empty then there are no moves to make, 
    //                    if there are children then each child represents a possible to move to make)
    //
    //currentPlayerToEvaluate    --represents the players turn on the tree ( it will change as we traverse each level of the tree )
    //
    //maxDepth          --represents the number of levels to use in the minimax algorithm (how far down the tree do we want to go?)
    //
    //currentDepth      --represents the current leve we are evaluating
    //
    //return            --this function returns the best move to make on the board

    static public Move MiniMax( moveBoard board, Move nextMove, GameBoard.PLAYERS_ID currentPlayerToEvaluate, int maxDepth, int currentDepth, float alpha, float beta )
	{
		//
        //create a bestmove to return
        //
		Move bestMove = new Move();


        //if game is over ( no more moves left tie/win/loss ) || we have reached our max depth to search i.e. currentDepth == maxDepth

		if(board.IsGameOver() || currentDepth == maxDepth)
        {
			// STOP RECURSION: 
			// Evaluate the board (i.e. int score = board.EvalBoardForAI() )
			// STRATEGY

			float score = board.EvalBoardForAI();
			nextMove.Score = score;
			nextMove.Depth = currentDepth;
			nextMove.CopyData(bestMove);
        }
        else
        {
			//
			// We haven't reached a terminal node  or the end of the game so traverse the children of "nextMove" and call minimax on each child.
			//

			// BEFORE LOOPING: 

			//
			// Initialize the bestmove "so far" object 
			//

			nextMove.CopyData(bestMove);
			bestMove.Score = INFINITY;      // Used when finding the min score

			//
			//  Ask: whose turn is it on the tree? Keep track of the max or min score for all the possible moves (children)
			//
			if (board.MaximizingPlayer == currentPlayerToEvaluate)
			{
                //init value to keep track of max score i.e.bestMove.Score = -INFINITY;
                bestMove.Score = -INFINITY;
            }
					

			//
			// START LOOP of moves in possibles moves (loop through siblings, that is the nextMove.PossibleMoves list),these are the children of "nextMove" : 
			//
			foreach(Move move in nextMove.PossibleMoves)
			{
				Move currBubbleScore; // Kepps track of score returned by minimax - also keeps track of its depth

                //
                //for each possible move, "move", make the "move" ( take the turn  on the scratch pad ) before calling minimax for the that "move".
                // i.e. board.makeMove(move, currentPlayerToEvaluate);
                // 

                board.makeMove(move, currentPlayerToEvaluate);

                //
                // CALL miniMax here using the board you just made a move on and the "move" (remember to switch to next player (GetNextPlayerID()) and to ADD one to depth value).
                //

                currBubbleScore = MiniMax(board, move, GetNextPlayerID(currentPlayerToEvaluate), maxDepth, currentDepth + 1, -INFINITY, INFINITY);

                // NOTE: miniMax will return a score for the board with the move that was just made. 
                // We need to keep track of the scores for the siblings (all moves in PossibleMoves) to 
                // find either the max score of all the siblings or the min score of all the siblings (this will depend on which player is making the move).
                //

                //
                // Remove move from our game board (game board scratch pad) so that the next sibling can make its move.
                // i.e. board.RemoveMoveFromGameMoveBoard(move);
                //

                board.RemoveMoveFromGameMoveBoard(move);

                //
                //if the current player to evaluate is the AI (MaximizingPlayer), 
                if (board.MaximizingPlayer == currentPlayerToEvaluate)
				{
					if(currBubbleScore.Score >= bestMove.Score)
					{
						bestMove.Score = currBubbleScore.Score;
						bestMove.Depth = currBubbleScore.Depth;
						bestMove.Row = move.Row;
						bestMove.Column = move.Column;
					}
				}
				else
				{
                    if (currBubbleScore.Score <= bestMove.Score)
                    {
                        bestMove.Score = currBubbleScore.Score;
                        bestMove.Depth = currBubbleScore.Depth;
                        bestMove.Row = move.Row;
                        bestMove.Column = move.Column;
                    }
                }
                //take the highest score found of all the moves
                //
                //else the opponent is the current player to evaluate so 
                //  take the lowest score of all the moves
                //

            }// END LOOP of PossibleMoves  
        }

		return bestMove;
	}
	
    // Used to get the next player (used in turn taking)
	static public PLAYERS_ID GetNextPlayerID( PLAYERS_ID currPlayer )
	{
		PLAYERS_ID nextPlayer = GameBoard.PLAYERS_ID.PLAYER_NONE;

		//Update the current player to the next player
		if( currPlayer == GameBoard.PLAYERS_ID.PLAYER_ONE)
		{
			nextPlayer = GameBoard.PLAYERS_ID.PLAYER_TWO;
		}
		else if (currPlayer == GameBoard.PLAYERS_ID.PLAYER_TWO)
		{
			nextPlayer = GameBoard.PLAYERS_ID.PLAYER_ONE;
		}
		return nextPlayer;
	}
	
	// This function is called by the AI when it is the AI's turn to "click" the board.
	static Move GetBestMove( GameBoard.PLAYERS_ID player )
	{
		//make a moveBoard of the current state of the board (it will be used as a scratch pad for minimax).
		moveBoard currentMoveBoard = new moveBoard();
		//this move board is used as a scratch pad to examine possible next moves, copy the current ACTUAL board onto the moveboard (scratch pad)
		currentMoveBoard.SetCurrentGameBoardToMoveBoard(player);
        //using the moveboard the minimax function finds the next BEST move
		Move bestMove = MiniMax( currentMoveBoard, currentMoveBoard.Root, player, MAX_DEPTH, 0, -INFINITY, INFINITY);
        // once the minimax function returns with the best move, return this move 
		return bestMove;
	} 
	
	static public void AI_BoardTurn( )
	{
		if (!gameFinished)
		{
			//Get the best move from the AI. Player_TWO is the AI and will be "maximing" in the minimax function
			Move bestMove = GetBestMove(PLAYERS_ID.PLAYER_TWO);
            // bestMove will contain the next best move for the AI
            // Make the best move on the ACTUAL board	
            BoardClicked(bestMove.Row, bestMove.Column, false, PLAYERS_ID.PLAYER_TWO );
		}
	}
	
	//If there are no more moves and no winner/loser then there is a tie
	static bool CheckForTie( )
	{
		bool gameTied = true;
        //Assumes that there is not a win  on the board (assumes other function checks for win).  
        //If there is not a win or loss on the board and the there are no unoccupied spots then there is a tie.
		for (int i = 0; i < MAX_ROWS; i++)
		{
			for(int j = 0; j < MAX_COLUMNS; j++)
			{
				if(!(gameBoard[i,j].Occupied) )
				{
					gameTied = false;
				}
			}
		}
		return gameTied;
	}

	//checks the actual game board for a winner
	static bool CheckForWinner( GamePiece playerGamePiece )
	{
		bool bWon = false;
		PLAYERS_ID playerTurnID = playerGamePiece.PlayerID;
		// **************************************************************************
		// NOTE: This can be simplified (more efficient) for the game of Tic Tac Toe
		// For example, it does not have to check ALL directions for every piece to
		// determine a win on the board.
		// **************************************************************************

		for (int i = 0; i < MAX_ROWS; i++)
		{
			for(int j = 0; j < MAX_COLUMNS; j++)
			{
				GamePiece gamePiece = gameBoard[i,j];
				if( playerTurnID == gamePiece.PlayerID )
				{
					int foundMatchCount = 0;
					//Check above
					if( CheckPieces( gamePiece,ref foundMatchCount, playerTurnID, -1,0 ) )
					{
						bWon = true;
						//found a winner 
						break;
					}
					foundMatchCount = 0;
					//Check below
					if( CheckPieces( gamePiece, ref foundMatchCount,playerTurnID, 1, 0) )
					{
						//found a winner 
						bWon = true;
						break;
					}
					foundMatchCount = 0;
					
					//Check right
					if( CheckPieces( gamePiece, ref foundMatchCount,playerTurnID, 0, 1) )
					{
						//found a winner 
						bWon = true;
						break;
					}
					foundMatchCount = 0;
					
					//Check Left
					if( CheckPieces( gamePiece, ref foundMatchCount,playerTurnID, 0, -1) )
					{
						//found a winner 
						bWon = true;
						break;
					}
					foundMatchCount = 0;
					
					//check upper right (above, right)
					if( CheckPieces( gamePiece, ref foundMatchCount,playerTurnID, -1,1) )
					{
						//found a winner 
						bWon = true;
						break;
					}

					foundMatchCount = 0;
					//check upper left (above, left)
					if( CheckPieces( gamePiece, ref foundMatchCount, playerTurnID, -1,-1) )
					{
						//found a winner 
						bWon = true;
						break;
					}

					foundMatchCount = 0;
					//check lower right (below, right)
					if( CheckPieces( gamePiece, ref foundMatchCount,playerTurnID, 1, 1) )
					{
						//found a winner 
						bWon = true;
						break;
					}
					
					foundMatchCount = 0;
					//check lower left (below, left)
					if( CheckPieces( gamePiece,ref foundMatchCount,playerTurnID, 1, -1) )
					{
						//found a winner 
						bWon = true;
						break;
					}
				}
			}
			if(bWon)
			{
				break;
			}
		}
		
		return bWon;
	}
    // Checks for matches for a win for playerTurnID, in the direction of rowOffset, and colOffset
    static bool CheckPieces(GamePiece playerGamePiece, ref int foundMatchCount, PLAYERS_ID playerTurnID, int rowOffset, int colOffset)
    {
        bool bFoundWin = false;
        if (playerGamePiece.PlayerID == playerTurnID)
        {
            foundMatchCount++;
            if (foundMatchCount == CONNECT_NUM)
            {
                bFoundWin = true;
            }
            else if ( (playerGamePiece.Row >= 0 && playerGamePiece.Row < MAX_ROWS) && (playerGamePiece.Column >= 0 && playerGamePiece.Column < MAX_COLUMNS)
                    && foundMatchCount < CONNECT_NUM )
            {
                if ( ((playerGamePiece.Row + rowOffset) >= 0 && (playerGamePiece.Row + rowOffset) < MAX_ROWS) && ((playerGamePiece.Column + colOffset) >= 0 && (playerGamePiece.Column + colOffset) < MAX_COLUMNS))
                    bFoundWin = CheckPieces(gameBoard[playerGamePiece.Row + rowOffset, playerGamePiece.Column + colOffset], ref foundMatchCount, playerTurnID, rowOffset, colOffset);
            }
            if (bFoundWin)
            {
                playerGamePiece.SetColor(new Color(0, 255, 0));
            }
        }
        return bFoundWin;
    }

    //Initialize the actual game board
    void InitGameBoard(bool init)
    {
        gameFinished = false;
        winningPlayer = PLAYERS_ID.PLAYER_NONE;

        for (int i = 0; i < MAX_ROWS; i++)
        {
            for (int j = 0; j < MAX_COLUMNS; j++)
            {
                if (init)
                {
                    GameObject gamePieceObj = (GameObject)Instantiate(gamePiecePrefab);
                    gameBoard[i, j] = gamePieceObj.GetComponent<GamePiece>();
                }
                gameBoard[i, j].InitPlayerSquare(i, j);
            }
        }
    }
    //Update the UI
    void OnGUI(){
		
		bool newGameButtonClicked = GUILayout.Button("NewGame");
		bool AITurnButtonClicked = GUILayout.Button("AI Turn");
		
        //Start a new game if necessary
		if(newGameButtonClicked)
		{
			InitGameBoard(false);
		}
		
        //Let the AI take a turn
		if(AITurnButtonClicked)
		{
			AI_BoardTurn();
		}

		//check for game end
		if (gameFinished)
		{
			if (winningPlayer == PLAYERS_ID.PLAYER_NONE )
			{
				 GUI.Label( new Rect(0,300,300,300), "Game Tied!");
			}
			else if (winningPlayer == PLAYERS_ID.PLAYER_ONE )
			{
				 GUI.Label (new Rect(0,300,300,300), "Player has won!");
			}
			else if(winningPlayer == PLAYERS_ID.PLAYER_TWO)
			{
				 GUI.Label (new Rect(0,300,300,300), "AI has won!");
			}
		}
	}
	
	
	
}

