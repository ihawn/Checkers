using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CheckersPiece
{
    public int Id { get; }
    public Vector2 BoardPosition { get; set; }
    public Color Color { get; set; }
    public bool IsKing { get; set; }
    public GameObject PieceGameObject { get; }
    public List<Vector2> PossibleMoves { get; set; }
    public CheckersBoard ParentBoard { get; set; }
    public CheckersSquare SquareOccupying { get; set; }
    public Queue<Vector2> PastMoves { get; private set; }

    public CheckersPiece(int id, Vector2 boardPosition, Vector2 absolutePosition, Color color, CheckersBoard parentBoard, float gameObjectSpawnDelay)
    {
        string name = "Piece (" + id + ")";

        Id = id;
        BoardPosition = boardPosition;
        Color = color;
        IsKing = false;
        ParentBoard = parentBoard;
        PieceGameObject = GlobalProperties.GameManager.MakeGameObjectForObject(GlobalProperties.Cylinder, name, absolutePosition, -Vector3.forward * GlobalProperties.SquareLength * 0.6f, new Vector3(90, 0, 0), color, gameObjectSpawnDelay, scaleFactor: 0.8f, heightScaleFactor: 0.1f);       
    }

    public void MovePieceTo(Vector2 newBoardPosition)
    {
        Vector2 boardOffset = newBoardPosition - BoardPosition;
        Vector2 movementOffset = boardOffset * GlobalProperties.SquareLength;

        CheckersPiece pieceThatWasJumped = null;
        if (Mathf.Abs(boardOffset.x) > 1)
            pieceThatWasJumped = ParentBoard.Pieces.FirstOrDefault(p => p.BoardPosition == new Vector2(BoardPosition.x + boardOffset.x / 2, BoardPosition.y + boardOffset.y / 2));

        if (pieceThatWasJumped != null)
            GameManager.DestroyPiece(pieceThatWasJumped, ParentBoard);
        
        BoardPosition = newBoardPosition;
        PossibleMoves = ParentBoard.CalculatePossibleMovesForPiece(this);
        ParentBoard.Game.CurrentPlayer.SelectedPiece = null;
        
        SquareOccupying.OccupyingPiece = null;
        CheckersSquare newSquare = ParentBoard.Squares.FirstOrDefault(s => s.BoardPosition == newBoardPosition);
        newSquare.OccupyingPiece = this;
        SquareOccupying = newSquare;

        ParentBoard.WhitePiecesCount = ParentBoard.Pieces.Where(p => p.Color == Color.white).Count();
        ParentBoard.BlackPiecesCount = ParentBoard.Pieces.Where(p => p.Color == Color.black).Count();

        CheckForKing();

        //check for multiple jumps (human only. AI double jumping is handled in TreeOptimizer)
        List<Vector2> jumpMoves = ParentBoard.CaclulateJumpMovesForPiece(this);
        if (jumpMoves.Count > 0 && Mathf.Abs(boardOffset.x) > 1 && ParentBoard.Game.CurrentPlayer.Type == PlayerType.Human)
        {
            ParentBoard.DoubleJumpState = true;
            ParentBoard.JumpingPiece = this;
            GlobalProperties.GameManager.HighlightPiece(this);
        }
        else
        {
            ParentBoard.DoubleJumpState = false;
            ParentBoard.JumpingPiece = null;
            GlobalProperties.GameManager.SwitchPlayer();
        }
    }

    void CheckForKing()
    {
        if(((BoardPosition.y == GlobalProperties.SquaresPerBoardSide - 1 && Color == Color.black) ||
            (BoardPosition.y == 0 && Color == Color.white)) && !IsKing)
        {
            IsKing = true;
            GlobalProperties.GameManager.Coronation(this);
        }
    }
}
