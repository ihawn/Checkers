using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class TreeOptimizer
{
    //returns: position evaluation, raw board, move, piece, moves count
    public static (int, RawCheckersBoard, (int, int), (int, int), int) Minimax((RawCheckersBoard, (int, int), (int, int)) board, (int, RawCheckersBoard, (int, int), (int, int)) minEvaluation, (int, RawCheckersBoard, (int, int), (int, int)) maxEvaluation, int depth, int originalDepth, (int, int) originatingMove, bool isMaximizingPlayer, (int, int) originatingPiece, bool usePruning, int alpha, int beta, int heuristicId, int moveCount)
    {
        if(board.Item1 == null)
            return (0, null, (-1, -1), (-1, -1), moveCount);

        int blackCount = board.Item1.BlackPieceCount;
        int whiteCount = board.Item1.WhitePieceCount;
        if (depth == 0 || blackCount == 0 || whiteCount == 0)
            return (Heuristics.Heuristic(board.Item1, heuristicId), board.Item1, originatingMove, originatingPiece, moveCount); //black player wants to maximize this value

        if (isMaximizingPlayer)
        {
            (List<(RawCheckersBoard, (int, int), (int, int))>, int) branchResult = GeneratePositionList(board.Item1, new int[] { 1, 3 }, moveCount);
            List<(RawCheckersBoard, (int, int), (int, int))> branchBoards = branchResult.Item1;
            moveCount = branchResult.Item2;

            foreach ((RawCheckersBoard, (int, int), (int, int)) branch in branchBoards)
            {
                originatingMove = depth == originalDepth ? branch.Item2 : originatingMove; //keep track of the base piece and base move
                originatingPiece = depth == originalDepth ? branch.Item3 : originatingPiece;

                (int, RawCheckersBoard, (int, int), (int, int), int) result = Minimax(branch, minEvaluation, maxEvaluation, depth - 1, originalDepth, originatingMove, false, originatingPiece, usePruning, alpha, beta, heuristicId, moveCount);
                var eval = (result.Item1, result.Item2, result.Item3, result.Item4);
                moveCount = result.Item5;
                maxEvaluation = eval.Item1 > maxEvaluation.Item1 ? eval : maxEvaluation;

                if(usePruning)
                {
                    alpha = Math.Max(alpha, eval.Item1);
                    if (beta <= alpha)
                        break;
                }
            }
            return (maxEvaluation.Item1, maxEvaluation.Item2, maxEvaluation.Item3, maxEvaluation.Item4, moveCount);
        }
        else
        {
            (List<(RawCheckersBoard, (int, int), (int, int))>, int) branchResult = GeneratePositionList(board.Item1, new int[] { 2, 4 }, moveCount);
            List<(RawCheckersBoard, (int, int), (int, int))> branchBoards = branchResult.Item1;
            moveCount = branchResult.Item2;

            foreach ((RawCheckersBoard, (int, int), (int, int)) branch in branchBoards)
            {
                originatingMove = depth == originalDepth ? branch.Item2 : originatingMove; //keep track of the base piece and base move
                originatingPiece = depth == originalDepth ? branch.Item3 : originatingPiece;

                (int, RawCheckersBoard, (int, int), (int, int), int) result = Minimax(branch, minEvaluation, maxEvaluation, depth - 1, originalDepth, originatingMove, true, originatingPiece, usePruning, alpha, beta, heuristicId, moveCount);
                var eval = (result.Item1, result.Item2, result.Item3, result.Item4);
                moveCount = result.Item5;
                minEvaluation = eval.Item1 < minEvaluation.Item1 ? eval : minEvaluation;

                if(usePruning)
                {
                    beta = Math.Min(beta, eval.Item1);
                    if (beta <= alpha)
                        break;
                }
            }
            return (minEvaluation.Item1, minEvaluation.Item2, minEvaluation.Item3, minEvaluation.Item4, moveCount);
        }
    }


    //returns: board, move, piece
    static (List<(RawCheckersBoard, (int, int), (int, int))>, int) GeneratePositionList(RawCheckersBoard baseBoard, int[] whoseTurn, int moveCount)
    {
        List<(RawCheckersBoard, (int, int), (int, int))> boardListWithGeneratingMove = new List<(RawCheckersBoard, (int, int), (int, int))>();
        List<(int, int, List<(int, int)>)> currentPlayerPiecesAndMoves = new List<(int, int, List<(int, int)>)>(); //contains the coordinates of the piece and a list of its possible moves

        for (int i = 0; i < GlobalProperties.SquaresPerBoardSide; i++)
        {
            for(int j = 0; j < GlobalProperties.SquaresPerBoardSide; j++)
            {
                if(whoseTurn.Contains(baseBoard.BoardMatrix[i,j]))
                {
                    List<(int, int)> moves = baseBoard.GetMovesForPiece((i, j));
                    foreach((int, int) move in moves)
                    {
                        RawCheckersBoard branchBoard = ObjectExtensions.Copy(baseBoard);
                        branchBoard.MovePiece((i, j), move);
                        boardListWithGeneratingMove.Add((branchBoard, move, (i, j)));
                        moveCount++;
                    }
                }
            }
        }    

        try
        {
            System.Random r = new System.Random();
            boardListWithGeneratingMove.Sort((x, y) => r.Next(-1, 1));
            return (boardListWithGeneratingMove, moveCount);
        }
        catch
        {
            return (boardListWithGeneratingMove, moveCount);
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
        BoardMatrix = new int[GlobalProperties.SquaresPerBoardSide, GlobalProperties.SquaresPerBoardSide];
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
        if (BoardMatrix[coord2.Item1, coord2.Item2] == 1 && coord2.Item2 == GlobalProperties.SquaresPerBoardSide - 1) //black king
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

        int[] otherPiece = BoardMatrix[piece.Item1, piece.Item2] == 1 || BoardMatrix[piece.Item1, piece.Item2] == 3 ? new int[] { 2, 4 } : new int[] { 1, 3};

        for(int i = 0; i < xOffset.Length; i++)
        {
            int newMoveX = piece.Item1 + xOffset[i];
            int newMoveY = piece.Item2 + yOffset[i];
            if (newMoveX >= 0 && newMoveX < GlobalProperties.SquaresPerBoardSide && newMoveY >= 0 && newMoveY < GlobalProperties.SquaresPerBoardSide) //new move is on board
            {
                if(BoardMatrix[newMoveX, newMoveY] == 0) //new space is unoccupied
                {
                    moves.Add((newMoveX, newMoveY));
                }
                else if(otherPiece.Contains(BoardMatrix[newMoveX, newMoveY])) //if new space is occupied by other piece, check for jump
                {
                    int jumpMoveX = newMoveX + xOffset[i];
                    int jumpMoveY = newMoveY + yOffset[i];
                    if (jumpMoveX >= 0 && jumpMoveX < GlobalProperties.SquaresPerBoardSide && jumpMoveY >= 0 && jumpMoveY < GlobalProperties.SquaresPerBoardSide && BoardMatrix[jumpMoveX, jumpMoveY] == 0) //new move is on board and empty
                    {
                        moves.Add((jumpMoveX, jumpMoveY));
                    }    
                }
            }
        }

        return moves;
    }
}