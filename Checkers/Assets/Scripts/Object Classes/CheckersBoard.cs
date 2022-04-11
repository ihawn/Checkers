using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersBoard
{
    public List<CheckersPiece> Pieces { get; set; }
    public List<CheckersSquare> Squares { get; set; }

    public CheckersBoard()
    {
        Pieces = new List<CheckersPiece>();
        Squares = new List<CheckersSquare>();
        int id = 0;

        for(int i = 0; i < GlobalProperties.SquaresPerBoardSide; i++)
        {
            for(int j = 0; j < GlobalProperties.SquaresPerBoardSide; j++)
            {
                Vector2 boardPosition = new Vector2(i, j);
                Vector2 absolutePosition = boardPosition * GlobalProperties.SquareLength;
                Color squareColor = i % 2 == j % 2 ? GlobalProperties.DarkColor : Color.white;
              
                CheckersSquare square = new CheckersSquare(boardPosition, absolutePosition, squareColor);

                if((j <= 2 && squareColor == GlobalProperties.DarkColor) || (j >= GlobalProperties.SquaresPerBoardSide - 3 && squareColor == GlobalProperties.DarkColor))
                {
                    CheckersPiece piece;


                    if (j <= 2 && squareColor == GlobalProperties.DarkColor)
                        piece = new CheckersPiece(id, boardPosition, absolutePosition, Color.black);
                    else
                        piece = new CheckersPiece(id, boardPosition, absolutePosition, Color.white);

                    piece.PieceGameObject.GetComponent<ObjectLinker>().LinkedObject = piece;
                    Pieces.Add(piece);
                }

                square.SquareGameObject.GetComponent<ObjectLinker>().LinkedObject = square;
                Squares.Add(square);

                id++;
            }
        }
    }
}
