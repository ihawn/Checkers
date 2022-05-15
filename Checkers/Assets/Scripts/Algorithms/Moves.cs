using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TakeTurns;
using TakeTurns.Containers;
using System.Linq;

public class Moves : TakeTurns<RawCheckersBoard, Coord, Coord>
{
    public override float GetGameEvaluation(RawCheckersBoard board)
    {
        return Heuristics.Heuristic(board, 3);
    }

    public override bool EndGameReached(RawCheckersBoard board)
    {
        return board.GetPieceCountWithMoves() == 0;
    }

    public override IList<MinimaxInput<RawCheckersBoard, Coord, Coord, float>> GetPositions(RawCheckersBoard baseBoard, bool isMaxPlayer)
    {
        IList<MinimaxInput<RawCheckersBoard, Coord, Coord, float>> boardListWithGeneratingMove = new List<MinimaxInput<RawCheckersBoard, Coord, Coord, float>>();
        int[] whoseTurn = isMaxPlayer ? new int[] { 1, 3 } : new int[] { 2, 4 };

        for (int i = 0; i < 8; i++)
        {
            for (int j = i % 2 == 0 ? 0 : 1; j < 8; j++)
            {
                if (baseBoard.BoardMatrix[i, j] == whoseTurn[0] || baseBoard.BoardMatrix[i, j] == whoseTurn[1])
                {
                    List<Coord> moves = baseBoard.GetMovesForPiece(new Coord(i, j));
                    foreach (Coord move in moves)
                    {
                        RawCheckersBoard branchBoard = new RawCheckersBoard(baseBoard);
                        branchBoard.MovePiece(new Coord(i, j), move);
                        List<Coord> movesForPiece = new List<Coord>() { move };

                        //multiple jumps
                        Coord jumpMove = move;
                        Coord currentPos = new Coord(i, j);
                        while (Math.Abs(currentPos.x - jumpMove.x) == 2)
                        {
                            List<Coord> jumpMoves = branchBoard.GetMovesForPiece(jumpMove).Where(m => Math.Abs(jumpMove.x - m.x) > 1.1f).ToList();
                            if (jumpMoves.Count == 0)
                                break;
                            else
                            {
                                currentPos = jumpMove;
                                jumpMove = jumpMoves[0];
                                branchBoard.MovePiece(currentPos, jumpMove);
                                movesForPiece.Add(jumpMove);
                            }
                        }

                        boardListWithGeneratingMove.Add(new MinimaxInput<RawCheckersBoard, Coord, Coord, float>(branchBoard, movesForPiece, new Coord(i, j)));
                    }
                }
            }
        }
        boardListWithGeneratingMove.Shuffle();
        return boardListWithGeneratingMove;

    }
}

static class ShuffleExtension
{
    private static System.Random rng = new System.Random();
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}

//Bare bones checkers board class designed to be deep-copied quickly. Only to be used for decision tree checking
public class RawCheckersBoard : CheckersBoardBase
{
    //0 = empty, 1 = black, 2 = white, 3 = black king, 4 = white king
    public int[,] BoardMatrix { get; set; }

    public RawCheckersBoard(CheckersBoard oopBoard)
    {
        BoardMatrix = new int[8, 8];
        BlackPiecesCount = 0;
        WhitePiecesCount = 0;

        foreach (CheckersPiece piece in oopBoard.Pieces)
        {
            if (piece.IsKing)
                BoardMatrix[(int)piece.BoardPosition.x, (int)piece.BoardPosition.y] = piece.Color == Color.black ? 3 : 4;
            else
                BoardMatrix[(int)piece.BoardPosition.x, (int)piece.BoardPosition.y] = piece.Color == Color.black ? 1 : 2;
            if (piece.Color == Color.black)
                BlackPiecesCount++;
            else
                WhitePiecesCount++;
        }
    }

    public RawCheckersBoard(RawCheckersBoard oldBoard)
    {
        BlackPiecesCount = oldBoard.BlackPiecesCount;
        WhitePiecesCount = oldBoard.WhitePiecesCount;

        BoardMatrix = new int[8, 8];
        Buffer.BlockCopy(oldBoard.BoardMatrix, 0, BoardMatrix, 0, 256);
    }

    public void MovePiece(Coord coord1, Coord coord2)
    {
        //movement
        int piece = BoardMatrix[coord1.x, coord1.y];
        BoardMatrix[coord2.x, coord2.y] = piece;
        BoardMatrix[coord1.x, coord1.y] = 0;

        //check jump
        if (Math.Abs(coord1.x - coord2.x) == 2)
        {
            switch (BoardMatrix[(coord1.x + coord2.x) / 2, (coord1.y + coord2.y) / 2])
            {
                case 1:
                    BlackPiecesCount--;
                    break;
                case 2:
                    WhitePiecesCount--;
                    break;
                case 3:
                    BlackPiecesCount--;
                    break;
                case 4:
                    WhitePiecesCount--;
                    break;
            }

            BoardMatrix[(coord1.x + coord2.x) / 2, (coord1.y + coord2.y) / 2] = 0;
        }

        //check king
        if (BoardMatrix[coord2.x, coord2.y] == 1 && coord2.y == 8 - 1) //black king
            BoardMatrix[coord2.x, coord2.y] = 3;
        else if (BoardMatrix[coord2.x, coord2.y] == 2 && coord2.y == 0) //white king
            BoardMatrix[coord2.x, coord2.y] = 4;
    }

    public List<Coord> GetMovesForPiece(Coord piece)
    {
        List<Coord> moves = new List<Coord>();
        int[] xOffset;
        int[] yOffset;
        if (BoardMatrix[piece.x, piece.y] == 3 || BoardMatrix[piece.x, piece.y] == 4) //kings
        {
            xOffset = new int[] { -1, -1, 1, 1 };
            yOffset = new int[] { 1, -1, 1, -1 };
        }
        else if (BoardMatrix[piece.x, piece.y] == 1) //black non-king
        {
            xOffset = new int[] { -1, 1 };
            yOffset = new int[] { 1, 1 };
        }
        else //white non-king
        {
            xOffset = new int[] { -1, 1 };
            yOffset = new int[] { -1, -1 };
        }

        int[] otherPiece = BoardMatrix[piece.x, piece.y] == 1 || BoardMatrix[piece.x, piece.y] == 3 ? new int[] { 2, 4 } : new int[] { 1, 3 };

        for (int i = 0; i < xOffset.Length; i++)
        {
            int newMoveX = piece.x + xOffset[i];
            int newMoveY = piece.y + yOffset[i];
            if (newMoveX >= 0 && newMoveX < 8 && newMoveY >= 0 && newMoveY < 8) //new move is on board
            {
                if (BoardMatrix[newMoveX, newMoveY] == otherPiece[0] || BoardMatrix[newMoveX, newMoveY] == otherPiece[1]) //if new space is occupied by other piece, check for jump
                {
                    int jumpMoveX = newMoveX + xOffset[i];
                    int jumpMoveY = newMoveY + yOffset[i];
                    if (jumpMoveX >= 0 && jumpMoveX < 8 && jumpMoveY >= 0 && jumpMoveY < 8 && BoardMatrix[jumpMoveX, jumpMoveY] == 0) //new move is on board and empty
                    {
                        moves.Add(new Coord(jumpMoveX, jumpMoveY));
                    }
                }
                else if (BoardMatrix[newMoveX, newMoveY] == 0) //new space is unoccupied
                {
                    moves.Add(new Coord(newMoveX, newMoveY));
                }
            }
        }
        return moves;
    }
    public int GetPieceCountWithMoves()
    {
        int count = 0;
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                if (BoardMatrix[i, j] != 0 && GetMovesForPiece(new Coord(i, j)).Count > 0)
                    count++;
        return count;
    }
}

public class Coord
{
    public int x;
    public int y;

    public Coord()
    {
        x = -1;
        y = -1;
    }

    public Coord(int X, int Y)
    {
        x = X;
        y = Y;
    }

    public static implicit operator Vector2(Coord rawMove) => new Vector2(rawMove.x, rawMove.y);
}
