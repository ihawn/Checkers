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

public class BranchResult : BoardMovesPiece<RawCheckersBoard, Coord>
{
    public List<MinimaxInput> Branches { get; set; }

    public BranchResult(List<MinimaxInput> branches)
    {
        Branches = branches;
    }
}

public class MinimaxResult : BoardMovesPiece<RawCheckersBoard, Coord>
{
    public int MinimaxEvaluation { get; set; }

    public MinimaxResult()
    {
        MinimaxEvaluation = 0;
        Board = null;
        Moves = new List<Coord>();
        Piece = new Coord();
    }

    public MinimaxResult(bool isMin, RawCheckersBoard board)
    {
        MinimaxEvaluation = isMin ? int.MaxValue : int.MinValue;
        Board = board;
        Moves = new List<Coord>();
        Piece = new Coord();
    }

    public MinimaxResult(int evaluation, RawCheckersBoard board, List<Coord> moves, Coord piece)
    {
        MinimaxEvaluation = evaluation;
        Board = board;
        Moves = moves;
        Piece = piece;
    }
}
