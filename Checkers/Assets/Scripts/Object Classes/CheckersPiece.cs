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
        string name = "Piece (" + boardPosition.x + ", " + boardPosition.y + ")";

        Id = id;
        BoardPosition = boardPosition;
        Color = color;
        IsKing = false;
        ParentBoard = parentBoard;
        PieceGameObject = GameManager.MakeGameObjectForObject(GlobalProperties.Cylinder, name, absolutePosition, -Vector3.forward * GlobalProperties.SquareLength * 0.6f, new Vector3(90, 0, 0), color, scaleFactor: 0.8f, heightScaleFactor: 0.1f);       
    }

    public void MovePieceTo(Vector2 newBoardPosition)
    {
        Vector2 movementOffset = (newBoardPosition - BoardPosition) * GlobalProperties.SquareLength;
        PieceGameObject.transform.position += new Vector3(movementOffset.x, movementOffset.y, 0);
        BoardPosition = newBoardPosition;
        ParentBoard.CalculatePossibleMovesForPiece(this);
        ParentBoard.Game.CurrentPlayer.SelectedPiece = null;
        
        SquareOccupying.OccupyingPiece = null;
        CheckersSquare newSquare = ParentBoard.Squares.FirstOrDefault(s => s.BoardPosition == newBoardPosition);
        newSquare.OccupyingPiece = this;
        SquareOccupying = newSquare;

        CheckForKing();
    }

    void CheckForKing()
    {

    }
}
