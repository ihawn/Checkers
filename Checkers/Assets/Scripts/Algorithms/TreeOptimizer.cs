using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

//GlobalProperties.SquaresPerBoardSide replaced with 8 for speed

public class TreeOptimizer
{
    //returns: position evaluation, raw board, move, piece, moves count
    //black is maximizing player
    public static MinimaxResult Minimax(MinimaxInput input, MinimaxResult minEvaluation, MinimaxResult maxEvaluation, int depth, int originalDepth, List<(int, int)> originatingMoves, bool isMaximizingPlayer, (int, int) originatingPiece, bool usePruning, int alpha, int beta, int heuristicId, int moveCount)
    {
        if(input.Board == null)
            return new MinimaxResult(moveCount);

        int blackCount = input.Board.BlackPieceCount;
        int whiteCount = input.Board.WhitePieceCount;
        if (depth == 0 || blackCount == 0 || whiteCount == 0)
            return new MinimaxResult(Heuristics.Heuristic(input.Board, heuristicId), input.Board, originatingMoves, originatingPiece, moveCount);

        if (isMaximizingPlayer)
        {
            BranchResult branchResult = GeneratePositionList(input.Board, new int[] { 1, 3 }, moveCount);
            List<MinimaxInput> branchBoards = branchResult.Branches;
            moveCount = branchResult.MoveEvaluationCount;

            foreach (MinimaxInput branch in branchBoards)
            {
                originatingMoves = depth == originalDepth ? branch.Moves : originatingMoves; //keep track of the base piece and base move
                originatingPiece = depth == originalDepth ? branch.Piece : originatingPiece;

                MinimaxResult result = Minimax(branch, minEvaluation, maxEvaluation, depth - 1, originalDepth, originatingMoves, false, originatingPiece, usePruning, alpha, beta, heuristicId, moveCount);
                moveCount = result.MoveEvaluationCount;
                maxEvaluation = result.MinimaxEvaluation > maxEvaluation.MinimaxEvaluation ? result : maxEvaluation;

                if(usePruning)
                {
                    alpha = Math.Max(alpha, maxEvaluation.MinimaxEvaluation);
                    if (beta <= alpha) //move was too good and the opponent will avoid this position
                        break;
                }
            }
            return maxEvaluation;
        }
        else
        {
            BranchResult branchResult = GeneratePositionList(input.Board, new int[] { 2, 4 }, moveCount);
            List<MinimaxInput> branchBoards = branchResult.Branches;
            moveCount = branchResult.MoveEvaluationCount;

            foreach (MinimaxInput branch in branchBoards)
            {
                originatingMoves = depth == originalDepth ? branch.Moves : originatingMoves; //keep track of the base piece and base move
                originatingPiece = depth == originalDepth ? branch.Piece : originatingPiece;

                MinimaxResult result = Minimax(branch, minEvaluation, maxEvaluation, depth - 1, originalDepth, originatingMoves, true, originatingPiece, usePruning, alpha, beta, heuristicId, moveCount);
                moveCount = result.MoveEvaluationCount;
                minEvaluation = result.MinimaxEvaluation < minEvaluation.MinimaxEvaluation ? result : minEvaluation;

                if(usePruning)
                {
                    beta = Math.Min(beta, minEvaluation.MinimaxEvaluation);
                    if (beta <= alpha) //move was too good and the opponent will avoid this position
                        break;
                }
            }
            return minEvaluation;
        }
    }

    //returns: board, move, piece | (List<(RawCheckersBoard, List<(int, int)>, (int, int))>, int)
    static BranchResult GeneratePositionList(RawCheckersBoard baseBoard, int[] whoseTurn, int moveCount)
    {
        List<MinimaxInput> boardListWithGeneratingMove = new List<MinimaxInput>();
        //List<(int, int, List<(int, int)>)> currentPlayerPiecesAndMoves = new List<(int, int, List<(int, int)>)>(); //contains the coordinates of the piece and a list of its possible moves

        for (int i = 0; i < 8; i++)
        {
            for(int j = i % 2 == 0 ? 0 : 1; j < 8; j++)
            {
                if(baseBoard.BoardMatrix[i, j] == whoseTurn[0] || baseBoard.BoardMatrix[i, j] == whoseTurn[1])
                {
                    List<(int, int)> moves = baseBoard.GetMovesForPiece((i, j));
                    foreach((int, int) move in moves)
                    {
                        RawCheckersBoard branchBoard = new RawCheckersBoard(baseBoard);
                        branchBoard.MovePiece((i, j), move);
                        List<(int, int)> movesForPiece = new List<(int, int)>() { move };

                        //multiple jumps
                        (int, int) jumpMove = move;
                        (int, int) currentPos = (i, j);
                        while(Math.Abs(currentPos.Item1 - jumpMove.Item1) == 2)
                        {
                            List<(int, int)> jumpMoves = branchBoard.GetMovesForPiece(jumpMove).Where(m => Math.Abs(jumpMove.Item1 - m.Item1) > 1.1f).ToList();
                            if (jumpMoves.Count == 0)
                                break;
                            else
                            {
                                currentPos = jumpMove;
                                jumpMove = jumpMoves[0];
                                branchBoard.MovePiece(currentPos, jumpMove);
                                movesForPiece.Add(jumpMove);
                            }
                        }

                        boardListWithGeneratingMove.Add(new MinimaxInput(branchBoard, movesForPiece, (i, j)));
                        moveCount++;
                    }
                }
            }
        }    

        try
        {
            System.Random r = new System.Random();
            boardListWithGeneratingMove.Sort((x, y) => r.Next(-1, 1));
            return new BranchResult(boardListWithGeneratingMove, moveCount);
        }
        catch
        {
            return new BranchResult(boardListWithGeneratingMove, moveCount);
        }
    }
}

//Bare bones checkers board class designed to be deep-copied quickly. Only to be used for decision tree checking
public class RawCheckersBoard
{
    //0 = empty, 1 = black, 2 = white, 3 = black king, 4 = white king
    public int[,] BoardMatrix { get; set; }
    public int BlackPieceCount { get; set; }
    public int WhitePieceCount { get; set; }

    public RawCheckersBoard(CheckersBoard oopBoard)
    {
        BoardMatrix = new int[8, 8];
        BlackPieceCount = 0;
        WhitePieceCount = 0;

        foreach(CheckersPiece piece in oopBoard.Pieces)
        {
            if(piece.IsKing)
                BoardMatrix[(int)piece.BoardPosition.x, (int)piece.BoardPosition.y] = piece.Color == Color.black ? 3 : 4;
            else
                BoardMatrix[(int)piece.BoardPosition.x, (int)piece.BoardPosition.y] = piece.Color == Color.black ? 1 : 2;
            if (piece.Color == Color.black)
                BlackPieceCount++;
            else
                WhitePieceCount++;          
        }
    }

    public RawCheckersBoard(RawCheckersBoard oldBoard)
    {
        BlackPieceCount = oldBoard.BlackPieceCount;
        WhitePieceCount = oldBoard.WhitePieceCount;

        BoardMatrix = new int[8, 8];

        for (int i = 0; i < 8; i++)
        {
            for (int j = i % 2 == 0 ? 0 : 1; j < 8; j += 2)
            {
                BoardMatrix[i, j] = oldBoard.BoardMatrix[i, j];
            }
        }
    }

    public void MovePiece((int, int) coord1, (int, int) coord2)
    {
        //movement
        int piece = BoardMatrix[coord1.Item1, coord1.Item2];
        BoardMatrix[coord2.Item1, coord2.Item2] = piece;
        BoardMatrix[coord1.Item1, coord1.Item2] = 0;

        //check jump
        if (Math.Abs(coord1.Item1 - coord2.Item1) == 2)
        {
            switch(BoardMatrix[(coord1.Item1 + coord2.Item1) / 2, (coord1.Item2 + coord2.Item2) / 2])
            {
                case 1:
                    BlackPieceCount--;
                    break;
                case 2:
                    WhitePieceCount--;
                    break;
                case 3:
                    BlackPieceCount--;
                    break;
                case 4:
                    WhitePieceCount--;
                    break;
            }

            BoardMatrix[(coord1.Item1 + coord2.Item1) / 2, (coord1.Item2 + coord2.Item2) / 2] = 0;
        }

        //check king
        if (BoardMatrix[coord2.Item1, coord2.Item2] == 1 && coord2.Item2 == 8 - 1) //black king
            BoardMatrix[coord2.Item1, coord2.Item2] = 3;
        else if (BoardMatrix[coord2.Item1, coord2.Item2] == 2 && coord2.Item2 == 0) //white king
            BoardMatrix[coord2.Item1, coord2.Item2] = 4;
    }

    public List<(int, int)> GetMovesForPiece((int, int) piece)
    {
        List<(int, int)> moves = new List<(int, int)>();
        int[] xOffset;
        int[] yOffset;
        if(BoardMatrix[piece.Item1, piece.Item2] == 3 || BoardMatrix[piece.Item1, piece.Item2] == 4) //kings
        {
            xOffset = new int[] { -1, -1, 1, 1 };
            yOffset = new int[] { 1, -1, 1, -1 };
        }
        else if(BoardMatrix[piece.Item1, piece.Item2] == 1) //black non-king
        {
            xOffset = new int[] { -1, 1 };
            yOffset = new int[] { 1, 1 };
        }
        else //white non-king
        {
            xOffset = new int[] { -1, 1 };
            yOffset = new int[] { -1, -1 };
        }

        int[] otherPiece = BoardMatrix[piece.Item1, piece.Item2] == 1 || BoardMatrix[piece.Item1, piece.Item2] == 3 ? new int[] { 2, 4 } : new int[] { 1, 3 };

        for(int i = 0; i < xOffset.Length; i++)
        {
            int newMoveX = piece.Item1 + xOffset[i];
            int newMoveY = piece.Item2 + yOffset[i];
            if (newMoveX >= 0 && newMoveX < 8 && newMoveY >= 0 && newMoveY < 8) //new move is on board
            {
                if(BoardMatrix[newMoveX, newMoveY] == otherPiece[0] || BoardMatrix[newMoveX, newMoveY] == otherPiece[1]) //if new space is occupied by other piece, check for jump
                {
                    int jumpMoveX = newMoveX + xOffset[i];
                    int jumpMoveY = newMoveY + yOffset[i];
                    if (jumpMoveX >= 0 && jumpMoveX < 8 && jumpMoveY >= 0 && jumpMoveY < 8 && BoardMatrix[jumpMoveX, jumpMoveY] == 0) //new move is on board and empty
                    {
                        moves.Add((jumpMoveX, jumpMoveY));
                    }
                }
                else if (BoardMatrix[newMoveX, newMoveY] == 0) //new space is unoccupied
                {
                    moves.Add((newMoveX, newMoveY));
                }
            }
        }

        return moves;
    }
}