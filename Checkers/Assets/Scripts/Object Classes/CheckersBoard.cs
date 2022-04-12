using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CheckersBoard
{
    public List<CheckersPiece> Pieces { get; set; }
    public List<CheckersSquare> Squares { get; set; }
    public CheckersGame Game { get; set; }

    public CheckersBoard(CheckersGame game)
    {
        Game = game;
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
                CheckersPiece piece = null;

                if ((j <= 2 && squareColor == GlobalProperties.DarkColor) || (j >= GlobalProperties.SquaresPerBoardSide - 3 && squareColor == GlobalProperties.DarkColor))
                {
                    if (j <= 2 && squareColor == GlobalProperties.DarkColor)
                        piece = new CheckersPiece(id, boardPosition, absolutePosition, Color.black, this);
                    else
                        piece = new CheckersPiece(id, boardPosition, absolutePosition, Color.white, this);

                    piece.PieceGameObject.GetComponent<ObjectLinker>().LinkedObject = piece;
                    piece.SquareOccupying = square;
                    Pieces.Add(piece);
                }

                square.OccupyingPiece = piece;
                square.SquareGameObject.GetComponent<ObjectLinker>().LinkedObject = square;
                Squares.Add(square);

                id++;
            }
        }

        for(int i = 0; i < Pieces.Count; i++)
        {
            Pieces[i].PossibleMoves = CalculatePossibleMovesForPiece(Pieces[i]);
        }
    }

    public List<Vector2> CalculatePossibleMovesForPiece(CheckersPiece piece)
    {
        List<Vector2> possibleMoves =
            Squares.Where(s => s.OccupyingPiece == null &&
                               s.Color == GlobalProperties.DarkColor &&
                               Vector3.Magnitude(s.BoardPosition - piece.BoardPosition) < 1.5f)
                   .Select(x => x.BoardPosition)      
                   .ToList();

        if(!piece.IsKing)
        {
            if (piece.Color == Color.black)
                possibleMoves = possibleMoves.Where(m => m.y > piece.BoardPosition.y).ToList();
            else
                possibleMoves = possibleMoves.Where(m => m.y < piece.BoardPosition.y).ToList();
        }

        return possibleMoves;
    }
}
