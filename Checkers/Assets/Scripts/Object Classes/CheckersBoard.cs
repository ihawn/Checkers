using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CheckersBoard : CheckersBoardBase
{
    public List<CheckersPiece> Pieces { get; set; }
    public List<CheckersSquare> Squares { get; set; }
    public CheckersGame Game { get; set; }
    public int AIMovesExplored { get; set; }
    public bool DoubleJumpState { get; set; }
    public CheckersPiece JumpingPiece { get; set; }

    public CheckersBoard(CheckersGame game)
    {
        Game = game;
        Pieces = new List<CheckersPiece>();
        Squares = new List<CheckersSquare>();
        AIMovesExplored = 0;
        DoubleJumpState = false;
        int id = 0;

        for(int i = 0; i < GlobalProperties.SquaresPerBoardSide; i++)
        {
            for(int j = 0; j < GlobalProperties.SquaresPerBoardSide; j++)
            {
                Vector2 boardPosition = new Vector2(i, j);
                Vector2 absolutePosition = boardPosition * GlobalProperties.SquareLength;
                Color squareColor = i % 2 == j % 2 ? GlobalProperties.DarkColor : Color.white;
              
                CheckersSquare square = new CheckersSquare(boardPosition, absolutePosition, squareColor, (i*j + i)*GlobalProperties.SpawnDelayOffset);
                CheckersPiece piece = null;

                if ((j <= 2 && squareColor == GlobalProperties.DarkColor) || (j >= GlobalProperties.SquaresPerBoardSide - 3 && squareColor == GlobalProperties.DarkColor))
                {
                    if (j <= 2 && squareColor == GlobalProperties.DarkColor)
                        piece = new CheckersPiece(id, boardPosition, absolutePosition, Color.black, this, (i * j + i) * GlobalProperties.SpawnDelayOffset);
                    else
                        piece = new CheckersPiece(id, boardPosition, absolutePosition, Color.white, this, (i * j + i) * GlobalProperties.SpawnDelayOffset);

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
        Color otherColor = piece.Color == Color.white ? Color.black : Color.white;

        //possible normal moves
        List<Vector2> possibleMoves =
            Squares.Where(s => s.OccupyingPiece == null &&
                               s.Color == GlobalProperties.DarkColor &&
                               s.BoardPosition != piece.BoardPosition &&
                               Vector3.Magnitude(s.BoardPosition - piece.BoardPosition) < 1.5f)
                   .Select(x => x.BoardPosition)      
                   .ToList();

        //possible jump moves
        List<Vector2> jumpMoves = CaclulateJumpMovesForPiece(piece);
            
        possibleMoves = possibleMoves.Concat(jumpMoves).ToList();

        if (!piece.IsKing)
        {
            if (piece.Color == Color.black)
                possibleMoves = possibleMoves.Where(m => m.y > piece.BoardPosition.y).ToList();
            else
                possibleMoves = possibleMoves.Where(m => m.y < piece.BoardPosition.y).ToList();
        }

        return possibleMoves;
    }

    public List<Vector2> CaclulateJumpMovesForPiece(CheckersPiece piece)
    {
        Color otherColor = piece.Color == Color.white ? Color.black : Color.white;
        var possibleMoves = Squares.Where(s => s.OccupyingPiece == null &&
                               s.Color == GlobalProperties.DarkColor &&
                               Vector3.Magnitude(s.BoardPosition - piece.BoardPosition) < 2.83f && //distance is sqrt(8)
                               Vector3.Magnitude(s.BoardPosition - piece.BoardPosition) > 2.82f &&
                               s.BoardPosition != piece.BoardPosition &&
                               Squares.FirstOrDefault(x => x.BoardPosition == new Vector2((s.BoardPosition.x + piece.BoardPosition.x) / 2, (s.BoardPosition.y + piece.BoardPosition.y) / 2)).OccupyingPiece != null && //there is a piece between and it is of the other color
                               Squares.FirstOrDefault(x => x.BoardPosition == new Vector2((s.BoardPosition.x + piece.BoardPosition.x) / 2, (s.BoardPosition.y + piece.BoardPosition.y) / 2)).OccupyingPiece.Color == otherColor)
                   .Select(x => x.BoardPosition)
                   .ToList();
        if (!piece.IsKing)
        {
            if (piece.Color == Color.black)
                possibleMoves = possibleMoves.Where(m => m.y > piece.BoardPosition.y).ToList();
            else
                possibleMoves = possibleMoves.Where(m => m.y < piece.BoardPosition.y).ToList();
        }
        return possibleMoves;
    }
}
