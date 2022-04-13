using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class TreeOptimizer
{
    public static (int, CheckersBoard, Vector2, CheckersPiece) Minimax((CheckersBoard, Vector2, CheckersPiece) board, int depth, int originalDepth, Vector2 originatingMove, bool isMaximizingPlayer, CheckersPiece originatingPiece = null)
    {
        if (depth == 0 || board.Item1.BlackPiecesCount == 0 || board.Item1.WhitePiecesCount == 0)
            return (board.Item1.BlackPiecesCount - board.Item1.WhitePiecesCount,  board.Item1, board.Item2, board.Item3); //black player wants to maximize this value

        if (isMaximizingPlayer)
        {
            (int, CheckersBoard, Vector2, CheckersPiece) maxEvaluation = (int.MinValue, board.Item1, Vector2.zero, board.Item3);
            List<(CheckersBoard, Vector2, CheckersPiece)> branchBoards = GeneratePositionList(board.Item1, Color.black);

            foreach((CheckersBoard, Vector2, CheckersPiece) branch in branchBoards)
            {
                originatingMove = depth == originalDepth ? branch.Item2 : originatingMove; //keep track of the base piece and base move
                originatingPiece = depth == originalDepth ? branch.Item3 : originatingPiece;

                (int, CheckersBoard, Vector2, CheckersPiece) eval = Minimax((ObjectExtensions.Copy(branch.Item1), ObjectExtensions.Copy(branch.Item2), ObjectExtensions.Copy(branch.Item3)), depth - 1, originalDepth, originatingMove, false, originatingPiece);
                maxEvaluation = eval.Item1 > maxEvaluation.Item1 ? eval : maxEvaluation;
            }
            return (maxEvaluation.Item1, maxEvaluation.Item2, originatingMove, originatingPiece);
        }
        else
        {
            (int, CheckersBoard, Vector2, CheckersPiece) minEvaluation = (int.MaxValue, board.Item1, Vector2.zero, board.Item3);
            List<(CheckersBoard, Vector2, CheckersPiece)> branchBoards = GeneratePositionList(board.Item1, Color.white);
                
            foreach ((CheckersBoard, Vector2, CheckersPiece) branch in branchBoards)
            {
                originatingMove = depth == originalDepth ? branch.Item2 : originatingMove; //keep track of the base piece and base move
                originatingPiece = depth == originalDepth ? branch.Item3 : originatingPiece;

                (int, CheckersBoard, Vector2, CheckersPiece) eval = Minimax((ObjectExtensions.Copy(branch.Item1), ObjectExtensions.Copy(branch.Item2), ObjectExtensions.Copy(branch.Item3)), depth - 1, originalDepth, originatingMove, true, originatingPiece);
                minEvaluation = eval.Item1 < minEvaluation.Item1 ? eval : minEvaluation;
            }
            return (minEvaluation.Item1, minEvaluation.Item2, originatingMove, originatingPiece);
        }
    }

    static List<(CheckersBoard, Vector2, CheckersPiece)> GeneratePositionList(CheckersBoard baseBoard, Color whoseTurn)
    {
        List<(CheckersBoard, Vector2, CheckersPiece)> boardListWithGeneratingMove = new List<(CheckersBoard, Vector2, CheckersPiece)>();
        List<CheckersPiece> currentPlayerPieces = baseBoard.Pieces.Where(p => p.Color == whoseTurn).ToList();

        for(int i = 0; i < currentPlayerPieces.Count; i++)
            currentPlayerPieces[i].PossibleMoves = baseBoard.CalculatePossibleMovesForPiece(currentPlayerPieces[i]);
        List<CheckersPiece> piecesWithMoves = currentPlayerPieces.Where(p => p.PossibleMoves.Count > 0).ToList();

        foreach(CheckersPiece piece in piecesWithMoves)
        {
            foreach(Vector2 move in piece.PossibleMoves)
            {
                CheckersBoard branchBoard = ObjectExtensions.Copy(baseBoard);
                CheckersPiece pieceFromCopy = branchBoard.Pieces.FirstOrDefault(p => p.BoardPosition == piece.BoardPosition);
                pieceFromCopy.MovePieceTo(move, treeTraversalMove: true);
                boardListWithGeneratingMove.Add((branchBoard, move, piece));
            }
        }

        return boardListWithGeneratingMove;
    }
}
