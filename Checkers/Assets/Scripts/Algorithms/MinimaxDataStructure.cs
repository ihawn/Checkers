using System.Collections.Generic;

public abstract class BoardMovesPiece<BoardType, PieceType>
{
    public virtual BoardType Board { get; set; }
    public virtual List<PieceType> Moves { get; set; }
    public virtual PieceType Piece { get; set; }
}

public class MinimaxInput : BoardMovesPiece<RawCheckersBoard, (int, int)> 
{
    public MinimaxInput(RawCheckersBoard board, List<(int, int)> moves, (int, int) piece)
    {
        Board = board;
        Moves = moves;
        Piece = piece;
    }

    public MinimaxInput(RawCheckersBoard board)
    {
        Board = board;
        Moves = new List<(int, int)>();
        Piece = (-1, -1);
    }
}

public class MinimaxCompleteInput : BoardMovesPiece<RawCheckersBoard, (int, int)>
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
        Moves = new List<(int, int)>();
        Piece = (-1, -1);
        MoveEvaluationCount = moveCount;
    }

    public MinimaxResult(bool isMin, RawCheckersBoard board)
    {
        MinimaxEvaluation = isMin ? int.MaxValue : int.MinValue;
        Board = board;
        Moves = new List<(int, int)>();
        Piece = (-1, -1);
        MoveEvaluationCount = 0;
    }

    public MinimaxResult(int evaluation, RawCheckersBoard board, List<(int, int)> moves, (int, int) piece, int moveCount)
    {
        MinimaxEvaluation = evaluation;
        Board = board;
        Moves = moves;
        Piece = piece;
        MoveEvaluationCount = moveCount;
    }
}