using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersPiece : MonoBehaviour
{
    public int Id { get; }
    public Vector2 BoardPosition { get; set; }
    public Color Color { get; set; }
    public bool IsKing { get; set; }
    public GameObject PieceGameObject { get; }

    public CheckersPiece(int id, Vector2 boardPosition, Vector2 absolutePosition, Color color)
    {
        string name = "Piece (" + boardPosition.x + ", " + boardPosition.y + ")";

        Id = id;
        BoardPosition = boardPosition;
        Color = color;
        IsKing = false;
        PieceGameObject = GameManager.MakeGameObjectForObject(GlobalProperties.Cylinder, name, absolutePosition, -Vector3.forward * GlobalProperties.SquareLength * 0.6f, new Vector3(90, 0, 0), color, this, scaleFactor: 0.8f, heightScaleFactor: 0.1f);       
    }
}
