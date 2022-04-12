using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersSquare
{
    public Vector2 BoardPosition { get; set; }
    public Color Color { get; set; }
    public GameObject SquareGameObject { get; }
    public CheckersPiece OccupyingPiece { get; set; }

    public CheckersSquare(Vector2 boardPosition, Vector2 absolutePosition, Color color)
    {
        string name = "Square (" + boardPosition.x + ", " + boardPosition.y + ")";

        BoardPosition = boardPosition;
        Color = color;
        SquareGameObject = GameManager.MakeGameObjectForObject(GlobalProperties.Cube, name, absolutePosition, Vector3.zero, Vector3.zero, color);
    }
}
