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

    public CheckersPiece(int id, Vector2 boardPosition, Vector2 absolutePosition, Color color, CheckersBoard parentBoard)
    {
        string name = "Piece (" + id + ")";

        Id = id;
        BoardPosition = boardPosition;
        Color = color;
        IsKing = false;
        ParentBoard = parentBoard;
        PieceGameObject = GameManager.MakeGameObjectForObject(GlobalProperties.Cylinder, name, absolutePosition, -Vector3.forward * GlobalProperties.SquareLength * 0.6f, new Vector3(90, 0, 0), color, scaleFactor: 0.8f, heightScaleFactor: 0.1f);       
    }

    public void MovePieceTo(Vector2 newBoardPosition, bool treeTraversalMove = false)
    {
        Vector2 boardOffset = newBoardPosition - BoardPosition;
        Vector2 movementOffset = boardOffset * GlobalProperties.SquareLength;

        CheckersPiece pieceThatWasJumped = null;
        if (Mathf.Abs(boardOffset.x) > 1)
            pieceThatWasJumped = ParentBoard.Pieces.FirstOrDefault(p => p.BoardPosition == new Vector2(BoardPosition.x + boardOffset.x / 2, BoardPosition.y + boardOffset.y / 2));

        if (pieceThatWasJumped != null)
            GameManager.DestroyPiece(pieceThatWasJumped, ParentBoard, treeTraversalMove);

        if(!treeTraversalMove)
            PieceGameObject.transform.position += new Vector3(movementOffset.x, movementOffset.y, 0);
        
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
    }

    void CheckForKing(bool treeTraversalMove = false)
    {
        if((BoardPosition.y == GlobalProperties.SquaresPerBoardSide - 1 && Color == Color.black) ||
            (BoardPosition.y == 0 && Color == Color.white))
        {
            IsKing = true;
            if(!treeTraversalMove)
                GameManager.Coronation(this);
        }
    }
}
