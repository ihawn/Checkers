using System.Collections.Generic;
using UnityEngine;

public abstract class BoardMovesPiece<BoardType, PieceType>
{
    public virtual BoardType Board { get; set; }
    public virtual List<PieceType> Moves { get; set; }
    public virtual PieceType Piece { get; set; }
}

public class MinimaxInput : BoardMovesPiece<RawCheckersBoard, Coord> 
{
    public MinimaxInput(RawCheckersBoard board, List<Coord> moves, Coord piece)
    {
        Board = board;
        Moves = moves;
        Piece = piece;
    }

    public MinimaxInput(RawCheckersBoard board)
    {
        Board = board;
        Moves = new List<Coord>();
        Piece = new Coord();
    }
}

public class MinimaxCompleteInput : BoardMovesPiece<RawCheckersBoard, Coord>
{
    public int MoveEvaluationCount { get; set; }
}

public class BranchResult : MinimaxCompleteInput
{
    public List<MinimaxInput> Branches { get; set; }

    public BranchResult(List<MinimaxInput> branches, int moveEvaluationCount)
    {
        Branches = branches;
        MoveEvaluationCount = moveEvaluationCount;
    }
}

public class MinimaxResult : MinimaxCompleteInput
{
    public int MinimaxEvaluation { get; set; }

    public MinimaxResult(int moveCount)
    {
        MinimaxEvaluation = 0;
        Board = null;
        Moves = new List<Coord>();
        Piece = new Coord();
        MoveEvaluationCount = moveCount;
    }

    public MinimaxResult(bool isMin, RawCheckersBoard board)
    {
        MinimaxEvaluation = isMin ? int.MaxValue : int.MinValue;
        Board = board;
        Moves = new List<Coord>();
        Piece = new Coord();
        MoveEvaluationCount = 0;
    }

    public MinimaxResult(int evaluation, RawCheckersBoard board, List<Coord> moves, Coord piece, int moveCount)
    {
        MinimaxEvaluation = evaluation;
        Board = board;
        Moves = moves;
        Piece = piece;
        MoveEvaluationCount = moveCount;
    }
}