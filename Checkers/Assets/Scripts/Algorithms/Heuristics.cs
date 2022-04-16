using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//black wants to maximize heuristic
public static class Heuristics
{
    public static int Heuristic(RawCheckersBoard board, int which)
    {
        switch(which)
        {
            case 1:
                return board.BlackPieceCount - board.WhitePieceCount;
            case 2:
                return BoardScore(board);
            default:
                return board.BlackPieceCount - board.WhitePieceCount;
        }
    }

    //https://github.com/Hsankesara/Draughts-AI
    static int BoardScore(RawCheckersBoard board)
    {
        int score = 0;
        for (int i = 0; i < GlobalProperties.SquaresPerBoardSide; i++)
        {
            for (int j = 0; j < GlobalProperties.SquaresPerBoardSide; j++)
            {
                if(board.BoardMatrix[i, j] != 0)
                {
                    if (board.BoardMatrix[i, j] == 3)
                        score += 10;
                    else if (board.BoardMatrix[i, j] == 4)
                        score -= 10;

                    else if (board.BoardMatrix[i, j] == 1)
                    {
                        if (j > GlobalProperties.SquaresPerBoardSide / 2)
                            score += 7;
                        else
                            score += 5;
                    }
                    else if (board.BoardMatrix[i, j] == 2)
                    {
                        if (j < GlobalProperties.SquaresPerBoardSide / 2)
                            score -= 7;
                        else
                            score -= 5;
                    }
                }
            }
        }
        return score;
    }
}
