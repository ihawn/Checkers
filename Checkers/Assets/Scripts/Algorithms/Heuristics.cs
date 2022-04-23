using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GlobalProperties.SquaresPerBoardSide replaced with 8 for speed
//black wants to maximize heuristic
public static class Heuristics
{
    public static int Heuristic(RawCheckersBoard board, int which)
    {
        switch(which)
        {
            case 1:
                return board.BlackPiecesCount - board.WhitePiecesCount;
            case 2:
                return BoardScore(board);
            case 3:
                return PieceDistance(board)/2 + BoardScore(board);
            default:
                return board.BlackPiecesCount - board.WhitePiecesCount;
        }
    }

    //https://github.com/Hsankesara/Draughts-AI
    static int BoardScore(RawCheckersBoard board)
    {
        int score = 0;
        for (int i = 0; i < 8; i++)
        {
            for (int j = i % 2 == 0 ? 0 : 1; j < 8; j+=2)
            {
                if (board.BoardMatrix[i, j] != 0)
                {
                    if (board.BoardMatrix[i, j] == 3)
                        score += 10;
                    else if (board.BoardMatrix[i, j] == 4)
                        score -= 10;

                    else if (board.BoardMatrix[i, j] == 1)
                    {
                        if (j > 4)
                            score += 7;
                        else
                            score += 5;
                    }
                    else if (board.BoardMatrix[i, j] == 2)
                    {
                        if (j < 4)
                            score -= 7;
                        else
                            score -= 5;
                    }
                }
            }
        }

        return score;
    }

    static int PieceDistance(RawCheckersBoard board)
    {
        List<(int, int)> blackPieces = new List<(int, int)>();
        List<(int, int)> whitePieces = new List<(int, int)>();

        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                if (board.BoardMatrix[i,j] == 1 || board.BoardMatrix[i,j] == 3)
                    blackPieces.Add((i, j));
                else if(board.BoardMatrix[i, j] == 2 || board.BoardMatrix[i,j] == 4)
                    whitePieces.Add((i, j));
            }
        }

        int sumDist = 0;
        foreach(var blackPiece in blackPieces)
        {
            foreach(var whitePiece in whitePieces)
            {
                sumDist = Mathf.Abs(blackPiece.Item1 - whitePiece.Item1) + Mathf.Abs(blackPiece.Item2 - whitePiece.Item2);
            }
        }

        return blackPieces.Count > whitePieces.Count ? -sumDist : sumDist;
    }
}
