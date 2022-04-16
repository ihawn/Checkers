using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public PlayerType Type { get; private set; }
    public Color PlayerColor { get; private set; }
    public CheckersPiece SelectedPiece { get; set; }
    public int HeuristicId { get; set; }
    public int Id { get; set; }

    public Player(PlayerType type, int heuristicId, Color playerColor)
    {
        Type = type;
        PlayerColor = playerColor;
        HeuristicId = heuristicId;
    }
}

public enum PlayerType
{
    Human = 0,
    DumbAI = 1,
    KindaDumbAI = 2,
    SmartAI = 3,
    ReallySmartAI = 4,
    GeniusAI = 5,
    Cthulu = 6
}
