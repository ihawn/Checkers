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
                return HeuristicWithKings(board);
            case 3:
                return HeuristicWithKingsAndKingGuards(board);
            case 4:
                return HeuristicWithKingsAndMoveCounts(board);
            case 5:
                return HeuristicWithKingsAndKingGuardsAndMoveCounts(board);
            case 6:
                return HeuristicWithKingsAndMoveCountsAndCenter(board);
            case 7:
                return HeuristicWithCenter(board);
            default:
                return board.BlackPieceCount - board.WhitePieceCount;
        }
    }

    static int HeuristicWithKings(RawCheckersBoard board)
    {
        int blackVal = 0;
        int whiteVal = 0;
        for(int i = 0; i < GlobalProperties.SquaresPerBoardSide; i++)
        {
            for(int j = 0; j < GlobalProperties.SquaresPerBoardSide; j++)
            {
                switch(board.BoardMatrix[i,j])
                {
                    case 1:
                        blackVal++;
                        break;
                    case 2:
                        whiteVal++;
                        break;
                    case 3:
                        blackVal += GlobalProperties.KingWorth;
                        break;
                    case 4:
                        whiteVal += GlobalProperties.KingWorth;
                        break;
                }
            }
        }
        return blackVal - whiteVal;
    }

    static int HeuristicWithKingsAndKingGuards(RawCheckersBoard board)
    {
        int blackVal = 0;
        int whiteVal = 0;
        for (int i = 0; i < GlobalProperties.SquaresPerBoardSide; i++)
        {
            for (int j = 0; j < GlobalProperties.SquaresPerBoardSide; j++)
            {
                switch (board.BoardMatrix[i, j])
                {
                    case 1:
                        if (j == 0)
                            blackVal += GlobalProperties.KingGuardWorth;
                        else
                            blackVal++;
                        break;
                    case 2:
                        if (j == GlobalProperties.SquaresPerBoardSide - 1)
                            whiteVal += GlobalProperties.KingGuardWorth;
                        else
                            whiteVal++;
                        break;
                    case 3:
                        if (j == 0)
                            blackVal += GlobalProperties.KingGuardWorth;
                        blackVal += GlobalProperties.KingWorth;
                        break;
                    case 4:
                        if (j == GlobalProperties.SquaresPerBoardSide - 1)
                            whiteVal += GlobalProperties.KingGuardWorth;
                        whiteVal += GlobalProperties.KingWorth;
                        break;
                }
            }
        }
        return blackVal - whiteVal;
    }

    static int HeuristicWithKingsAndMoveCounts(RawCheckersBoard board)
    {
        int blackVal = 0;
        int whiteVal = 0;
        for (int i = 0; i < GlobalProperties.SquaresPerBoardSide; i++)
        {
            for (int j = 0; j < GlobalProperties.SquaresPerBoardSide; j++)
            {
                if (board.BoardMatrix[i, j] != 0)
                {
                    int moveCount = board.GetMovesForPiece((i, j)).Count;
                    switch (board.BoardMatrix[i, j])
                    {
                        case 1:
                            blackVal += 1 + moveCount;
                            break;
                        case 2:
                            whiteVal += 1 + moveCount;
                            break;
                        case 3:
                            blackVal += GlobalProperties.KingWorth + moveCount;
                            break;
                        case 4:
                            whiteVal += GlobalProperties.KingWorth + moveCount;
                            break;
                    }
                }
            }
        }
        return blackVal - whiteVal;
    }

    static int HeuristicWithKingsAndMoveCountsAndCenter(RawCheckersBoard board)
    {
        int blackVal = 0;
        int whiteVal = 0;
        for (int i = 0; i < GlobalProperties.SquaresPerBoardSide; i++)
        {
            for (int j = 0; j < GlobalProperties.SquaresPerBoardSide; j++)
            {
                if (board.BoardMatrix[i, j] != 0)
                {
                    int moveCount = board.GetMovesForPiece((i, j)).Count;
                    switch (board.BoardMatrix[i, j])
                    {
                        case 1:
                            blackVal += 1 + moveCount;
                            if (i > 1 && i < GlobalProperties.SquaresPerBoardSide - 1)
                                blackVal += moveCount;
                            break;
                        case 2:
                            whiteVal += 1 + moveCount;
                            if (i > 1 && i < GlobalProperties.SquaresPerBoardSide - 1)
                                whiteVal += moveCount;
                            break;
                        case 3:
                            blackVal += GlobalProperties.KingWorth + moveCount;
                            if (i > 1 && i < GlobalProperties.SquaresPerBoardSide - 1)
                                blackVal += moveCount;
                            break;
                        case 4:
                            whiteVal += GlobalProperties.KingWorth + moveCount;
                            if (i > 1 && i < GlobalProperties.SquaresPerBoardSide - 1)
                                whiteVal += moveCount;
                            break;
                    }
                }
            }
        }
        return blackVal - whiteVal;
    }

    static int HeuristicWithKingsAndKingGuardsAndMoveCounts(RawCheckersBoard board)
    {
        int blackVal = 0;
        int whiteVal = 0;
        for (int i = 0; i < GlobalProperties.SquaresPerBoardSide; i++)
        {
            for (int j = 0; j < GlobalProperties.SquaresPerBoardSide; j++)
            {
                if(board.BoardMatrix[i, j] != 0)
                {
                    int moveCount = board.GetMovesForPiece((i, j)).Count;
                    switch (board.BoardMatrix[i, j])
                    {
                        case 1:
                            if (j == 0)
                                blackVal += GlobalProperties.KingGuardWorth;
                            else
                                blackVal++;
                            blackVal += moveCount;
                            break;
                        case 2:
                            if (j == GlobalProperties.SquaresPerBoardSide - 1)
                                whiteVal += GlobalProperties.KingGuardWorth;
                            else
                                whiteVal++;
                            whiteVal += moveCount;
                            break;
                        case 3:
                            if (j == 0)
                                blackVal += GlobalProperties.KingGuardWorth;
                            blackVal += GlobalProperties.KingWorth;
                            blackVal += moveCount;
                            break;
                        case 4:
                            if (j == GlobalProperties.SquaresPerBoardSide - 1)
                                whiteVal += GlobalProperties.KingGuardWorth;
                            whiteVal += GlobalProperties.KingWorth;
                            whiteVal += moveCount;
                            break;
                    }
                }              
            }
        }
        return blackVal - whiteVal;
    }
    static int HeuristicWithCenter(RawCheckersBoard board)
    {
        int blackVal = 0;
        int whiteVal = 0;
        for (int i = 0; i < GlobalProperties.SquaresPerBoardSide; i++)
        {
            for (int j = 0; j < GlobalProperties.SquaresPerBoardSide; j++)
            {
                if (board.BoardMatrix[i, j] != 0)
                {
                    int moveCount = board.GetMovesForPiece((i, j)).Count;
                    switch (board.BoardMatrix[i, j])
                    {
                        case 1:
                            blackVal++;
                            if (i > 1 && i < GlobalProperties.SquaresPerBoardSide - 1)
                                blackVal++;
                            break;
                        case 2:
                            whiteVal++;
                            if (i > 1 && i < GlobalProperties.SquaresPerBoardSide - 1)
                                whiteVal++;
                            break;
                        case 3:
                            blackVal++;
                            if (i > 1 && i < GlobalProperties.SquaresPerBoardSide - 1)
                                blackVal++;
                            break;
                        case 4:
                            whiteVal++;
                            if (i > 1 && i < GlobalProperties.SquaresPerBoardSide - 1)
                                whiteVal++;
                            break;
                    }
                }
            }
        }
        return blackVal - whiteVal;
    }
}